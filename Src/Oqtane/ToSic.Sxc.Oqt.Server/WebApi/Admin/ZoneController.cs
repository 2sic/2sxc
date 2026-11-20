using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Eav.WebApi.Admin.Sys.ZoneControllerReal;

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

[OqtLogWebApi]
[OqtInitializeAlias]
[NoContentForEmptyAction]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ZoneController : ControllerBase, IHasLog, IZoneController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();


    /// <inheritdoc />
    [HttpGet]
    public void SwitchLanguage(string cultureCode, bool enable) => Real.SwitchLanguage(cultureCode, enable);

}