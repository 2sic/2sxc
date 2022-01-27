using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Languages;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Admin
{
    /// <summary>
    /// Experimental new class
    /// Goal is to reduce code in the Dnn and Oqtane controllers, which basically does the same thing, mostly DI work
    /// </summary>
    public class AppControllerReal: HasLog<AppControllerReal> // , IAppController
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

        public AppControllerReal(
            Lazy<AppsBackend> appsBackendLazy,
            Lazy<CmsZones> cmsZonesLazy,
            Lazy<ExportApp> exportAppLazy,
            Lazy<ImportApp> importAppLazy,
            Lazy<AppCreator> appBuilderLazy,
            Lazy<ResetApp> resetAppLazy,
            Lazy<SystemManager> systemManagerLazy,
            Lazy<LanguagesBackend> languagesBackendLazy,
            Lazy<IAppStates> appStatesLazy
            ): base("Api.AppCon")
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
        }

        public List<SiteLanguageDto> Languages(int appId)
            => _languagesBackendLazy.Value.Init(Log).GetLanguagesOfApp(_appStatesLazy.Value.Get(appId), true);


    }
}
