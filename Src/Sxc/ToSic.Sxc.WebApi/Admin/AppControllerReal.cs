using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Configuration;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Languages;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.AppStack;
using ToSic.Sxc.WebApi.ImportExport;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.WebApi.Admin
{
    /// <summary>
    /// Experimental new class
    /// Goal is to reduce code in the Dnn and Oqtane controllers, which basically does the same thing, mostly DI work
    /// </summary>
    public class AppControllerReal<THttpResponseType> : ServiceBase where THttpResponseType : class
    {
        public const string LogSuffix = "AppCon";

        public AppControllerReal(
            LazySvc<AppsBackend> appsBackendLazy,
            LazySvc<CmsZones> cmsZonesLazy,
            LazySvc<ExportApp> exportAppLazy,
            LazySvc<ImportApp> importAppLazy,
            LazySvc<AppCreator> appBuilderLazy,
            LazySvc<ResetApp> resetAppLazy,
            LazySvc<SystemManager> systemManagerLazy,
            LazySvc<LanguagesBackend> languagesBackendLazy,
            LazySvc<IAppStates> appStatesLazy,
            LazySvc<AppStackBackend> appStackBackendLazy
            ) : base($"{Eav.EavLogs.WebApi}.{LogSuffix}Rl")
        {
            ConnectServices(
                _appsBackendLazy = appsBackendLazy,
                _cmsZonesLazy = cmsZonesLazy,
                _exportAppLazy = exportAppLazy,
                _importAppLazy = importAppLazy,
                _appBuilderLazy = appBuilderLazy,
                _resetAppLazy = resetAppLazy,
                _systemManagerLazy = systemManagerLazy,
                _languagesBackendLazy = languagesBackendLazy,
                _appStatesLazy = appStatesLazy,
                _appStackBackendLazy = appStackBackendLazy
            );
        }

        private readonly LazySvc<AppsBackend> _appsBackendLazy;
        private readonly LazySvc<CmsZones> _cmsZonesLazy;
        private readonly LazySvc<ExportApp> _exportAppLazy;
        private readonly LazySvc<ImportApp> _importAppLazy;
        private readonly LazySvc<AppCreator> _appBuilderLazy;
        private readonly LazySvc<ResetApp> _resetAppLazy;
        private readonly LazySvc<SystemManager> _systemManagerLazy;
        private readonly LazySvc<LanguagesBackend> _languagesBackendLazy;
        private readonly LazySvc<IAppStates> _appStatesLazy;
        private readonly LazySvc<AppStackBackend> _appStackBackendLazy;


        public List<AppDto> List(int zoneId) => _appsBackendLazy.Value.Apps();

        public List<AppDto> InheritableApps() => _appsBackendLazy.Value.GetInheritableApps();

        public void App(int zoneId, int appId, bool fullDelete = true)
            => _cmsZonesLazy.Value.SetId(zoneId).AppsMan.RemoveAppInSiteAndEav(appId, fullDelete);

        public void App(int zoneId, string name, int? inheritAppId = null)
            => _appBuilderLazy.Value.Init(zoneId).Create(name, null, inheritAppId);

        public List<SiteLanguageDto> Languages(int appId)
            => _languagesBackendLazy.Value.GetLanguagesOfApp(_appStatesLazy.Value.Get(appId), true);

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

        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null)
            => _appStackBackendLazy.Value.GetAll(appId, part ?? ConfigurationConstants.RootNameSettings, key, view, null);

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
                return wrapLog.Return(new ImportResultDto(false, "no file uploaded"), "no file uploaded");

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
}
