using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnZoneMapper : ZoneMapperBase
    {
        private readonly Lazy<ZoneManager> _zoneManagerLazy;

        /// <inheritdoc />
        public DnnZoneMapper(Lazy<ZoneManager> zoneManagerLazy) : base("DNN.ZoneMp")
        {
            _zoneManagerLazy = zoneManagerLazy;
        }

        /// <inheritdoc />
        /// <summary>
        /// Will get the EAV ZoneId for the current tenant
        /// Always returns a valid value, as it will otherwise create one if it was missing
        /// ...if the tenant/portal exists
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public override int GetZoneId(int tenantId)
        {
            // additional protection against invalid portalId which may come from bad dnn configs and execute in search-index mode
            // see https://github.com/2sic/2sxc/issues/1054
            if (tenantId < 0)
                throw new Exception("Can't get zone for invalid portal ID: " + tenantId);

            //const string zoneSettingKey = Settings.PortalSettingsPrefix + "ZoneID";
            var c = PortalController.Instance.GetPortalSettings(tenantId);

            // Create new zone automatically
            if (c.ContainsKey(Settings.PortalSettingZoneId)) return int.Parse(c[Settings.PortalSettingZoneId]);

            var portalSettings = new PortalSettings(tenantId);
            var zoneId = _zoneManagerLazy.Value.Init(0, Log).CreateZone(portalSettings.PortalName + " (Portal " + tenantId + ")");
            PortalController.UpdatePortalSetting(tenantId, Settings.PortalSettingZoneId, zoneId.ToString());
            return zoneId;

        }

        public override ISite SiteOfZone(int zoneId)
        {
            var pinst = PortalController.Instance;
            var portals = pinst.GetPortals();
            var found = portals.Cast<PortalInfo>().Select(p =>
                {
                    var set = pinst.GetPortalSettings(p.PortalID);
                    if (!set.TryGetValue(Settings.PortalSettingZoneId, out string portalZonId)) return null;
                    if (!int.TryParse(portalZonId, out int zid)) return null;
                    return zid == zoneId ? new PortalSettings(p) : null;
                })
                .FirstOrDefault(f => f != null);
            return found != null ? new DnnSite(found) : null;
        }


        /// <inheritdoc />
        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        /// <param name="tenantId">The ID of the tenant in the host system</param>
        /// <param name="zoneId">The ID of the zone, to check which languages are enabled in this zone</param>
        public override List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            // note: 
            var availableEavLanguages = new ZoneRuntime().Init(zoneId, Log).Languages(true); 
            var defaultLanguageCode = new PortalSettings(tenantId).DefaultLanguage;

            return (from c in LocaleController.Instance.GetLocales(tenantId)
                    select new TempTempCulture(
                        c.Value.Code,
                        c.Value.Text,
                        availableEavLanguages.Any(a => a.Active && a.Matches(c.Value.Code)))
                )
                .OrderByDescending(c => c.Key == defaultLanguageCode)
                .ThenBy(c => c.Key).ToList();
        }
    }
}