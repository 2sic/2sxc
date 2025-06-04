using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Logs;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Integration;
using RealController = ToSic.Eav.WebApi.Sys.Logs.LogControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathAndLang + "/" + AreaRoutes.Sys)]

// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[Authorize(Roles = RoleNames.Admin)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LogController() : OqtStatefulControllerBase(RealController.LogSuffix), ILogController
{
    private RealController Real => GetService<RealController>();



    /// <inheritdoc />
    [HttpGet]
    public string EnableDebug(int duration = 1) => Real.EnableDebug(OqtLogging.ActivateForDuration, duration);
}