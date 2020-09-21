using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class SystemController : SxcApiControllerBase
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

    }
}