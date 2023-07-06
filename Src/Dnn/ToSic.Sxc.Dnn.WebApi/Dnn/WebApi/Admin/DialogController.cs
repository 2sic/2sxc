using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi.Admin;
using RealController = ToSic.Sxc.WebApi.Admin.DialogControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules(DnnSupportedModuleNames)]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class DialogController : DnnApiControllerWithFixes, IDialogController
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://go.2sxc.org/proxy-controllers

        public DialogController(): base(RealController.LogSuffix) { }

        private RealController Real => SysHlp.GetService<RealController>();

        [HttpGet]
        public DialogContextStandaloneDto Settings(int appId) => Real.Settings(appId);
    }
}