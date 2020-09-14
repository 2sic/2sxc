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
        public dynamic DialogSettings(int appId) =>
            new AdminBackend().Init(Log).DialogSettings(
                GetContext(),
                new DnnContextBuilder(
                    PortalSettings.Current,
                    Request.FindModuleInfo(), UserInfo),
                appId);

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