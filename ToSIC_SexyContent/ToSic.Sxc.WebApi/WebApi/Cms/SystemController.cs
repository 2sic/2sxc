using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Security;
using ToSic.Sxc.WebApi.Context;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public partial class SystemController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sSysC");
        }

        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic DialogSettings(int appId)
        {
            IApp app = null;
            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appId != 0)
            {
                var appAndPerms = new MultiPermissionsApp(BlockBuilder, appId, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var exp))
                    throw exp;
                app = appAndPerms.App;
            }

            var psCurrent = PortalSettings.Current;
            var cb = new ContextBuilder(psCurrent, 
                Request.FindModuleInfo(),
                UserInfo,
                app?.ZoneId,
                app);

            return new
            {
                // TODO: Deprecate PARAMS these properties as soon as old UI is gone
                IsContent = app?.AppGuid == "Default",
                Language = psCurrent.CultureCode,
                LanguageDefault = psCurrent.DefaultLanguage,
                AppPath = app?.Path,
                GettingStartedUrl = cb.GettingStartedUrl(),
                // END TODO
                Context = cb.Get(Ctx.All),
            };
        }

        #endregion

        #region Enable extended logging

        [HttpGet]
        public string ExtendedLogging(int duration = 1)
        {
            Log.Add("Extended logging will set for duration:" + duration);
            var msg = DnnLogging.ActivateForDuration(duration);
            Log.Add(msg);
            return msg;
        }

        #endregion
    }
}