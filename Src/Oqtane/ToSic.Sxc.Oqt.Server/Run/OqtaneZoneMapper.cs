using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneZoneMapper : ZoneMapperBase
    {
        /// <inheritdoc />
        public OqtaneZoneMapper(/*IHttp http,*/ ISiteRepository siteRepository, ISettingRepository settingRepository) : base("Mvc.ZoneMp")
        {
            //_http = http;
            _siteRepository = siteRepository;
            _settingRepository = settingRepository;
        }
        //private readonly IHttp _http;
        private readonly ISiteRepository _siteRepository;
        private readonly ISettingRepository _settingRepository;

        public override int GetZoneId(int tenantId)
        {
            // additional protection against invalid portalId which may come from bad dnn configs and execute in search-index mode
            // see https://github.com/2sic/2sxc/issues/1054
            if (tenantId < 0)
                throw new Exception("Can't get zone for invalid portal ID: " + tenantId);

            if (HasZoneId(tenantId, out var i)) return i;

            // Create new zone automatically
            var portalSettings = _siteRepository.GetSite(tenantId);
            var zoneId = ZoneManager.CreateZone(portalSettings.Name + " (Site " + tenantId + ")", Log);
            _settingRepository.AddSetting(new Setting
            {
                CreatedBy = "2sxc", 
                CreatedOn = DateTime.Now, 
                EntityId = tenantId, 
                EntityName = OqtConstants.SiteSettingsName,
                ModifiedBy = "2sxc",
                ModifiedOn = DateTime.Now,
                SettingName = OqtConstants.SiteKeyForZoneId,
                SettingValue = zoneId.ToString()
            });
            return zoneId;

        }

        private bool HasZoneId(int tenantId, out int i)
        {
            var c = _settingRepository.GetSettings(OqtConstants.SiteSettingsName, tenantId).ToList();

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

        public override ITenant TenantOfZone(int zoneId)
        {
            var sites = _siteRepository.GetSites().ToList();
            var found = sites.FirstOrDefault(p => HasZoneId(p.SiteId, out var zoneOfSite) && zoneOfSite == zoneId);
            return found != null ? new OqtaneTenantSite(found) : null;
        }

        public ITenant TenantOfSite(int siteId)
        {
            var site = _siteRepository.GetSite(siteId);
            return new OqtaneTenantSite(site);
        }

        // TODO: #Oqtane
        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            return new List<TempTempCulture>
            {
                new TempTempCulture(WipConstants.DefaultLanguage, WipConstants.DefaultLanguageText, true)
            };
        }

    }
}
