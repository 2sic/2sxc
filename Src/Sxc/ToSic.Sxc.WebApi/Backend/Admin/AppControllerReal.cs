using Microsoft.EntityFrameworkCore.Internal;
using System.IO;
using System.Xml.Linq;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.DataSources.Sys.Internal;
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
public class AppControllerReal : ServiceBase
{
    private readonly LazySvc<IJsonService> _json;
    private readonly IGlobalConfiguration _globalConfiguration;
    public const string LogSuffix = "AppCon";
    private const string TemplatesJson = "templates.json";

    public AppControllerReal(
        LazySvc<AppsBackend> appsBackendLazy,
        LazySvc<WorkAppsRemove> workAppsRemove,
        LazySvc<ExportApp> exportAppLazy,
        LazySvc<ImportApp> importAppLazy,
        LazySvc<AppCreator> appBuilderLazy,
        LazySvc<ResetApp> resetAppLazy,
        LazySvc<AppCachePurger> systemManagerLazy,
        LazySvc<LanguagesBackend> languagesBackendLazy,
        LazySvc<IAppStates> appStatesLazy,
        LazySvc<AppStackBackend> appStackBackendLazy,
        LazySvc<IJsonService> json,
        IGlobalConfiguration globalConfiguration
    ) : base($"{Eav.EavLogs.WebApi}.{LogSuffix}Rl")
    {
        ConnectServices(
            _appsBackendLazy = appsBackendLazy,
            _workAppsRemove = workAppsRemove,
            _exportAppLazy = exportAppLazy,
            _importAppLazy = importAppLazy,
            _appBuilderLazy = appBuilderLazy,
            _resetAppLazy = resetAppLazy,
            _systemManagerLazy = systemManagerLazy,
            _languagesBackendLazy = languagesBackendLazy,
            _appStatesLazy = appStatesLazy,
            _appStackBackendLazy = appStackBackendLazy,
            _json = json,
            _globalConfiguration = globalConfiguration
        );
    }

    private readonly LazySvc<WorkAppsRemove> _workAppsRemove;

    private readonly LazySvc<AppsBackend> _appsBackendLazy;
    private readonly LazySvc<ExportApp> _exportAppLazy;
    private readonly LazySvc<ImportApp> _importAppLazy;
    private readonly LazySvc<AppCreator> _appBuilderLazy;
    private readonly LazySvc<ResetApp> _resetAppLazy;
    private readonly LazySvc<AppCachePurger> _systemManagerLazy;
    private readonly LazySvc<LanguagesBackend> _languagesBackendLazy;
    private readonly LazySvc<IAppStates> _appStatesLazy;
    private readonly LazySvc<AppStackBackend> _appStackBackendLazy;


    public List<AppDto> List(int zoneId) => _appsBackendLazy.Value.Apps();

    public List<AppDto> InheritableApps() => _appsBackendLazy.Value.GetInheritableApps();

    public void App(int zoneId, int appId, bool fullDelete = true)
        => _workAppsRemove.Value /*_cmsZonesLazy.Value.SetId(zoneId).AppsMan*/.RemoveAppInSiteAndEav(zoneId, appId, fullDelete);

    public void App(int zoneId, string name, int? inheritAppId = null, int templateId = 0)
    {
        var l = Log.Fn($"{nameof(zoneId)}:{zoneId}, {nameof(name)}:{name}, {nameof(inheritAppId)}:{inheritAppId}, {nameof(templateId)}:{templateId}");

        if (templateId == 0)
        {
            l.A("create default new app without template");
            _appBuilderLazy.Value.Init(zoneId).Create(name, null, inheritAppId);
            l.Done("ok");
            return; // Exit here, because we created the app without template
        }

        l.A($"find template app zip path for {nameof(templateId)}:{templateId}");
        var zipPath = GetTemplateZipPathOrThrow(templateId);

        l.A($"create new app from template zip:{zipPath}");
        var resultDto = _importAppLazy.Value.Import(zipPath, zoneId, name, inheritAppId);
        if (!resultDto.Success)
            throw l.Ex(new Exception($"Error importing app from template: {string.Join(", ", resultDto.Messages.Select(m => $"{m.MessageType}:{m.Text}"))}"));

        l.Done("ok");
    }

