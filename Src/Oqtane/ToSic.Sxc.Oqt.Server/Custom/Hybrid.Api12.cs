using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
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
    public abstract partial class Api12 : OqtStatefulControllerBase, IDynamicCode, IDynamicWebApi, IDynamicCode12
    {
        [PrivateApi]
        protected override string HistoryLogName => EavWebApiConstants.HistoryNameWebApi;

        /// <summary>
        /// Our custom dynamic 2sxc app api controllers, depends on event OnActionExecuting to provide dependencies (without DI in constructor).
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Note that BlockOptional was already retrieved in the base class
            _DynCodeRoot = ServiceProvider.Build<DynamicCodeRoot>().Init(BlockOptional, Log, ToSic.Sxc.Constants.CompatibilityLevel12);
            _AdamCode = ServiceProvider.Build<AdamCode>().Init(_DynCodeRoot, Log);

            // In case SxcBlock was null, there is no instance, but we may still need the app
            if (_DynCodeRoot.App == null)
            {
                Log.Add("DynCode.App is null");
                TryToAttachAppFromUrlParams(context);
            }

            // Ensure the Api knows what path it's on, in case it will
            // create instances of .cs files
            if (context.HttpContext.Items.TryGetValue(CodeCompiler.SharedCodeRootPathKeyInCache, out var createInstancePath))
                CreateInstancePath = createInstancePath as string;
        }

        private void TryToAttachAppFromUrlParams(ActionExecutingContext context)
        {
            var wrapLog = Log.Call();
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
                    Log.Add($"AppId: {appId}");
                    var app = LoadAppOnly(appId, CtxResolver.Site().Site);
                    _DynCodeRoot.AttachAppAndInitLink(app);
                    found = true;
                }
            }
            catch { /* ignore */ }

            wrapLog(found.ToString());
        }

        /// <summary>
        /// Only load the app in case we don't have a module context
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private IApp LoadAppOnly(int appId, ISite site)
        {
            var wrapLog = Log.Call<IApp>($"{appId}");
            var zoneId = ToSic.Eav.Apps.App.AutoLookupZone;
            var showDrafts = false;
            var app = ServiceProvider.Build<ToSic.Sxc.Apps.App>();
            app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(zoneId, appId),
                ServiceProvider.Build<AppConfigDelegate>().Init(Log).Build(showDrafts),
                Log);
            return wrapLog(null, appStuff);
        }
    }
}
