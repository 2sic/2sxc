using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Integration;
using RealController = ToSic.Eav.WebApi.Sys.LogControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    public class LogController: OqtStatefulControllerBase, ILogController
    {
        public LogController() : base(RealController.LogSuffix) { }

        private RealController Real => GetService<RealController>();



        /// <inheritdoc />
        [HttpGet]
        public string EnableDebug(int duration = 1) => Real.EnableDebug(OqtLogging.ActivateForDuration, duration);
    }
}
