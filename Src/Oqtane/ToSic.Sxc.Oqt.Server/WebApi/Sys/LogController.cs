using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Integration;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    public class LogController: OqtStatefulControllerBase<LogControllerReal>, ILogController
    {
        public LogController() : base(LogControllerReal.LogSuffix) { }


        /// <inheritdoc />
        [HttpGet]
        public string EnableDebug(int duration = 1) => Real.EnableDebug(OqtLogging.ActivateForDuration, duration);
    }
}
