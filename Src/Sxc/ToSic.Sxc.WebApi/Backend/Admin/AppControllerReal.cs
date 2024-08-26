using System.IO;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.DataSources.Sys.Internal;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Languages;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Backend.AppStack;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Services;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
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
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
    : ServiceBase($"{Eav.EavLogs.WebApi}.{LogSuffix}Rl",
        connect:
        [
            appsBackendLazy, workAppsRemove, exportAppLazy, importAppLazy, appBuilderLazy, resetAppLazy,
            systemManagerLazy, languagesBackendLazy, appReadersLazy, appStackBackendLazy, json, globalConfiguration
        ])
{
    public const string LogSuffix = "AppCon";
    private const string TemplatesJson = "templates.json";


    public List<AppDto> List(int zoneId) => appsBackendLazy.Value.Apps();

    public List<AppDto> InheritableApps() => appsBackendLazy.Value.GetInheritableApps();

    public void App(int zoneId, int appId, bool fullDelete = true)
        => workAppsRemove.Value.RemoveAppInSiteAndEav(zoneId, appId, fullDelete);

    public void App(int zoneId, string name, int? inheritAppId = null, int templateId = 0)
    {
        var l = Log.Fn($"{nameof(zoneId)}:{zoneId}, {nameof(name)}:{name}, {nameof(inheritAppId)}:{inheritAppId}, {nameof(templateId)}:{templateId}");

        if (templateId == 0)
        {
            l.A("create default new app without template");
            appBuilderLazy.Value.Init(zoneId).Create(name, null, inheritAppId);
            l.Done("ok");
            // Exit here, because we created the app without template
            return;
        }

        l.A($"find template app zip path for {nameof(templateId)}:{templateId}");
        var zipPath = GetTemplateZipPathOrThrow(templateId);

        l.A($"create new app from template zip:{zipPath}");
        var resultDto = importAppLazy.Value.Import(zipPath, zoneId, name, inheritAppId);
        if (!resultDto.Success)
            throw l.Ex(new Exception($"Error importing app from template: {string.Join(", ", resultDto.Messages.Select(m => $"{m.MessageType}:{m.Text}"))}"));

        l.Done("ok");
    }

    private string GetTemplateZipPathOrThrow(int templateId)
    {
        var l = Log.Fn<string>($"{nameof(templateId)}:{templateId}");

        var templatesJsonPath = Path.Combine(globalConfiguration.NewAppsTemplateFolder, TemplatesJson);

        if (!File.Exists(templatesJsonPath))
            throw l.Ex(new FileNotFoundException($"{TemplatesJson} file not found"));

        var templatesJson = File.ReadAllText(templatesJsonPath);
        var templates = json.Value.To<List<TemplateJson>>(templatesJson);
        var template = templates.FirstOrDefault(t => t.Id == templateId) 
            ?? throw l.Ex(new Exception($"Template with id {templateId} not found in {TemplatesJson}"));

        var zipPath = Path.Combine(globalConfiguration.NewAppsTemplateFolder, template.Zip);
        if (!File.Exists(zipPath))
            throw l.Ex(new FileNotFoundException($"Template {Path.GetFileName(zipPath)} not found"));

        return l.ReturnAsOk(zipPath);
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

    public List<AppStackDataRaw> GetStack(int appId, string part, string key = null, Guid? view = null)
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