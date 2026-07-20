using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Eav.WebApi.Sys.Admin.AppInternalsControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// Proxy Class to the AppInternalsController (Web API Controller)
/// </summary>
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppInternalsController() : OqtStatefulControllerBase(RealController.LogSuffix)
{
    private RealController Real => GetService<RealController>();

    // Replaced by DataSource System.AppEnhancements through query System.SysData.
    // Use app/auto/query/System.SysData/ with SysDataSource=System.AppEnhancements.
    ///// <inheritdoc/>
    //[HttpGet]
    //[ValidateAntiForgeryToken]
    //[Authorize(Roles = RoleNames.Admin)]
    //public AppInternalsDto Get(int appId)
    //    => Real.Get(appId);
}