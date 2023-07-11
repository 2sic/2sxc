#if NETCOREAPP
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Apps;
using ToSic.Eav.Code;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.WebApi.Adam;
using IApp = ToSic.Sxc.Apps.IApp;
using IContextResolver = ToSic.Sxc.Context.IContextResolver;

namespace ToSic.Sxc.WebApi.Infrastructure
{
    internal class NetCoreWebApiContextHelper: CodeHelperBase
    {
        private readonly ControllerBase _owner;
        private readonly ICanGetService _helper;

        public NetCoreWebApiContextHelper(ControllerBase owner, ICanGetService helper) : base("Oqt.ApiHlp")
        {
            if (owner is IHasLog ownerWithLog)
                this.LinkLog(ownerWithLog.Log);
            _owner = owner;
            _helper = helper;
        }

        #region Initialize

        /// <summary>
        /// This will make sure that any services requiring the context can get it.
        /// It must usually be called from the base class which expects to use this.
        /// </summary>
        public void InitializeBlockContext(ActionExecutingContext context)
        {
            if (_blockContextInitialized) return;
            _blockContextInitialized = true;
            var getBlock = _helper.GetService<IWebApiContextBuilder>();
            CtxResolver = getBlock.PrepareContextResolverForApiRequest();
            BlockOptional = CtxResolver.BlockOrNull();
        }

        private bool _blockContextInitialized;

        private IContextResolver CtxResolver { get; set; }
        internal IBlock BlockOptional { get; private set; }

        public void OnActionExecutingEnd(ActionExecutingContext context)
        {
            // base.OnActionExecuting(context);
            InitializeBlockContext(context);

            // Use the ServiceProvider of the current request to build DynamicCodeRoot
            // Note that BlockOptional was already retrieved in the base class
            var codeRoot = context.HttpContext.RequestServices
                .Build<DynamicCodeRoot>()
                .InitDynCodeRoot(BlockOptional, Log, (_owner as ICompatibilityLevel)?.CompatibilityLevel ?? Constants.CompatibilityLevel12);
            ConnectToRoot(codeRoot);

            AdamCode = _helper.GetService<AdamCode>();
            AdamCode.ConnectToRoot(_DynCodeRoot, Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (_DynCodeRoot.App == null)
            {
                Log.A("DynCode.App is null");
                TryToAttachAppFromUrlParams(context);
            }

            // Ensure the Api knows what path it's on, in case it will
            // create instances of .cs files
            if (context.HttpContext.Items.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var createInstancePath))
                if (_owner is IGetCodePath withCodePath)
                    withCodePath.CreateInstancePath = createInstancePath as string;
        }


        private void TryToAttachAppFromUrlParams(ActionExecutingContext context) => base.Log.Do(() =>
        {
            var found = false;
            try
            {
                // Handed in from the App-API Transformer
                context.HttpContext.Items.TryGetValue(SxcWebApiConstants.HttpContextKeyForAppFolder,
                    out var routeAppPathObj);
                if (routeAppPathObj == null) return "";
                var routeAppPath = routeAppPathObj.ToString();

                var appId = CtxResolver.SetAppOrNull(routeAppPath)?.AppState.AppId ?? Eav.Constants.NullId;

                if (appId != Eav.Constants.NullId)
                {
                    // Look up if page publishing is enabled - if module context is not available, always false
                    base.Log.A($"AppId: {appId}");
                    var app = LoadAppOnly(appId, CtxResolver.Site().Site);
                    _DynCodeRoot.AttachApp(app);
                    found = true;
                }
            }
            catch
            {
                /* ignore */
            }

            return found.ToString();
        });

        /// <summary>
        /// Only load the app in case we don't have a module context
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private IApp LoadAppOnly(int appId, ISite site) => base.Log.Func($"{appId}", () =>
        {
            var app = _helper.GetService<Apps.App>();
            app.PreInit(site);
            return app.Init(new AppIdentity(AppConstants.AutoLookupZone, appId), _helper.GetService<AppConfigDelegate>().Build());
        });


        #endregion

        #region Context Maker

        public void SetupResponseMaker() => _helper.GetService<IResponseMaker>().Init(_owner);

        #endregion

        #region Adam

        public AdamCode AdamCode { get; private set; }

        public Sxc.Adam.IFile SaveInAdam(string noParamOrder = Eav.Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "") =>
            AdamCode.SaveInAdam(
                stream: stream,
                fileName: fileName,
                contentType: contentType,
                guid: guid,
                field: field,
                subFolder: subFolder);

        #endregion

    }
}
#endif