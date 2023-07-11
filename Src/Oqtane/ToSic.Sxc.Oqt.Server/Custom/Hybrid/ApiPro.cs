using System.Collections.Generic;
using System;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using Constants = ToSic.Sxc.Constants;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.Oqt.Server.Custom;
using static ToSic.Eav.Parameters;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ToSic.Sxc.Adam;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Oqtane specific Api base class.
    ///
    /// It's identical to [](xref:Custom.Hybrid.Api14) but this may be enhanced in future. 
    /// </summary>
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    [JsonFormatter]
    public abstract class ApiPro : OqtStatefulControllerBase, IDynamicWebApi, IHasCodeLog, IHasDynamicCodeRoot, IDynamicCode16
    {
        #region setup

        protected ApiPro() : this(EavWebApiConstants.HistoryNameWebApi) { }

        protected ApiPro(string logSuffix) : base(logSuffix) { }

        /// <summary>
        /// Our custom dynamic 2sxc app api controllers, depends on event OnActionExecuting to provide dependencies (without DI in constructor).
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            CtxHlp.OnActionExecutingEnd(context);
        }

        [PrivateApi] public int CompatibilityLevel => Constants.CompatibilityLevel16;

        #endregion

        #region Infrastructure

        /// <inheritdoc cref="IHasCodeLog.Log" />
        public new ICodeLog Log => CtxHlp.CodeLog;

        // ReSharper disable once InconsistentNaming
        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot => CtxHlp._DynCodeRoot;

        /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
        public new TService GetService<TService>() where TService : class => _DynCodeRoot.GetService<TService>();

        public ServiceKit14 Kit => _kit.Get(_DynCodeRoot.GetKit<ServiceKit14>);
        private readonly GetOnce<ServiceKit14> _kit = new();

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;

        #endregion


        #region Adam

        ///// <inheritdoc cref="IDynamicCode.AsAdam" />
        //[NonAction]
        //public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot?.AsAdam(item, fieldName);

        /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
        [NonAction]
        public IFile SaveInAdam(string noParamOrder = Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
            => CtxHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

        #endregion


        #region New App, Settings, Resources

        /// <inheritdoc />
        public IAppTyped App => (IAppTyped)_DynCodeRoot?.App;

        /// <inheritdoc cref="IDynamicCode16.AllResources" />
        public ITypedStack AllResources => _DynCodeRoot.Resources;

        /// <inheritdoc cref="IDynamicCode16.AllSettings" />
        public ITypedStack AllSettings => _DynCodeRoot.Settings;

        #endregion

        #region My... Stuff

        private TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper() => new TypedCode16Helper(_DynCodeRoot, MyData, null, false, "c# WebApiController");

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        public IContextData MyData => _DynCodeRoot.Data;

        #endregion


        #region As Conversions

        /// <inheritdoc cref="IDynamicCode16.AsItem" />
        public ITypedItem AsItem(object target, string noParamOrder = Protector) => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc cref="IDynamicCode16.AsItems" />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Protector) => _DynCodeRoot.AsC.AsItems(list);

        /// <inheritdoc cref="IDynamicCode16.AsEntity" />
        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.AsC.AsEntity(thing);

        /// <inheritdoc cref="IDynamicCode16.AsTyped" />
        public ITyped AsTyped(object original) => _DynCodeRoot.AsC.AsTyped(original);

        /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
        public IEnumerable<ITyped> AsTypedList(object list) => _DynCodeRoot.AsC.AsTypedList(list);

        /// <inheritdoc cref="IDynamicCode16.AsStack" />
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.AsStack(items);

        #endregion

        public ITypedModel MyModel => CodeHelper.MyModel;

        /// <inheritdoc cref="IDynamicCode16.GetCode"/>
        public dynamic GetCode(string path) => _DynCodeRoot.CreateInstance(path, relativePath: (this as IGetCodePath).CreateInstancePath);

        #region MyContext

        /// <inheritdoc cref="IDynamicCode16.MyContext" />
        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc cref="IDynamicCode16.MyUser" />
        public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

        /// <inheritdoc cref="IDynamicCode16.MyPage" />
        public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

        /// <inheritdoc cref="IDynamicCode16.MyView" />
        public ICmsView MyView => _DynCodeRoot.CmsContext.View;

        #endregion

        #region  CreateInstance implementation

        string IGetCodePath.CreateInstancePath { get; set; }

        ///// <inheritdoc cref="ICreateInstance.CreateInstance"/>
        //[NonAction]
        //public dynamic CreateInstance(string virtualPath, string noParamOrder = Protector, string name = null, string relativePath = null, bool throwOnError = true)
        //    => _DynCodeRoot.CreateInstance(virtualPath, noParamOrder, name, (this as IGetCodePath).CreateInstancePath, throwOnError);

        #endregion


        #region File Response / Download

        /// <inheritdoc cref="IDynamicWebApi.File"/>
        public dynamic File(string noParamOrder = Protector,
            bool? download = null,
            string virtualPath = null,
            string contentType = null,
            string fileDownloadName = null,
            object contents = null
        ) =>
            new OqtWebApiShim(response: Response, this).File(noParamOrder, download, virtualPath, contentType, fileDownloadName, contents);

        #endregion

    }
}