    private string GetTemplateZipPathOrThrow(int templateId)
    {
        var l = Log.Fn<string>($"{nameof(templateId)}:{templateId}");

        var templatesJsonPath = Path.Combine(_globalConfiguration.NewAppsTemplateFolder, TemplatesJson);

        if (!File.Exists(templatesJsonPath))
            throw l.Ex(new FileNotFoundException($"{TemplatesJson} file not found"));

        var templatesJson = File.ReadAllText(templatesJsonPath);
        var templates = _json.Value.To<List<TemplateJson>>(templatesJson);
        var template = templates.FirstOrDefault(t => t.Id == templateId) 
            ?? throw l.Ex(new Exception($"Template with id {templateId} not found in {TemplatesJson}"));

        var zipPath = Path.Combine(_globalConfiguration.NewAppsTemplateFolder, template.Zip);
        if (!File.Exists(zipPath))
            throw l.Ex(new FileNotFoundException($"Template {Path.GetFileName(zipPath)} not found"));

        return l.ReturnAsOk(zipPath);
    }

    public List<SiteLanguageDto> Languages(int appId)
        => _languagesBackendLazy.Value.GetLanguagesOfApp(_appStatesLazy.Value.GetReader(appId), true);

    public AppExportInfoDto Statistics(int zoneId, int appId) => _exportAppLazy.Value.GetAppInfo(zoneId, appId);

    public bool FlushCache(int zoneId, int appId)
    {
        var wrapLog = Log.Fn<bool>($"{zoneId}, {appId}");
        _systemManagerLazy.Value.Purge(zoneId, appId);
        return wrapLog.ReturnTrue("ok");
    }

    public THttpResponseType Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
        => _exportAppLazy.Value.Export(zoneId, appId, includeContentGroups, resetAppGuid) as THttpResponseType;

    public bool SaveData(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool withPortalFiles)
        => _exportAppLazy.Value.SaveDataForVersionControl(zoneId, appId, includeContentGroups, resetAppGuid, withPortalFiles);

    public List<AppStackDataRaw> GetStack(int appId, string part, string key = null, Guid? view = null)
        => _appStackBackendLazy.Value.GetAll(appId, part ?? AppStackConstants.RootNameSettings, key, view, null);

    public ImportResultDto Reset(int zoneId, int appId, string defaultLanguage, bool withPortalFiles)
        => _resetAppLazy.Value.Reset(zoneId, appId, defaultLanguage, withPortalFiles);

    /// <summary>
    /// Import App from import zip.
    /// </summary>
    /// <param name="uploadInfo">file upload</param>
    /// <param name="zoneId">int</param>
    /// <param name="renameApp">optional new name for app, provide to rename the app</param>
    /// <returns></returns>
    public ImportResultDto Import(HttpUploadedFile uploadInfo, int zoneId, string renameApp)
    {
        var wrapLog = Log.Fn<ImportResultDto>();

        if (!uploadInfo.HasFiles())
            return wrapLog.Return(new(false, "no file uploaded"), "no file uploaded");

        var (_, stream) = uploadInfo.GetStream(0);

        var result = _importAppLazy.Value.Import(stream, zoneId, renameApp);

        return wrapLog.ReturnAsOk(result);
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
        var wrapLog = Log.Fn<IEnumerable<PendingAppDto>>();
        var result = _importAppLazy.Value.GetPendingApps(zoneId);
        return wrapLog.ReturnAsOk(result);
    }

    /// <summary>
    /// Install pending apps
    /// </summary>
    /// <param name="zoneId"></param>
    /// <param name="pendingApps"></param>
    /// <returns></returns>
    public ImportResultDto InstallPendingApps(int zoneId, IEnumerable<PendingAppDto> pendingApps)
    {
        var wrapLog = Log.Fn<ImportResultDto>();
        var result = _importAppLazy.Value.InstallPendingApps(zoneId, pendingApps);
        return wrapLog.ReturnAsOk(result);
    }
}