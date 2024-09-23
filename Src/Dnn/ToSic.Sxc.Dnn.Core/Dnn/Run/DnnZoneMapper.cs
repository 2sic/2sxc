using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Cms.Internal.Languages;
using ToSic.Eav.Context;
using ToSic.Eav.Integration;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnZoneMapper(Generator<ISite> site, LazySvc<ZoneCreator> zoneCreatorLazy, IAppsCatalog appsCatalog)
    : ZoneMapperBase(appsCatalog, "DNN.ZoneMp", connect: [site, zoneCreatorLazy])
{
    /// <summary>
    /// This is the name of the setting in the PortalSettings pointing to the zone of this portal
    /// </summary>
    private const string PortalSettingZoneId = "ToSIC_SexyContent_ZoneID";

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
            throw new("Can't get zone for invalid portal ID: " + siteId);

        var c = PortalController.Instance.GetPortalSettings(siteId);

        // Create new zone automatically
        if (c.TryGetValue(PortalSettingZoneId, out var value)) return int.Parse(value);

        var portalSettings = new PortalSettings(siteId);
        var zoneId = zoneCreatorLazy.Value.Create(portalSettings.PortalName + " (Portal " + siteId + ")");
        PortalController.UpdatePortalSetting(siteId, PortalSettingZoneId, zoneId.ToString());
        return zoneId;

    }

    public override ISite SiteOfZone(int zoneId)
    {
        var l = Log.Fn<ISite>($"{zoneId}");
        var portalController = PortalController.Instance;
        var portals = portalController.GetPortals();
        l.A($"Sites/Portals Count: {portals.Count}");
        var found = portals
            .Cast<PortalInfo>()
            .Select(p =>
            {
                var pSettings = portalController.GetPortalSettings(p.PortalID);
                if (!pSettings.TryGetValue(PortalSettingZoneId, out var portalZoneId)) return null;
                if (!int.TryParse(portalZoneId, out var zid)) return null;
                return zid == zoneId ? new PortalSettings(p) : null;
            })
            .FirstOrDefault(f => f != null);

        return found == null
            ? l.Return(null, "not found")
            : l.Return(((DnnSite)site.New()).TryInitPortal(found, Log), $"found {found.PortalId}");
    }

    /// <inheritdoc />
    public override List<ISiteLanguageState> CulturesWithState(ISite site)
    {
        if (_supportedCultures != null)
            return _supportedCultures;

        var availableEavLanguages = AppsCatalog.Zone(site.ZoneId).Languages;
        var defaultLanguageCode = site.DefaultCultureCode;

        return _supportedCultures = LocaleController.Instance.GetLocales(site.Id)
            .Select(c => new SiteLanguageState(
                c.Value.Code, 
                c.Value.Text,
                availableEavLanguages.Any(a => a.Active && a.Matches(c.Value.Code))))
            .OrderByDescending(c => c.Code == defaultLanguageCode)
            .ThenBy(c => c.Code)
            .Cast<ISiteLanguageState>()
            .ToList();
    }
    private List<ISiteLanguageState> _supportedCultures;
}