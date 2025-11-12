using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Text.Json;
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
    [HttpPut("{name}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Extensions(int zoneId, int appId, string name, [FromBody] JsonElement configuration)
        => Real.Extensions(zoneId, appId, name, configuration);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool InstallExtension([FromQuery] int zoneId, [FromQuery] int appId, [FromQuery] bool overwrite = true)
    {
        // Ensure that Hot Reload is not enabled or try to disable it.
        HotReloadEnabledCheck.Check();
        return Real.InstallExtensionZip(new(Request), zoneId, appId, overwrite);
    }

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public IActionResult Download([FromQuery] int zoneId, [FromQuery] int appId, [FromQuery] string name)
        => Real.Download(zoneId, appId, name);
}
