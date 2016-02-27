using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;

namespace ToSic.SexyContent.Statics
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
            var zoneSettingKey = SexyContent.PortalSettingsPrefix + "ZoneID";
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
                PortalController.UpdatePortalSetting(PortalID, SexyContent.PortalSettingsPrefix + "ZoneID", ZoneID.Value.ToString());
            else
                PortalController.DeletePortalSetting(PortalID, SexyContent.PortalSettingsPrefix + "ZoneID");
        }

        public static List<Zone> GetZones()
        {
            return new SexyContent(Constants.DefaultZoneId, AppHelpers.GetDefaultAppId(Constants.DefaultZoneId)).ContentContext.Zone.GetZones();
        }

        public static Zone AddZone(string zoneName)
        {
            return
                new SexyContent(Constants.DefaultZoneId, AppHelpers.GetDefaultAppId(Constants.DefaultZoneId)).ContentContext.Zone
                    .AddZone(zoneName).Item1;
        }
    }
}