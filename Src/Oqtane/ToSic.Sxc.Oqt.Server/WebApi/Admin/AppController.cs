using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Text.Json;
using ToSic.Eav.DataSources.Sys;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.WebApi;
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

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppController() : OqtStatefulControllerBase(RealController.LogSuffix), IAppController<IActionResult>
{
    private RealController Real => GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ICollection<AppDto> List(int zoneId)
        => Real.List(zoneId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Host)]
    public ICollection<AppDto> InheritableApps()
        => Real.InheritableApps();

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public void App(int zoneId, int appId, bool fullDelete = true)
        => Real.App(zoneId, appId, fullDelete);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public void App(int zoneId, string name, int? inheritAppId = null) => Real.App(zoneId, name, inheritAppId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ICollection<SiteLanguageDto> Languages(int appId)
        => Real.Languages(appId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public AppExportInfoDto Statistics(int zoneId, int appId)
        => Real.Statistics(zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool FlushCache(int zoneId, int appId)
        => Real.FlushCache(zoneId, appId);

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

    // New: install extension ZIP (multipart/form-data)
    [HttpPost("extensions/upload")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool ExtensionsInstall([FromQuery] int zoneId, [FromQuery] int appId, [FromQuery] string? folder = null, [FromQuery] bool overwrite = false)
    {
        // Ensure that Hot Reload is not enabled or try to disable it.
        HotReloadEnabledCheck.Check();
        return Real.InstallExtensionZip(new(Request), zoneId, appId, folder, overwrite);
    }
}
