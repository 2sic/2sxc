using System.Collections.Generic;
using System;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
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
    public abstract class ApiTyped : OqtStatefulControllerBase, IDynamicWebApi, IHasCodeLog, IHasDynamicCodeRoot, IDynamicCode16
    {
        #region setup

        protected ApiTyped() : this(EavWebApiConstants.HistoryNameWebApi) { }

        protected ApiTyped(string logSuffix) : base(logSuffix) { }

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

        /// <inheritdoc cref="IDynamicCode16.Kit"/>
        public ServiceKit16 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit16>());
        private readonly GetOnce<ServiceKit16> _kit = new GetOnce<ServiceKit16>();

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => CodeHelper.DevTools;

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;

        #endregion


        #region Adam

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
        public ITypedStack AllResources => CodeHelper.AllResources;

        /// <inheritdoc cref="IDynamicCode16.AllSettings" />
        public ITypedStack AllSettings => CodeHelper.AllSettings;

        #endregion

        #region My... Stuff

        private TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper() => new(_DynCodeRoot, MyData, null, false, "c# WebApiController");

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        public IContextData MyData => _DynCodeRoot.Data;

        #endregion


        #region As Conversions

        /// <inheritdoc cref="IDynamicCode16.AsItem" />
        public ITypedItem AsItem(object data, string noParamOrder = Protector, bool? propsRequired = default, bool? mock = default)
            => _DynCodeRoot.Cdf.AsItem(data, noParamOrder, propsRequired: propsRequired ?? true, mock: mock);

        /// <inheritdoc cref="IDynamicCode16.AsItems" />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsItems(list, noParamOrder, propsRequired: propsRequired ?? true);

        /// <inheritdoc cref="IDynamicCode16.AsEntity" />
        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.Cdf.AsEntity(thing);

        /// <inheritdoc cref="IDynamicCode16.AsTyped" />
        public ITyped AsTyped(object original, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsTyped(original, propsRequired: propsRequired);

        /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
        public IEnumerable<ITyped> AsTypedList(object list, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

        /// <inheritdoc cref="IDynamicCode16.AsStack" />
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.Cdf.AsStack(items);

        #endregion

        public ITypedModel MyModel => CodeHelper.MyModel;

        /// <inheritdoc cref="IDynamicCode16.GetCode"/>
        public dynamic GetCode(string path, string noParamOrder = Protector, string className = default)
        {
            Protect(noParamOrder, nameof(className));
            return _DynCodeRoot.CreateInstance(path, relativePath: (this as IGetCodePath).CreateInstancePath, name: className);
        }

        #region MyContext & UniqueKey

        /// <inheritdoc cref="IDynamicCode16.MyContext" />
        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc cref="IDynamicCode16.MyPage" />
        public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

        /// <inheritdoc cref="IDynamicCode16.MyUser" />
        public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

        /// <inheritdoc cref="IDynamicCode16.MyView" />
        public ICmsView MyView => _DynCodeRoot.CmsContext.View;

        /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
        public string UniqueKey => Kit.Key.UniqueKey;

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