using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using RealController = ToSic.Sxc.Backend.Admin.AppExtensionsControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

// Release routes - same base routes as AppController
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppExtensionsController() : OqtStatefulControllerBase(RealController.LogSuffix), IAppExtensionsController
{
    private RealController Real => GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ExtensionsResultDto Extensions(int appId)
        => Real.Extensions(appId);

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
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public ExtensionInspectResultDto Inspect(int appId, string name, string edition = null)
        => Real.Inspect(appId, name, edition);

    /// <inheritdoc />
    //[HttpPut("{name}")]
    [HttpPost("{name}")]
    //[HttpPost("extensions")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Configuration(int appId, string name, [FromBody] ExtensionManifest configuration)
        => Real.Configuration(appId, name, configuration);

    ///// <summary>
    ///// Alias POST endpoint for front-ends posting to /appExtensions/extensions with query parameters.
    ///// Matches DNN plural POST behavior to avoid 405 errors if client uses POST instead of PUT.
    ///// </summary>
    //[HttpPost("extensions")]
    //[ValidateAntiForgeryToken]
    //[Authorize(Roles = RoleNames.Admin)]
    //public bool ExtensionsPostAlias(int appId, string name, [FromBody] ExtensionManifest configuration)
    //    => Real.Extension(appId, name, configuration);

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
