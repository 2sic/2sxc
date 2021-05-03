using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps;
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
        /// <inheritdoc />
        public OqtZoneMapper(ISiteRepository siteRepository, 
            ISettingRepository settingRepository, 
            IServiceProvider serviceProvider,
            Lazy<ZoneCreator> zoneCreatorLazy,
            OqtCulture oqtCulture) : base($"{OqtConstants.OqtLogPrefix}.ZoneMp")
        {
            _siteRepository = siteRepository;
            _settingRepository = settingRepository;
            _serviceProvider = serviceProvider;
            _zoneCreatorLazy = zoneCreatorLazy;
            _oqtCulture = oqtCulture;
        }
        private readonly ISiteRepository _siteRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<ZoneCreator> _zoneCreatorLazy;
        private readonly OqtCulture _oqtCulture;


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

        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            if (_supportedCultures != null) return _supportedCultures;
            var availableEavLanguages = new ZoneRuntime().Init(zoneId, Log).Languages(true);
            _supportedCultures = _oqtCulture.GetSupportedCultures(tenantId, availableEavLanguages);
            return _supportedCultures;
        }
        private List<TempTempCulture> _supportedCultures;
    }
}
