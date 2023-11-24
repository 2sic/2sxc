using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Web.Http;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using RealController = ToSic.Eav.WebApi.Sys.LogControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class LogController : DnnApiControllerWithFixes, ILogController
    {
        public LogController() : base(RealController.LogSuffix) { }

        private RealController Real => SysHlp.GetService<RealController>();

        /// <inheritdoc />
        [HttpGet]
        public string EnableDebug(int duration = 1) => Real.EnableDebug(DnnLogging.ActivateForDuration, duration);
    }
}