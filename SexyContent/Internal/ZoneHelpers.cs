using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav;

namespace ToSic.SexyContent.Internal
{
    public class ZoneHelpers
    {
        /// <summary>
        /// Returns the ZoneID from PortalSettings
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static int? GetZoneID(int portalId)
        {
            var zoneSettingKey = Settings.PortalSettingsPrefix + "ZoneID";
            var c = PortalController.GetPortalSettingsDictionary(portalId);
            var portalSettings = new PortalSettings(portalId);

            int zoneId;

            // Create new zone automatically
            if (!c.ContainsKey(zoneSettingKey))
            {
                var newZone = AddZone(portalSettings.PortalName + " (Portal " + portalId + ")");
                SetZoneID(newZone.ZoneID, portalId);
                zoneId = newZone.ZoneID;
            }
            else
            {
                zoneId = Int32.Parse(c[zoneSettingKey]);
            }

            return zoneId;
        }

        /// <summary>
        /// Sets the ZoneID from PortalSettings
        /// </summary>
        /// <param name="ZoneID"></param>
        /// <param name="PortalID"></param>
        public static void SetZoneID(int? ZoneID, int PortalID)
        {
            if (ZoneID.HasValue)
                PortalController.UpdatePortalSetting(PortalID, Settings.PortalSettingsPrefix + "ZoneID", ZoneID.Value.ToString());
            else
                PortalController.DeletePortalSetting(PortalID, Settings.PortalSettingsPrefix + "ZoneID");
        }

        public static List<Zone> GetZones()
        {
            return new SxcInstance(Constants.DefaultZoneId, AppHelpers.GetDefaultAppId(Constants.DefaultZoneId)).EavAppContext.Zone.GetZones();
        }

        public static Zone AddZone(string zoneName)
        {
            return
                new SxcInstance(Constants.DefaultZoneId, AppHelpers.GetDefaultAppId(Constants.DefaultZoneId)).EavAppContext.Zone
                    .AddZone(zoneName).Item1;
        }

        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        public static List<CulturesWithActiveState> GetCulturesWithActiveState(int portalId, int zoneId)
        {
            //var DefaultLanguageID = ContentContext.GetLanguageId();
            var AvailableEAVLanguages = new SxcInstance(zoneId, AppHelpers.GetDefaultAppId(zoneId)).EavAppContext.Dimensions.GetLanguages();
            var DefaultLanguageCode = new PortalSettings(portalId).DefaultLanguage;
            var DefaultLanguage = AvailableEAVLanguages.Where(p => p.ExternalKey == DefaultLanguageCode).FirstOrDefault();
            var DefaultLanguageIsActive = DefaultLanguage != null && DefaultLanguage.Active;

            return (from c in LocaleController.Instance.GetLocales(portalId)
                select new CulturesWithActiveState
                {
                    Code = c.Value.Code,
                    Text = c.Value.Text,
                    Active = AvailableEAVLanguages.Any(a => a.Active && a.ExternalKey == c.Value.Code && a.ZoneID == zoneId),
                    // Allow State Change only if
                    // 1. This is the default language and default language is not active or
                    // 2. This is NOT the default language and default language is active
                    AllowStateChange = (c.Value.Code == DefaultLanguageCode && !DefaultLanguageIsActive) || (DefaultLanguageIsActive && c.Value.Code != DefaultLanguageCode)
                }).OrderByDescending(c => c.Code == DefaultLanguageCode).ThenBy(c => c.Code).ToList();

        }

        public class CulturesWithActiveState
        {
            public string Code { get; set; }
            public string Text { get; set; }
            public bool Active { get; set; }
            public bool AllowStateChange { get; set; }
        }
    }
}