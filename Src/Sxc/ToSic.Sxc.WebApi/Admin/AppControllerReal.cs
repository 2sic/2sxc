using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
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
    public class AppControllerReal<THttpResponseType> : HasLog<AppControllerReal<THttpResponseType>> where THttpResponseType : class
    {
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
            ) : base("Api.AppCon")
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


        public List<AppDto> List(int zoneId) => _appsBackendLazy.Ready.Apps();

        public List<AppDto> InheritableApps() => _appsBackendLazy.Ready.GetInheritableApps();

        public void App(int zoneId, int appId, bool fullDelete = true)
            => _cmsZonesLazy.Value.Init(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId, fullDelete);

        public void App(int zoneId, string name, int? inheritAppId = null)
            => _appBuilderLazy.Value.Init(zoneId, Log).Create(name, null, inheritAppId);

        public List<SiteLanguageDto> Languages(int appId)
            => _languagesBackendLazy.Ready.GetLanguagesOfApp(_appStatesLazy.Value.Get(appId), true);

        public AppExportInfoDto Statistics(int zoneId, int appId) => _exportAppLazy.Ready.GetAppInfo(zoneId, appId);

        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            _systemManagerLazy.Ready.Purge(zoneId, appId);
            return wrapLog("ok", true);
        }

        public THttpResponseType Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
            => _exportAppLazy.Ready.Export(zoneId, appId, includeContentGroups, resetAppGuid) as THttpResponseType;

        public bool SaveData(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
            => _exportAppLazy.Ready.SaveDataForVersionControl(zoneId, appId, includeContentGroups, resetAppGuid);

        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null)
            => _appStackBackendLazy.Ready.GetAll(appId, part ?? AppConstants.RootNameSettings, key, view, null);

        public ImportResultDto Reset(int zoneId, int appId, string defaultLanguage) => _resetAppLazy.Ready.Reset(zoneId, appId, defaultLanguage);

        public ImportResultDto Import(int zoneId, string name, Stream stream) => _importAppLazy.Ready.Import(zoneId, name, stream);
    }
}
