using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnZoneMapper : ZoneMapperBase
    {
        /// <summary>
        /// This is the name of the setting in the PortalSettings pointing to the zone of this portal
        /// </summary>
        private const string PortalSettingZoneId = "ToSIC_SexyContent_ZoneID";

        /// <inheritdoc />
        public DnnZoneMapper(IServiceProvider spForNewSites, Lazy<ZoneCreator> zoneCreatorLazy, IAppStates appStates) : base(appStates, "DNN.ZoneMp")
        {
            _spForNewSites = spForNewSites;
            _zoneCreatorLazy = zoneCreatorLazy;
        }
        private readonly IServiceProvider _spForNewSites;
        private readonly Lazy<ZoneCreator> _zoneCreatorLazy;


        /// <inheritdoc />
        /// <summary>
        /// Will get the EAV ZoneId for the current tenant
        /// Always returns a valid value, as it will otherwise create one if it was missing
        /// ...if the tenant/portal exists
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public override int GetZoneId(int siteId)
        {
            // additional protection against invalid portalId which may come from bad dnn configs and execute in search-index mode
            // see https://github.com/2sic/2sxc/issues/1054
            if (siteId < 0)
                throw new Exception("Can't get zone for invalid portal ID: " + siteId);

            //const string zoneSettingKey = Settings.PortalSettingsPrefix + "ZoneID";
            var c = PortalController.Instance.GetPortalSettings(siteId);

            // Create new zone automatically
            if (c.ContainsKey(PortalSettingZoneId)) return int.Parse(c[PortalSettingZoneId]);

            var portalSettings = new PortalSettings(siteId);
            var zoneId = _zoneCreatorLazy.Value.Init(Log).Create(portalSettings.PortalName + " (Portal " + siteId + ")");
            PortalController.UpdatePortalSetting(siteId, PortalSettingZoneId, zoneId.ToString());
            return zoneId;

        }

        public override ISite SiteOfZone(int zoneId)
        {
            var wrapLog = Log.Call<ISite>($"{zoneId}");
            var pinst = PortalController.Instance;
            var portals = pinst.GetPortals();
            Log.Add($"Sites/Portals Count: {portals.Count}");
            var found = portals.Cast<PortalInfo>().Select(p =>
                {
                    var pSettings = pinst.GetPortalSettings(p.PortalID);
                    if (!pSettings.TryGetValue(PortalSettingZoneId, out var portalZoneId)) return null;
                    if (!int.TryParse(portalZoneId, out var zid)) return null;
                    return zid == zoneId ? new PortalSettings(p) : null;
                })
                .FirstOrDefault(f => f != null);

            return found == null 
                ? wrapLog("not found", (DnnSite)null) 
                : wrapLog($"found {found.PortalId}", _spForNewSites.Build<DnnSite>().Swap(found));
        }


        /// <inheritdoc />
        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        /// <param name="siteId">The ID of the tenant in the host system</param>
        /// <param name="zoneId">The ID of the zone, to check which languages are enabled in this zone</param>
        public override List<TempTempCulture> CulturesWithState(int siteId, int zoneId)
        {
            // note: 
            var availableEavLanguages = AppStates.Languages(zoneId, true); // _spForNewSites.Build<ZoneRuntime>().Init(zoneId, Log).Languages(true); 
            var defaultLanguageCode = new PortalSettings(siteId).DefaultLanguage;

            return (from c in LocaleController.Instance.GetLocales(siteId)
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