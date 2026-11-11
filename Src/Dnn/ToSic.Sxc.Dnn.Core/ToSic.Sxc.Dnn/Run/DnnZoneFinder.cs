using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Cms;
using ToSic.Sys.Locking;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnZoneFinder(LazySvc<ZoneCreator> zoneCreatorLazy) : ServiceBase("DNN.ZoneMp", connect: [zoneCreatorLazy]), IZoneFinder
{
    /// <inheritdoc />
    /// <summary>
    /// Will get the EAV ZoneId for the current tenant
    /// Always returns a valid value, as it will otherwise create one if it was missing
    /// ...if the tenant/portal exists
    /// </summary>
    /// <param name="siteId"></param>
    /// <returns></returns>
    public int GetZoneId(int siteId)
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
            // Retrieve existing Zone ID if it was created by another thread
            cacheOrFallback: () =>
                GetExistingZoneId(siteId) ??
                throw new InvalidOperationException("Failed to retrieve or create Zone ID."));

        return zoneIdResult.Result;
    }

    // Instance of TryLockTryDo for synchronization
    private readonly TryLockTryDo _zoneCreateLocker = new();

    private static int? GetExistingZoneId(int siteId)
    {
        var portalSettings = PortalController.Instance.GetPortalSettings(siteId);
        if (portalSettings.TryGetValue(SiteSettingNames.SiteKeyForZoneId, out var value) &&
            int.TryParse(value, out var zoneId))
            return zoneId;
        return null;
    }

    private int CreateNewZone(int siteId)
    {
        var portalInfo = new PortalSettings(siteId);
        var newZoneId = zoneCreatorLazy.Value.Create($"{portalInfo.PortalName} (Portal {siteId})");
        PortalController.UpdatePortalSetting(siteId, SiteSettingNames.SiteKeyForZoneId, newZoneId.ToString());
        return newZoneId;
    }
}
