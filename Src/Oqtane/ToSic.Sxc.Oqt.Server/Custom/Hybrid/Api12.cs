using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Code;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using IApp = ToSic.Sxc.Apps.IApp;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Custom base controller class for custom dynamic 2sxc app api controllers.
    /// It is without dependencies in class constructor, commonly provided with DI.
    /// </summary>
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract partial class Api12 : OqtStatefulControllerBase<DummyControllerReal>, IDynamicWebApi, IDynamicCode12, ToSic.Eav.Logging.IHasLog
    {
        protected Api12() : base(EavWebApiConstants.HistoryNameWebApi)
        {
            var log = base.Log.SubLogOrNull("Hyb12.Api12"); // real log
            _log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        protected Api12(string logSuffix) : base(logSuffix)
        {
            var log = base.Log.SubLogOrNull($"{logSuffix}.Api12"); // real log
            _log = new LogAdapter(log); // Eav.Logging.ILog compatibility
        }

        /// <summary>
        /// Our custom dynamic 2sxc app api controllers, depends on event OnActionExecuting to provide dependencies (without DI in constructor).
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Use the ServiceProvider of the current request to build DynamicCodeRoot
            // Note that BlockOptional was already retrieved in the base class
            _DynCodeRoot = context.HttpContext.RequestServices.Build<DynamicCodeRoot>().InitDynCodeRoot(BlockOptional, Log, ToSic.Sxc.Constants.CompatibilityLevel12);

            _AdamCode = GetService<AdamCode>().Init(_DynCodeRoot, Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (_DynCodeRoot.App == null)
            {
                Log.A("DynCode.App is null");
                TryToAttachAppFromUrlParams(context);
            }

            // Ensure the Api knows what path it's on, in case it will
            // create instances of .cs files
            if (context.HttpContext.Items.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var createInstancePath))
                CreateInstancePath = createInstancePath as string;
        }

        private void TryToAttachAppFromUrlParams(ActionExecutingContext context)
        {
            var wrapLog = Log.Fn();
            var found = false;
            try
            {
                // Handed in from the App-API Transformer
                context.HttpContext.Items.TryGetValue(AppApiDynamicRouteValueTransformer.HttpContextKeyForAppFolder, out var routeAppPathObj);
                if (routeAppPathObj == null) return;
                var routeAppPath = routeAppPathObj.ToString();
                
                var appId = CtxResolver.AppOrNull(routeAppPath)?.AppState.AppId ?? ToSic.Eav.Constants.NullId;

                if (appId != ToSic.Eav.Constants.NullId)
                {
                    // Look up if page publishing is enabled - if module context is not available, always false
                    Log.A($"AppId: {appId}");
                    var app = LoadAppOnly(appId, CtxResolver.Site().Site);
                    _DynCodeRoot.AttachApp(app);
                    found = true;
                }
            }
            catch { /* ignore */ }

            wrapLog.Done(found.ToString());
        }

        /// <summary>
        /// Only load the app in case we don't have a module context
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private IApp LoadAppOnly(int appId, ISite site)
        {
            var wrapLog = Log.Fn<IApp>($"{appId}");
            var showDrafts = false;
            var app = GetService<ToSic.Sxc.Apps.App>();
            app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(AppConstants.AutoLookupZone, appId),
                GetService<AppConfigDelegate>().Init(Log).Build(showDrafts),
                Log);
            return wrapLog.Return(appStuff);
        }

        #region IHasLog

        /// <inheritdoc />
        public new ToSic.Eav.Logging.ILog Log => _log ??= new LogAdapter(null);

        private ToSic.Eav.Logging.ILog _log;

        //ToSic.Lib.Logging.ILog ToSic.Lib.Logging.IHasLog.Log => Log.GetContents(); // explicit Log implementation (to ensure that new IHasLog.Log interface is implemented)

        #endregion
    }
}
