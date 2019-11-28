using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.DataSources;
using Factory = ToSic.Eav.Factory;
using ToSic.Sxc.Adam.WebApi;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using DynamicCodeHelper = ToSic.Sxc.Dnn.DynamicCodeHelper;
using IApp = ToSic.Sxc.Apps.IApp;
using IDynamicCode = ToSic.Sxc.Dnn.IDynamicCode;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.SexyContent.WebApi
{
    /// <inheritdoc cref="SxcApiControllerBase" />
    /// <summary>
    /// This is the base class for API Controllers which need the full context
    /// incl. the current App, DNN, Data, etc.
    /// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
    /// safer because it can't accidentally mix the App with a different appId in the params
    /// </summary>
    [SxcWebApiExceptionHandling]
    public abstract class SxcApiController : SxcApiControllerBase, IDynamicWebApi
    {
        #region constructor

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            // Note that the SxcBlock is created by the BaseClass, if it's detectable. Otherwise it's null
            DnnAppAndDataHelpers = new DynamicCodeHelper(CmsBlock, CmsBlock?.Log ?? Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (DnnAppAndDataHelpers.App == null)
                TryToAttachAppFromUrlParams();

            // must run this after creating AppAndDataHelpers
            controllerContext.Request.Properties.Add(Constants.DnnContextKey, Dnn); 

            if(controllerContext.Request.Properties.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var value))
                SharedCodeVirtualRoot = value as string;
        }
        #endregion

        private DynamicCodeHelper DnnAppAndDataHelpers { get; set; }

        public IDnnContext Dnn => DnnAppAndDataHelpers.Dnn;

        public SxcHelper Sxc => DnnAppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public IApp App => DnnAppAndDataHelpers.App;

        private void TryToAttachAppFromUrlParams()
        {
            var wrapLog = Log.Call("TryToAttachAppFromUrlParams");
            var found = false;
            try
            {
                var routeAppPath = Route.AppPathOrNull(Request.GetRouteData());
                var appId = AppFinder.GetCurrentAppIdFromPath(routeAppPath).AppId;
                // Look up if page publishing is enabled - if module context is not available, always false
                var publish = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var publishingEnabled = Dnn.Module != null && publish.IsEnabled(Dnn.Module.ModuleID);
                var app = Environment.Dnn7.Factory.App(appId, publishingEnabled) as IApp;
                DnnAppAndDataHelpers.LateAttachApp(app);
                found = true;
            } catch { /* ignore */ }

            wrapLog(found.ToString());
        }

        /// <inheritdoc />
        public IBlockDataSource Data => DnnAppAndDataHelpers.Data;


        #region AsDynamic implementations

        /// <inheritdoc cref="IDynamicCode" />
        public dynamic AsDynamic(IEntity entity) => DnnAppAndDataHelpers.AsDynamic(entity);

        /// <inheritdoc cref="IDynamicCode" />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  DnnAppAndDataHelpers.AsDynamic(dynamicEntity);

        // todo: only in "old" controller, not in new one
        /// <inheritdoc cref="IDynamicCode" />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  DnnAppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);

        /// <inheritdoc cref="IDynamicCode" />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) =>  DnnAppAndDataHelpers.AsDynamic(stream.List);

        /// <inheritdoc cref="IDynamicCode" />
        public IEntity AsEntity(dynamic dynamicEntity) =>  DnnAppAndDataHelpers.AsEntity(dynamicEntity);

        /// <inheritdoc cref="IDynamicCode" />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  DnnAppAndDataHelpers.AsDynamic(entities);
        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => DnnAppAndDataHelpers.AsDynamic(entity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => DnnAppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => DnnAppAndDataHelpers.AsDynamic(entities);
        #endregion


        #region CreateSource implementations
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
	        ITokenListFiller configurationProvider = null)
	        => DnnAppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, ITokenListFiller configurationProvider = null)
            where T : IDataSource
            =>  DnnAppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);

	    public T CreateSource<T>(IDataStream inStream) where T : IDataSource 
            => DnnAppAndDataHelpers.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List
        /// <summary>
        /// content item of the current view
        /// </summary>
        public dynamic Content => DnnAppAndDataHelpers.Content;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Content.Presentation instead")]
        public dynamic Presentation => DnnAppAndDataHelpers.Content?.Presentation;

        public dynamic Header => DnnAppAndDataHelpers.Header;

        [Obsolete("use Header instead")]
	    public dynamic ListContent => DnnAppAndDataHelpers.Header;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Header.Presentation instead")]
	    public dynamic ListPresentation => DnnAppAndDataHelpers.Header?.Presentation;

        [Obsolete("This is an old way used to loop things. Use Data[\"Default\"] instead. Will be removed in 2sxc v10")]
        public List<Element> List => DnnAppAndDataHelpers.List;

        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
	        => DnnAppAndDataHelpers.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DnnAppAndDataHelpers.AsAdam(entity, fieldName);


        /// <summary>
        /// Save a file from a stream (usually an upload from the browser) into an adam-field
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">ensure that all parameters use names, so the api can change in future</param>
        /// <param name="stream">the stream</param>
        /// <param name="fileName">file name to save to</param>
        /// <param name="contentType">content-type of the target item (important for security checks)</param>
        /// <param name="guid"></param>
        /// <param name="field"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
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

            if(stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[]{FeatureIds.UseAdamInWebApi, FeatureIds.PublicUpload};
            if (!Features.EnabledOrException(feats, "can't save in ADAM", out var exp))
                throw exp;

            return new AdamUploader(CmsBlock, 
                CmsBlock.Block.AppId, // 2019-11-09 not nullable any more ?? throw new Exception("can't save in adam - full context not available"), 
                Log)
                .UploadOne(stream, fileName, contentType, guid.Value, field, subFolder, false, true);
        }

        #endregion

        #region Link & Edit - added in 2sxc 10.01
        public ILinkHelper Link => DnnAppAndDataHelpers?.Link;
        public IInPageEditingSystem Edit => DnnAppAndDataHelpers?.Edit;

        #endregion


        public string SharedCodeVirtualRoot { get; set; }

        public dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null, 
            string relativePath = null, 
            bool throwOnError = true) =>
            DnnAppAndDataHelpers.CreateInstance(virtualPath, dontRelyOnParameterOrder, name,
                SharedCodeVirtualRoot, throwOnError);
    }
}
