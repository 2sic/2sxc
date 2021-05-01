using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtZoneMapper : ZoneMapperBase
    {
        #region Constructor and DI

        /// <inheritdoc />
        public OqtZoneMapper(ISiteRepository siteRepository, 
            ISettingRepository settingRepository, 
            IServiceProvider serviceProvider,
            Lazy<ZoneCreator> zoneCreatorLazy,
            Lazy<ILocalizationManager> localizationManager,
            Lazy<ILanguageRepository> languageRepository,
            SiteState siteState) : base($"{OqtConstants.OqtLogPrefix}.ZoneMp")
        {
            _siteRepository = siteRepository;
            _settingRepository = settingRepository;
            _serviceProvider = serviceProvider;
            _zoneCreatorLazy = zoneCreatorLazy;
            _localizationManager = localizationManager;
            _languageRepository = languageRepository;
            _siteState = siteState;
        }
        private readonly ISiteRepository _siteRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<ZoneCreator> _zoneCreatorLazy;
        private readonly Lazy<ILocalizationManager> _localizationManager;
        private readonly Lazy<ILanguageRepository> _languageRepository;
        private readonly SiteState _siteState;

        #endregion

        public override int GetZoneId(int tenantId)
        {
            // additional protection against invalid portalId which may come from bad dnn configs and execute in search-index mode
            // see https://github.com/2sic/2sxc/issues/1054
            if (tenantId < 0)
                throw new Exception("Can't get zone for invalid portal ID: " + tenantId);

            if (HasZoneId(tenantId, out var i)) return i;

            // Create new zone automatically
            var portalSettings = _siteRepository.GetSite(tenantId);
            var zoneId = _zoneCreatorLazy.Value.Init(Log).Create(portalSettings.Name + " (Site " + tenantId + ")");
            _settingRepository.AddSetting(new Setting
            {
                CreatedBy = "2sxc", 
                CreatedOn = DateTime.Now, 
                EntityId = tenantId, 
                EntityName = EntityNames.Site,
                ModifiedBy = "2sxc",
                ModifiedOn = DateTime.Now,
                SettingName = OqtConstants.SiteKeyForZoneId,
                SettingValue = zoneId.ToString()
            });
            return zoneId;
        }

        private bool HasZoneId(int tenantId, out int i)
        {
            var c = _settingRepository.GetSettings(EntityNames.Site, tenantId).ToList();

            var zoneSetting = c.FirstOrDefault(s => s.SettingName == OqtConstants.SiteKeyForZoneId);
            if (zoneSetting != null)
            {
                if (!int.TryParse(zoneSetting.SettingValue, out var zId))
                    throw new Exception(Log.Add($"Got value '{zoneSetting.SettingValue}' for ZoneId but can't convert to int"));
                i = zId;
                return true;
            }

            i = 0;
            return false;
        }

        public override ISite SiteOfZone(int zoneId)
        {
            var sites = _siteRepository.GetSites().ToList();
            var found = sites.FirstOrDefault(p => HasZoneId(p.SiteId, out var zoneOfSite) && zoneOfSite == zoneId);
            return found != null ? _serviceProvider.Build<OqtSite>().Init(found) : null;
        }

        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId) => _supportedCultures ??= GetSupportedCultures(tenantId, zoneId);
        private List<TempTempCulture> _supportedCultures;
        
        public List<TempTempCulture> GetSupportedCultures(int tenantId, int zoneId)
        {
            //return new List<TempTempCulture>
            //{
            //    new TempTempCulture(WipConstants.DefaultLanguage, WipConstants.DefaultLanguageText, true)
            //};
            //return WipConstants.EmptyCultureList;


            // All localizations that are installed in system (/code/Oqtane.Client.resources.dll) but default English is missing in the list.
            string[] allSupportedCultures = _localizationManager.Value.GetSupportedCultures();

            // List of cultures that are enabled for this site (from database).
            var supportedCultures = _languageRepository.Value.GetLanguages(tenantId)
                .Select(c => new TempTempCulture(c.Code, c.Name, allSupportedCultures.Contains(c.Code)))
                .ToList();

            // Oqtane v2.0.2: Add English, because it is missing in list.
            supportedCultures.Add(new TempTempCulture("en-us", "English", true));

            return supportedCultures.OrderBy(c => c.Key).ToList();
        }
    }
}
