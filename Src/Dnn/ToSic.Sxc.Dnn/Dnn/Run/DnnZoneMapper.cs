using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnZoneMapper : HasLog, IZoneMapper
    {
        /// <inheritdoc />
        public DnnZoneMapper() : base("DNN.ZoneMp")
        {
        }

        public IZoneMapper Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }


        /// <inheritdoc />
        /// <summary>
        /// Will get the EAV ZoneId for the current tenant
        /// Always returns a valid value, as it will otherwise create one if it was missing
        /// ...if the tenant/portal exists
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public int GetZoneId(int tenantId)
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
            var zoneId = ZoneManager.CreateZone(portalSettings.PortalName + " (Portal " + tenantId + ")", Log);
            PortalController.UpdatePortalSetting(tenantId, Settings.PortalSettingZoneId, zoneId.ToString());
            return zoneId;

        }

        public int GetZoneId(ITenant tenant) => GetZoneId(tenant.Id);
        public IAppIdentity IdentityFromTenant(int tenantId, int appId) 
            => new AppIdentity(GetZoneId(tenantId), appId);

        public ITenant Tenant(int zoneId)
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
            return found != null ? new DnnTenant(found) : null;
        }


        /// <inheritdoc />
        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        /// <param name="tenantId">The ID of the tenant in the host system</param>
        /// <param name="zoneId">The ID of the zone, to check which languages are enabled in this zone</param>
        public List<TempTempCulture> CulturesWithState(int tenantId, int zoneId)
        {
            // note: 
            var availableEavLanguages = new ZoneRuntime(zoneId, Log).Languages(true); 
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

        public List<TempTempCulture> CulturesWithState(PortalSettings tenant, int zoneId) => CulturesWithState(tenant.PortalId, zoneId);
    }
}