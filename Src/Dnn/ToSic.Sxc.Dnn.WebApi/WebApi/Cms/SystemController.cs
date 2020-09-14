using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Security;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public partial class SystemController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.SysCnt";

        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic DialogSettings(int appId)
        {
            return new AdminBackend().Init(Log).DialogSettings(
                GetContext(),
                new DnnContextBuilder(
                    PortalSettings.Current,
                    Request.FindModuleInfo(), UserInfo),
                appId);

            //IApp app = null;
            //// if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            //if (appId != 0 && appId != Eav.Constants.AppIdEmpty)
            //{
            //    var appAndPerms = new MultiPermissionsApp().Init(GetContext(), GetApp(appId), Log);
            //    if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
            //        throw HttpException.PermissionDenied(error);
            //    app = appAndPerms.App;
            //}

            //var psCurrent = PortalSettings.Current;
            //var cb = new DnnContextBuilder(psCurrent,
            //        Request.FindModuleInfo(),
            //        UserInfo)
            //    .InitApp(app?.ZoneId, app);

            //return new
            //{
            //    // TODO: Deprecate PARAMS these properties as soon as old UI is gone
            //    //IsContent = app?.AppGuid == "Default",
            //    //Language = psCurrent.CultureCode,
            //    //LanguageDefault = psCurrent.DefaultLanguage,
            //    //AppPath = app?.Path,
            //    //GettingStartedUrl = cb.GettingStartedUrl(),
            //    // END TODO
            //    Context = cb.Get(Ctx.All),
            //};
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