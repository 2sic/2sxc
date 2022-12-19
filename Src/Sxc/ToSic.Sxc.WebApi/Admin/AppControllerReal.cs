using System;
using System.Collections.Generic;
using ToSic.Eav;
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

namespace ToSic.Sxc.WebApi.Admin
{
    /// <summary>
    /// Experimental new class
    /// Goal is to reduce code in the Dnn and Oqtane controllers, which basically does the same thing, mostly DI work
    /// </summary>
    public class AppControllerReal<THttpResponseType> : HasLog where THttpResponseType : class
    {
        public const string LogSuffix = "AppCon";

        public AppControllerReal(
            LazyInitLog<AppsBackend> appsBackendLazy,
            Lazy<CmsZones> cmsZonesLazy,
            LazyInitLog<ExportApp> exportAppLazy,
            LazyInitLog<ImportApp> importAppLazy,
            Lazy<AppCreator> appBuilderLazy,
            LazyInitLog<ResetApp> resetAppLazy,
            LazyInitLog<SystemManager> systemManagerLazy,
            LazyInitLog<LanguagesBackend> languagesBackendLazy,
            Lazy<IAppStates> appStatesLazy,
            LazyInitLog<AppStackBackend> appStackBackendLazy
            ) : base($"{LogNames.WebApi}.{LogSuffix}Rl")
        {
            _appsBackendLazy = appsBackendLazy.SetLog(Log);
            _cmsZonesLazy = cmsZonesLazy;
            _exportAppLazy = exportAppLazy.SetLog(Log);
            _importAppLazy = importAppLazy.SetLog(Log);
            _appBuilderLazy = appBuilderLazy;
            _resetAppLazy = resetAppLazy.SetLog(Log);
            _systemManagerLazy = systemManagerLazy.SetLog(Log);
            _languagesBackendLazy = languagesBackendLazy.SetLog(Log);
            _appStatesLazy = appStatesLazy;
            _appStackBackendLazy = appStackBackendLazy.SetLog(Log);
        }

        private readonly LazyInitLog<AppsBackend> _appsBackendLazy;
        private readonly Lazy<CmsZones> _cmsZonesLazy;
        private readonly LazyInitLog<ExportApp> _exportAppLazy;
        private readonly LazyInitLog<ImportApp> _importAppLazy;
        private readonly Lazy<AppCreator> _appBuilderLazy;
        private readonly LazyInitLog<ResetApp> _resetAppLazy;
        private readonly LazyInitLog<SystemManager> _systemManagerLazy;
        private readonly LazyInitLog<LanguagesBackend> _languagesBackendLazy;
        private readonly Lazy<IAppStates> _appStatesLazy;
        private readonly LazyInitLog<AppStackBackend> _appStackBackendLazy;


        public List<AppDto> List(int zoneId) => _appsBackendLazy.Value.Apps();

        public List<AppDto> InheritableApps() => _appsBackendLazy.Value.GetInheritableApps();

        public void App(int zoneId, int appId, bool fullDelete = true)
            => _cmsZonesLazy.Value.Init(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId, fullDelete);

        public void App(int zoneId, string name, int? inheritAppId = null)
            => _appBuilderLazy.Value.Init(zoneId, Log).Create(name, null, inheritAppId);

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
