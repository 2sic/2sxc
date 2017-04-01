namespace ToSic.SexyContent.Internal
{
    public class ZoneHelpers
    {
        ///// <summary>
        ///// Returns the ZoneID from PortalSettings
        ///// </summary>
        ///// <param name="portalId"></param>
        ///// <returns></returns>
        //public static int? GetZoneId(int portalId)
        //{
        //    return new Environment.Environment().ZoneMapper.GetZoneId(portalId);
        //}

        // 2017-04-01 2dm move to environenment.dnn7...
        ///// <summary>
        ///// Sets the ZoneID from PortalSettings
        ///// </summary>
        ///// <param name="zoneId"></param>
        ///// <param name="portalId"></param>
        //public static void SetZoneId(int zoneId, int portalId)
        //{
        //    //if (zoneId.HasValue)
        //        PortalController.UpdatePortalSetting(portalId, Settings.PortalSettingsPrefix + "ZoneID", zoneId/*.Value*/.ToString());
        //    //else
        //    //    PortalController.DeletePortalSetting(portalId, Settings.PortalSettingsPrefix + "ZoneID");
        //}

        // 2017-04-01 2dm removed, unused
        //public static List<Zone> GetZones()
        //{
        //    return EavDataController.Instance(Constants.DefaultZoneId, State.GetDefaultAppId(Constants.DefaultZoneId)).Zone.GetZones();

        //    //return new SxcInstance(Constants.DefaultZoneId, AppHelpers.GetDefaultAppId(Constants.DefaultZoneId)).EavAppContext.Zone.GetZones();
        //}

        // 2017-04-01 2dm factored away
        //public static Zone AddZone(string zoneName)
        //{
        //    return EavDataController.Instance(null, null).Zone.AddZone(zoneName).Item1;
        //}

        ///// <summary>
        ///// Returns all DNN Cultures with active / inactive state
        ///// </summary>
        //public static List<Culture> CulturesWithState(int tennantId, int zoneId)
        //{
        //    var availableEavLanguages = EavDataController.Instance(zoneId, State.GetDefaultAppId(zoneId)).Dimensions.GetLanguages();
        //    var defaultLanguageCode = new PortalSettings(tennantId).DefaultLanguage;
        //    var defaultLanguage = availableEavLanguages
        //        .FirstOrDefault(p => p.ExternalKey == defaultLanguageCode);
        //    var defaultLanguageIsActive = defaultLanguage != null && defaultLanguage.Active;

        //    return (from c in LocaleController.Instance.GetLocales(tennantId)
        //        select new Culture(
        //            c.Value.Code, 
        //            c.Value.Text, 
        //            availableEavLanguages.Any(a => a.Active && a.ExternalKey == c.Value.Code && a.ZoneID == zoneId),
        //            c.Value.Code == defaultLanguageCode && !defaultLanguageIsActive || (defaultLanguageIsActive && c.Value.Code != defaultLanguageCode)) 
        //        //{
        //        //    Code = c.Value.Code,
        //        //    Text = c.Value.Text,
        //        //    Active = availableEavLanguages.Any(a => a.Active && a.ExternalKey == c.Value.Code && a.ZoneID == zoneId),
        //        //    // Allow State Change only if
        //        //    // 1. This is the default language and default language is not active or
        //        //    // 2. This is NOT the default language and default language is active
        //        //    AllowStateChange = (c.Value.Code == defaultLanguageCode && !defaultLanguageIsActive) || (defaultLanguageIsActive && c.Value.Code != defaultLanguageCode)
        //        //}
        //        ).OrderByDescending(c => c.Code == defaultLanguageCode).ThenBy(c => c.Code).ToList();

        //}

        //public class CulturesWithActiveState: Culture
        //{
        //    public string Code { get; set; }
        //    public string Text { get; set; }
        //    public bool Active { get; set; }
        //    public bool AllowStateChange { get; set; }
        //}
    }
}