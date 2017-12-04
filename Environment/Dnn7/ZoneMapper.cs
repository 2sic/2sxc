using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ZoneMapper<T> : HasLog, IZoneMapper<T> where T: PortalSettings
    {
        public ZoneMapper(Log parentLog = null) : base("DNN.ZoneMp", parentLog)
        {
        }

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
            var c = PortalController.Instance.GetPortalSettings(tennantId);//.GetPortalSettingsDictionary(tennantId);
            var portalSettings = new PortalSettings(tennantId);

            int zoneId;

            // Create new zone automatically
            if (!c.ContainsKey(zoneSettingKey))
            {
                zoneId = ZoneManager.CreateZone(portalSettings.PortalName + " (Portal " + tennantId + ")", Log);
                PortalController.UpdatePortalSetting(tennantId, Settings.PortalSettingZoneId, zoneId.ToString());
            }
            else zoneId = Int32.Parse(c[zoneSettingKey]);

            return zoneId;
        }

        public int GetZoneId(Tennant<T> tennant) => GetZoneId(tennant.Settings.PortalId);


        /// <summary>
        /// Returns all DNN Cultures with active / inactive state
        /// </summary>
        public List<TempTempCulture> CulturesWithState(int tennantId, int zoneId)
        {
            // note: 
            var availableEavLanguages = new ZoneRuntime(zoneId, Log).Languages(true); 
            var defaultLanguageCode = new PortalSettings(tennantId).DefaultLanguage;
            var defaultLanguage = availableEavLanguages
                .FirstOrDefault(p => p.Matches(defaultLanguageCode));
            //var defaultLanguageIsActive = defaultLanguage?.Active == true;

            return (from c in LocaleController.Instance.GetLocales(tennantId)
                    select new TempTempCulture(
                        c.Value.Code,
                        c.Value.Text,
                        availableEavLanguages.Any(a => a.Active && a.Matches(c.Value.Code))
                        //,
                        //string.Equals(c.Value.Code, defaultLanguageCode, StringComparison.InvariantCultureIgnoreCase) && !defaultLanguageIsActive ||
                        //defaultLanguageIsActive && !string.Equals(c.Value.Code, defaultLanguageCode,
                        //    StringComparison.InvariantCultureIgnoreCase)
                            )
                )
                .OrderByDescending(c => c.Key == defaultLanguageCode)
                .ThenBy(c => c.Key).ToList();

        }

        public List<TempTempCulture> CulturesWithState(T tennant, int zoneId) => CulturesWithState(tennant.PortalId, zoneId);
    }
}