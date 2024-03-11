using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using RealController = ToSic.Sxc.Backend.Sys.InstallControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathAndLang + "/" + AreaRoutes.Sys)]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class InstallController()
    : OqtStatefulControllerBase(RealController.LogSuffix), IInstallController<IActionResult>
{
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
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public InstallAppsDto InstallSettings(bool isContentApp)
        => Real.InstallSettings(isContentApp, CtxHlp.BlockOptional.Context.Module);


    /// <inheritdoc />
    [HttpPost]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult RemotePackage(string packageUrl)
    {
        HotReloadEnabledCheck.Check(); // Ensure that Hot Reload is not enabled or try to disable it.
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.RemotePackage(packageUrl, CtxHlp.BlockOptional?.Context.Module);
    }
}