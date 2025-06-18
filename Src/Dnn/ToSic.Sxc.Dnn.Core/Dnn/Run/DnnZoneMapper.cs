using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sys.Locking;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnZoneMapper(Generator<ISite> site, LazySvc<ZoneCreator> zoneCreatorLazy, IAppsCatalog appsCatalog)
    : ZoneMapperBase(appsCatalog, "DNN.ZoneMp", connect: [site, zoneCreatorLazy])
{
    /// <summary>
    /// This is the name of the setting in the PortalSettings pointing to the zone of this portal
    /// </summary>
    private const string PortalSettingZoneId = "TsDynDataZoneID";

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
        // Additional protection against invalid portalId
        if (siteId < 0)
            throw new ArgumentException("Can't get zone for invalid portal ID: " + siteId);

        // Attempt to retrieve existing Zone ID
        var existingZoneId = GetExistingZoneId(siteId);
        if (existingZoneId.HasValue)
            return existingZoneId.Value;

        // Use TryLockTryDo to prevent race condition during zone creation
        var zoneIdResult = _zoneCreateLocker.Call(
            conditionToGenerate: () => !GetExistingZoneId(siteId).HasValue,
            generator: () => CreateNewZone(siteId),
            cacheOrFallback: () =>
            {
                // Retrieve existing Zone ID if it was created by another thread
                var fallbackZoneId = GetExistingZoneId(siteId);
                if (fallbackZoneId.HasValue)
                    return fallbackZoneId.Value;
                throw new InvalidOperationException("Failed to retrieve or create Zone ID.");
            });

        return zoneIdResult.Result;
    }
    // Instance of TryLockTryDo for synchronization
    private readonly TryLockTryDo _zoneCreateLocker = new();

    private static int? GetExistingZoneId(int siteId)
    {
        var portalSettings = PortalController.Instance.GetPortalSettings(siteId);
        if (portalSettings.TryGetValue(PortalSettingZoneId, out var value) && int.TryParse(value, out var zoneId))
            return zoneId;
        return null;
    }

    private int CreateNewZone(int siteId)
    {
        var portalInfo = new PortalSettings(siteId);
        var newZoneId = zoneCreatorLazy.Value.Create($"{portalInfo.PortalName} (Portal {siteId})");
        PortalController.UpdatePortalSetting(siteId, PortalSettingZoneId, newZoneId.ToString());
        return newZoneId;
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
            ? l.ReturnNull("not found")
            : l.Return(((DnnSite)site.New()).TryInitPortal(found, Log), $"found {found.PortalId}");
    }

    /// <inheritdoc />
    public override List<ISiteLanguageState> CulturesWithState(ISite ofSite)
    {
        if (_supportedCultures != null)
            return _supportedCultures;

        var availableEavLanguages = AppsCatalog.Zone(ofSite.ZoneId).Languages;
        var defaultLanguageCode = ofSite.DefaultCultureCode;

        return _supportedCultures = LocaleController.Instance.GetLocales(ofSite.Id)
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