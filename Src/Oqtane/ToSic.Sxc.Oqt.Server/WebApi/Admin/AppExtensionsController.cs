using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps.Sys.Extensions;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Apps.Extensions.Sys.Preflight;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Sxc.Backend.Admin.AppExtensionsControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

// Release routes - same base routes as AppController
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
public class AppExtensionsController : ControllerBase, IHasLog, IAppExtensionsController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public PreflightResultDto InstallPreflight(int appId, [FromQuery] string editions = "")
    {
        HotReloadEnabledCheck.Check();
        return Real.InstallPreflight(new(Request), appId, editions);
    }

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public PreflightResultDto InstallPreflightFrom(int appId, [FromBody] string[] urls, [FromQuery] string editions = "")
    {
        HotReloadEnabledCheck.Check();
        return Real.InstallPreflightFrom(urls, appId, editions);
    }

    /// <inheritdoc />
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Install(int zoneId, int appId, [FromQuery] string editions = "", bool overwrite = false)
    {
        HotReloadEnabledCheck.Check();
        return Real.Install(new(Request), zoneId, appId, editions, overwrite);
    }

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool InstallFrom(int zoneId, int appId, [FromBody] string[] urls, [FromQuery] string editions = "", bool overwrite = false)
    {
        HotReloadEnabledCheck.Check();
        return Real.InstallFrom(urls, zoneId, appId, editions, overwrite);
    }


    /// <inheritdoc />
    //[HttpPut("{name}")]
    [HttpPost("{name}")]
    //[HttpPost("extensions")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Configuration(int appId, string name, [FromBody] ExtensionManifest configuration)
        => Real.Configuration(appId, name, configuration);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public IActionResult Download(int appId, string name)
        => Real.Download(appId, name).ToHttpResponse();

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Delete(int appId, string name, string edition = null, bool force = false, bool withData = false)
        => Real.Delete(appId, name, edition, force, withData);
}
