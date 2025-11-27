using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps.Sys.FileSystemState;
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
public class AppExtensionsController() : OqtStatefulControllerBase(RealController.LogSuffix), IAppExtensionsController<IActionResult>
{
    private RealController Real => GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ExtensionsResultDto Extensions(int appId)
        => Real.Extensions(appId);

    /// <inheritdoc />
    /// Original update/create endpoint using PUT with name segment.
    [HttpPut("{name}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Extension(int zoneId, int appId, string name, [FromBody] ExtensionManifest configuration)
        => Real.Extension(zoneId, appId, name, configuration);

    /// <summary>
    /// Alias POST endpoint for front-ends posting to /appExtensions/extensions with query parameters.
    /// Matches DNN plural POST behavior to avoid 405 errors if client uses POST instead of PUT.
    /// </summary>
    [HttpPost("extensions")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool ExtensionsPostAlias(int zoneId, int appId, string name, [FromBody] ExtensionManifest configuration)
        => Real.Extension(zoneId, appId, name, configuration);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Install(int zoneId, int appId, bool overwrite = true)
    {
        HotReloadEnabledCheck.Check();
        return Real.Install(new(Request), zoneId, appId, overwrite);
    }

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public IActionResult Download(int zoneId, int appId, string name)
        => Real.Download(zoneId, appId, name);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public ExtensionInspectResultDto Inspect(int appId, string name, string edition = null)
        => Real.Inspect(appId, name, edition);

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Delete(int appId, string name, string edition = null, bool force = false, bool withData = false)
        => Real.Delete(appId, name, edition, force, withData);
}
