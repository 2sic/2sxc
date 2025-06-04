using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Zone;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Eav.WebApi.Sys.Admin.ZoneControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
[ValidateAntiForgeryToken]
[Authorize(Roles = RoleNames.Admin)]
// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ZoneController() : OqtStatefulControllerBase(RealController.LogSuffix), IZoneController
{
    private RealController Real => GetService<RealController>();


    /// <inheritdoc />
    [HttpGet]
    public IList<SiteLanguageDto> GetLanguages() => Real.GetLanguages();

    /// <inheritdoc />
    [HttpGet]
    public void SwitchLanguage(string cultureCode, bool enable) => Real.SwitchLanguage(cultureCode, enable);

    /// <inheritdoc />
    [HttpGet]
    public SystemInfoSetDto GetSystemInfo() => Real.GetSystemInfo();

}