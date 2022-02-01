using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;
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
    public class AppControllerReal<HttpResponseType> : HasLog<AppControllerReal<HttpResponseType>>//, IAppController
    {
        private readonly Lazy<AppsBackend> _appsBackendLazy;
        private readonly Lazy<CmsZones> _cmsZonesLazy;
        private readonly Lazy<ExportApp> _exportAppLazy;
        private readonly Lazy<ImportApp> _importAppLazy;
        private readonly Lazy<AppCreator> _appBuilderLazy;
        private readonly Lazy<ResetApp> _resetAppLazy;
        private readonly Lazy<SystemManager> _systemManagerLazy;
        private readonly Lazy<LanguagesBackend> _languagesBackendLazy;
        private readonly Lazy<IAppStates> _appStatesLazy;
        private readonly Lazy<AppStackBackend> _appStackBackendLazy;

        public AppControllerReal(
            Lazy<AppsBackend> appsBackendLazy,
            Lazy<CmsZones> cmsZonesLazy,
            Lazy<ExportApp> exportAppLazy,
            Lazy<ImportApp> importAppLazy,
            Lazy<AppCreator> appBuilderLazy,
            Lazy<ResetApp> resetAppLazy,
            Lazy<SystemManager> systemManagerLazy,
            Lazy<LanguagesBackend> languagesBackendLazy,
            Lazy<IAppStates> appStatesLazy,
            Lazy<AppStackBackend> appStackBackendLazy
            ) : base("Api.AppCon")
        {
            _appsBackendLazy = appsBackendLazy;
            _cmsZonesLazy = cmsZonesLazy;
            _exportAppLazy = exportAppLazy;
            _importAppLazy = importAppLazy;
            _appBuilderLazy = appBuilderLazy;
            _resetAppLazy = resetAppLazy;
            _systemManagerLazy = systemManagerLazy;
            _languagesBackendLazy = languagesBackendLazy;
            _appStatesLazy = appStatesLazy;
            _appStackBackendLazy = appStackBackendLazy;
        }

        private Action _preventServerTimeout300;

        public AppControllerReal<HttpResponseType> Init(Action preventServerTimeout300, ILog parent)
        {
            _preventServerTimeout300 = preventServerTimeout300;
            base.Init(parent);
            return this;
        }

        public List<AppDto> List(int zoneId)
            => _appsBackendLazy.Value.Init(Log).Apps();

        public List<AppDto> InheritableApps()
            => _appsBackendLazy.Value.Init(Log).GetInheritableApps();

        public void App(int zoneId, int appId, bool fullDelete = true)
            => _cmsZonesLazy.Value.Init(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId, fullDelete);

        public void App(int zoneId, string name, int? inheritAppId = null)
            => _appBuilderLazy.Value.Init(zoneId, Log).Create(name, null, inheritAppId);

        public List<SiteLanguageDto> Languages(int appId)
            => _languagesBackendLazy.Value.Init(Log).GetLanguagesOfApp(_appStatesLazy.Value.Get(appId), true);

        public AppExportInfoDto Statistics(int zoneId, int appId)
            => _exportAppLazy.Value.Init(Log).GetAppInfo(appId, zoneId);

        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            _systemManagerLazy.Value.Init(Log).Purge(zoneId, appId);
            return wrapLog("ok", true);
        }

        public HttpResponseType Export<HttpResponseType>(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid) where HttpResponseType : class
            => _exportAppLazy.Value.Init(Log).Export(appId, zoneId, includeContentGroups, resetAppGuid) as HttpResponseType;

        public bool SaveData(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => _exportAppLazy.Value.Init(Log).SaveDataForVersionControl(appId, zoneId, includeContentGroups, resetAppGuid);

        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null)
            => _appStackBackendLazy.Value.GetAll(appId, part ?? AppConstants.RootNameSettings, key, view, null);

        public ImportResultDto Reset(int zoneId, int appId, string defaultLanguage)
        {
            _preventServerTimeout300();
            return _resetAppLazy.Value.Init(Log).Reset(zoneId, appId, defaultLanguage);
        }

        public ImportResultDto Import(int zoneId, string name, Stream stream)
        {
            _preventServerTimeout300();
            return _importAppLazy.Value.Init(Log).Import(zoneId, name, stream);
        }
    }
}
