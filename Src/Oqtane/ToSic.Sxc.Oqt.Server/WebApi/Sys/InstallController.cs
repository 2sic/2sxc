using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Net;
using ToSic.Eav.WebApi.Sys.Install;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using RealController = ToSic.Sxc.Backend.Sys.InstallControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathAndLang + "/" + AreaRoutes.Sys)]

[ShowApiWhenReleased(ShowApiMode.Never)]
public class InstallController()
    : OqtControllerBase(false, RealController.LogSuffix), IInstallController
{
    /// <inheritdoc />
    public override ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => GetService<RealController>();


    /// <summary>
    /// Make sure that these requests don't land in the normal api-log.
    /// Otherwise each log-access would re-number what item we're looking at
    /// </summary>
    protected override string HistoryLogGroup { get; } = "web-api.install";


    /// <inheritdoc />
    [HttpGet]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    [Authorize(Roles = RoleNames.Host)]
    public bool Resume() => Real.Resume();


    /// <inheritdoc />
    [HttpPost]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult RemotePackage(string packageUrl, string newName = null)
    {
        HotReloadEnabledCheck.Check(); // Ensure that Hot Reload is not enabled or try to disable it.

        var block = this.PrepareRequestBlock().BlockOrNull;
        var result = Real.RemotePackage(packageUrl, block?.Context.Module, newName);
        return result == null
            ? Ok()
            : Problem(result, null, (int)HttpStatusCode.InternalServerError);
    }
}
