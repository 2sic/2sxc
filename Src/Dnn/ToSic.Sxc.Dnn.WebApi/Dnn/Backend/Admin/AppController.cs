using System.Web;
using ToSic.Eav.DataSources.Sys;
using ToSic.Eav.WebApi.Sys.Admin;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Dnn.WebApi.Sys;
using AppDto = ToSic.Eav.WebApi.Sys.Dto.AppDto;
using RealController = ToSic.Sxc.Backend.Admin.AppControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
// token, which fails in the cases where the url is called using get, which should result in a download
// [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
// we can't set this globally (only needed for imports)
[DnnLogExceptions]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppController() : DnnSxcControllerBase(RealController.LogSuffix), IAppController<HttpResponseMessage>
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public ICollection<AppDto> List(int zoneId)
        => Real.List(zoneId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public ICollection<AppDto> InheritableApps()
        => Real.InheritableApps();

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public void App(int zoneId, int appId, bool fullDelete = true)
    {
        SysHlp.PreventServerTimeout300();
        Real.App(zoneId, appId, fullDelete);
    }

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public void App(int zoneId, string name, int? inheritAppId = null) => Real.App(zoneId, name, inheritAppId);

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public ICollection<SiteLanguageDto> Languages(int appId) => Real.Languages(appId);

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public AppExportInfoDto Statistics(int zoneId, int appId) => Real.Statistics(zoneId, appId);


    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool FlushCache(int zoneId, int appId) => Real.FlushCache(zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    public HttpResponseMessage Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool assetsAdam, bool assetsSite, bool assetAdamDeleted = true)
        => Real.Export(new(zoneId, appId, includeContentGroups, resetAppGuid, assetsAdam, assetsSite, assetAdamDeleted));

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    public bool SaveData(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool withPortalFiles = false)
        => Real.SaveData(new(zoneId, appId, includeContentGroups, resetAppGuid, WithSiteFiles: withPortalFiles));

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public List<AppStackDataRaw> GetStack(int appId, string part, string key = null, Guid? view = null) 
        => Real.GetStack(appId, part, key, view);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Reset(int zoneId, int appId, bool withPortalFiles = false)
    {
        SysHlp.PreventServerTimeout300();
        return Real.Reset(zoneId, appId, PortalSettings.DefaultLanguage, withPortalFiles);
    }

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Import(int zoneId)
    {
        SysHlp.PreventServerTimeout300();
        return Real.Import(new(Request, HttpContext.Current.Request), zoneId, HttpContext.Current.Request["Name"]);
    }

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<PendingAppDto> GetPendingApps(int zoneId) 
        => Real.GetPendingApps(zoneId);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto InstallPendingApps(int zoneId, IEnumerable<PendingAppDto> pendingApps)
    {
        SysHlp.PreventServerTimeout300();
        return Real.InstallPendingApps(zoneId, pendingApps);
    }
}