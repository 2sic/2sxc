using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Configuration;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// Custom base controller class for custom dynamic 2sxc app api controllers.
    /// It is without dependencies in class constructor, commonly provided with DI.
    /// </summary>
    public abstract class ApiController : OqtControllerBase, IHasOqtaneDynamicCodeContext /*, DynamicApiController, IHttpController, IDisposable, IHasDynCodeContext, IDynamicWebApi, IDnnDynamicCode, IDynamicCode, ICreateInstance, ICompatibilityLevel, IHasLog, IDynamicCodeBeforeV10*/
    {
        protected IServiceProvider ServiceProvider { get; private set; }

        protected override string HistoryLogName { get; } = "oqt-api-controller";

        private OqtState _oqtState;

        /// <summary>
        /// Our custom dynamic 2sxc app api controllers, depends on event OnActionExecuting to provide dependencies (without DI in constructor).
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var httpContext = context.HttpContext;
            ServiceProvider = httpContext.RequestServices;
            HttpRequest GetRequest() => httpContext.Request;
            _oqtState = new OqtState(GetRequest, ServiceProvider, Log);

            var getBlock = _oqtState.GetBlock(true);

            DynCode = ServiceProvider.Build<OqtaneDynamicCode>().Init(getBlock, Log);

            var stxResolver = ServiceProvider.Build<IContextResolver>(typeof(IContextResolver));
            stxResolver.AttachRealBlock(() => getBlock);
            stxResolver.AttachBlockContext(() => _oqtState.GetContext());

            if (context.HttpContext.Items.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var createInstancePath))
                CreateInstancePath = createInstancePath as string;
        }


        public OqtaneDynamicCode DynCode { get; set; }

        [PrivateApi] public int CompatibilityLevel => DynCode.CompatibilityLevel;

        /// <inheritdoc />
        public IApp App => DynCode?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => DynCode?.Data;

        #region AsDynamic implementations
        /// <inheritdoc/>
        [NonAction]
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynCode?.AsDynamic(json, fallback);

        /// <inheritdoc />
        [NonAction]
        public dynamic AsDynamic(IEntity entity) => DynCode?.AsDynamic(entity);

        /// <inheritdoc />
        [NonAction]
        public dynamic AsDynamic(object dynamicEntity) => DynCode?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        [NonAction]
        public IEntity AsEntity(object dynamicEntity) => DynCode?.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        [NonAction]
        public IEnumerable<dynamic> AsList(object list) => DynCode?.AsList(list);

        #endregion

        #region CreateSource implementations

        /// <inheritdoc />
        [NonAction]
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => DynCode.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
        [NonAction]
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => DynCode.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public dynamic Content => DynCode?.Content;

        /// <inheritdoc />
        public dynamic Header => DynCode?.Header;


        #endregion

        #region Adam

        /// <inheritdoc />
        [NonAction]
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCode?.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        [NonAction]
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCode?.AsAdam(entity, fieldName);


        // Adam - Shared Code Across the APIs (prevent duplicate code)

        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        [NonAction]
        public Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "SaveInAdam",
                $"{nameof(stream)},{nameof(fileName)},{nameof(contentType)},{nameof(guid)},{nameof(field)},{nameof(subFolder)} (optional)");

            if (stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[] { FeatureIds.UseAdamInWebApi, FeatureIds.PublicUpload };
            if (!Eav.Configuration.Features.EnabledOrException(feats, "can't save in ADAM", out var exp))
                throw exp;

            var appId = DynCode?.Block?.AppId ?? DynCode?.App?.AppId ?? throw new Exception("Error, SaveInAdam needs an App-Context to work, but the App is not known.");
            return ServiceProvider.Build<AdamTransUpload<int, int>>(typeof(AdamTransUpload<int, int>))
                .Init(appId, contentType, guid.Value, field, false, Log)
                .UploadOne(stream, fileName, subFolder, true);
        }

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc />
        public ILinkHelper Link => DynCode?.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => DynCode?.Edit;

        #endregion

        #region  CreateInstance implementation

        public string CreateInstancePath { get; set; }

        [NonAction]
        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            DynCode.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, CreateInstancePath, throwOnError);

        #endregion

        /// <inheritdoc />
        /// TODO: make Oqt implementation
        //public new IDnnContext Dnn => base.Dnn;


        #region RunContext WiP

        public ICmsContext CmsContext => DynCode?.CmsContext;


        #endregion
    }
}
