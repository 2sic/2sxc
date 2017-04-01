using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.SexyContent.Environment.Base;
using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ZoneMapper : IZoneMapper
    {
        /// <summary>
        /// Will get the EAV ZoneId for the current tennant
        /// Always returns a valid value, as it will otherwise create one if it was missing
        /// </summary>
        /// <param name="tennantId"></param>
        /// <returns></returns>
        public int GetZoneId(int tennantId)
        {
            // additional protection agains invalid portalid which may come from bad dnn configs and execute in search-index mode
            // see https://github.com/2sic/2sxc/issues/1054
            if (tennantId < 0)
                throw new Exception("Can't get zone for invalid portal ID: " + tennantId);

            var zoneSettingKey = Settings.PortalSettingsPrefix + "ZoneID";
            var c = PortalController.GetPortalSettingsDictionary(tennantId);
            var portalSettings = new PortalSettings(tennantId);

            int zoneId;

            // Create new zone automatically
            if (!c.ContainsKey(zoneSettingKey))
            {
                var newZone =
                    EavDataController.Instance(null, null)
                        .Zone.AddZone(portalSettings.PortalName + " (Portal " + tennantId + ")")
                        .Item1;
                SetZoneId(newZone.ZoneID, tennantId);
                zoneId = newZone.ZoneID;
            }
            else
            {
                zoneId = Int32.Parse(c[zoneSettingKey]);
            }

            return zoneId;
        }

        private void SetZoneId(int tennantId, int zoneId)
        {
            //if (zoneId.HasValue)
            PortalController.UpdatePortalSetting(tennantId, Settings.PortalSettingsPrefix + "ZoneID",
                zoneId /*.Value*/.ToString());
            //else
            //    PortalController.DeletePortalSetting(portalId, Settings.PortalSettingsPrefix + "ZoneID");
        }

        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        public List<Culture> CulturesWithState(int tennantId, int zoneId)
        {
            var availableEavLanguages = EavDataController.Instance(zoneId, State.GetDefaultAppId(zoneId)).Dimensions.GetLanguages();
            var defaultLanguageCode = new PortalSettings(tennantId).DefaultLanguage;
            var defaultLanguage = availableEavLanguages
                .FirstOrDefault(p => p.ExternalKey == defaultLanguageCode);
            var defaultLanguageIsActive = defaultLanguage != null && defaultLanguage.Active;

            return (from c in LocaleController.Instance.GetLocales(tennantId)
                    select new Culture(
                        c.Value.Code,
                        c.Value.Text,
                        availableEavLanguages.Any(a => a.Active && a.ExternalKey == c.Value.Code && a.ZoneID == zoneId),
                        c.Value.Code == defaultLanguageCode && !defaultLanguageIsActive ||
                        (defaultLanguageIsActive && c.Value.Code != defaultLanguageCode))
                )
                .OrderByDescending(c => c.Code == defaultLanguageCode)
                .ThenBy(c => c.Code).ToList();

        }
    }
}