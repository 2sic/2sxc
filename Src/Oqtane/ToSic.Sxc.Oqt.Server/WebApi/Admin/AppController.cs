using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.DataSources.Sys.Internal;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using RealController = ToSic.Sxc.Backend.Admin.AppControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;
// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
// token, which fails in the cases where the url is called using get, which should result in a download
// [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab)
// we can't set this globally (only needed for imports)

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppController() : OqtStatefulControllerBase(RealController.LogSuffix), IAppController<IActionResult>
{
    private RealController Real => GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public List<AppDto> List(int zoneId) => Real.List(zoneId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Host)]
    public List<AppDto> InheritableApps() => Real.InheritableApps();

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public void App(int zoneId, int appId, bool fullDelete = true) => Real.App(zoneId, appId, fullDelete);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public void App(int zoneId, string name, int? inheritAppId = null, int templateId = 0) => Real.App(zoneId, name, inheritAppId, templateId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public List<SiteLanguageDto> Languages(int appId) => Real.Languages(appId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public AppExportInfoDto Statistics(int zoneId, int appId) => Real.Statistics(zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool FlushCache(int zoneId, int appId) => Real.FlushCache(zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    public IActionResult Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool assetsAdam, bool assetsSite, bool assetAdamDeleted = true) 
        => Real.Export(new AppExportSpecs(zoneId, appId, includeContentGroups, resetAppGuid, assetsAdam, assetsSite, assetAdamDeleted));

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public bool SaveData(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool withPortalFiles = false)
        => Real.SaveData(new AppExportSpecs(zoneId, appId, includeContentGroups, resetAppGuid, WithSiteFiles: withPortalFiles));

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public List<AppStackDataRaw> GetStack(int appId, string part, string key = null, Guid? view = null)
        => Real.GetStack(appId, part, key, view);

    /// <inheritdoc />
    [HttpPost]
    [Authorize(Roles = RoleNames.Host)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Reset(int zoneId, int appId, bool withPortalFiles = false) 
        => Real.Reset(zoneId, appId, CtxHlp.BlockOptional.Context.Site.DefaultCultureCode, withPortalFiles);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResultDto Import(int zoneId)
    {
        // Ensure that Hot Reload is not enabled or try to disable it.
        HotReloadEnabledCheck.Check();
        return Real.Import(new(Request), zoneId, Request.Form["Name"]);
    }

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<PendingAppDto> GetPendingApps(int zoneId)
        => Real.GetPendingApps(zoneId);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResultDto InstallPendingApps(int zoneId, [FromBody] IEnumerable<PendingAppDto> pendingApps)
    {
        // Ensure that Hot Reload is not enabled or try to disable it.
        HotReloadEnabledCheck.Check();
        return Real.InstallPendingApps(zoneId, pendingApps);
    }
}