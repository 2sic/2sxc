using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.DataSources.Sys;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Eav.WebApi.Sys.Languages;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Backend.AppStack;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Services;
using ToSic.Sys.Configuration;
using Services_ServiceBase = ToSic.Sys.Services.ServiceBase;

#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

/// <summary>
/// Experimental new class
/// Goal is to reduce code in the Dnn and Oqtane controllers, which basically does the same thing, mostly DI work
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppControllerReal(
    LazySvc<AppsBackend> appsBackendLazy,
    LazySvc<WorkAppsRemove> workAppsRemove,
    LazySvc<ExportApp> exportAppLazy,
    LazySvc<ImportApp> importAppLazy,
    LazySvc<AppCreator> appBuilderLazy,
    LazySvc<ResetApp> resetAppLazy,
    LazySvc<AppCachePurger> systemManagerLazy,
    LazySvc<LanguagesBackend> languagesBackendLazy,
    LazySvc<IAppReaderFactory> appReadersLazy,
    LazySvc<AppStackBackend> appStackBackendLazy,
    LazySvc<IJsonService> json,
    IGlobalConfiguration globalConfiguration)
    : Services_ServiceBase($"{EavLogs.WebApi}.{LogSuffix}Rl",
        connect:
        [
            appsBackendLazy, workAppsRemove, exportAppLazy, importAppLazy, appBuilderLazy, resetAppLazy,
            systemManagerLazy, languagesBackendLazy, appReadersLazy, appStackBackendLazy, json, globalConfiguration
        ])
{
    public const string LogSuffix = "AppCon";

    public ICollection<AppDto> List(int zoneId)
        => appsBackendLazy.Value.Apps();

    public ICollection<AppDto> InheritableApps()
        => appsBackendLazy.Value.GetInheritableApps();

    public void App(int zoneId, int appId, bool fullDelete = true)
        => workAppsRemove.Value.RemoveAppInSiteAndEav(zoneId, appId, fullDelete);

    public void App(int zoneId, string name, int? inheritAppId = null)
    {
        var l = Log.Fn($"{nameof(zoneId)}:{zoneId}, {nameof(name)}:{name}, {nameof(inheritAppId)}:{inheritAppId}");
        l.A("create default new app without template");
        appBuilderLazy.Value.Init(zoneId).Create(name, null, inheritAppId);
        l.Done("ok");
    }

    public List<SiteLanguageDto> Languages(int appId)
        => languagesBackendLazy.Value.GetLanguagesOfApp(appReadersLazy.Value.Get(appId), true);

    public AppExportInfoDto Statistics(int zoneId, int appId) => exportAppLazy.Value.GetAppInfo(zoneId, appId);

    public bool FlushCache(int zoneId, int appId)
    {
        var l = Log.Fn<bool>($"{zoneId}, {appId}");
        systemManagerLazy.Value.Purge(zoneId, appId);
        return l.ReturnTrue("ok");
    }

    public THttpResponseType Export(AppExportSpecs specs)
        => exportAppLazy.Value.Export(specs) as THttpResponseType;

    public bool SaveData(AppExportSpecs specs)
        => exportAppLazy.Value.SaveDataForVersionControl(specs);

    public List<AppStackDataRaw> GetStack(int appId, string? part, string? key = null, Guid? view = null)
        => appStackBackendLazy.Value.GetAll(appId, part ?? AppStackConstants.RootNameSettings, key, view);

    public ImportResultDto Reset(int zoneId, int appId, string defaultLanguage, bool withPortalFiles)
        => resetAppLazy.Value.Reset(zoneId, appId, defaultLanguage, withPortalFiles);

    /// <summary>
    /// Import App from import zip.
    /// </summary>
    /// <param name="uploadInfo">file upload</param>
    /// <param name="zoneId">int</param>
    /// <param name="renameApp">optional new name for app, provide to rename the app</param>
    /// <returns></returns>
    public ImportResultDto Import(HttpUploadedFile uploadInfo, int zoneId, string renameApp)
    {
        var l = Log.Fn<ImportResultDto>();

        if (!uploadInfo.HasFiles())
            return l.Return(new(false, "no file uploaded"), "no file uploaded");

        var (_, stream) = uploadInfo.GetStream(0);
        if (stream == null!)
            throw new NullReferenceException("File Stream is null, upload canceled");

        var result = importAppLazy.Value.Import(stream, zoneId, renameApp);

        return l.ReturnAsOk(result);
    }

    /// <summary>
    /// List all app folders in the 2sxc which:
    /// - are not installed as apps yet
    /// - have a App_Data/app.xml
    /// </summary>
    /// <param name="zoneId"></param>
    /// <returns></returns>
    public IEnumerable<PendingAppDto> GetPendingApps(int zoneId)
    {
        var l = Log.Fn<IEnumerable<PendingAppDto>>();
        var result = importAppLazy.Value.GetPendingApps(zoneId);
        return l.ReturnAsOk(result);
    }

    /// <summary>
    /// Install pending apps
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="pendingApps"></param>
    /// <returns></returns>
    public ImportResultDto InstallPendingApps(int zoneId, IEnumerable<PendingAppDto> pendingApps)
    {
        var l = Log.Fn<ImportResultDto>();
        var result = importAppLazy.Value.InstallPendingApps(zoneId, pendingApps);
        return l.ReturnAsOk(result);
    }
}