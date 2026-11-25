using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Cms;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtZoneOfSiteSvc(
    ISiteRepository siteRepository,
    ISettingRepository settingRepository,
    LazySvc<ZoneCreator> zoneCreatorLazy,
    OqtSiteGroup oqtSiteGroup,
    LazySvc<ITenantManager> tenantManager)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.ZoneMp",
        connect: [siteRepository, settingRepository, zoneCreatorLazy, oqtSiteGroup, tenantManager]), IZoneOfSiteSvc
{
    public int GetZoneId(int siteId)
    {
        // additional protection against invalid portalId which may come from bad configs and execute in search-index mode
        // see https://github.com/2sic/2sxc/issues/1054
        // in Oqtane minimal siteId = 1, so 0 and negative values are invalid
        if (siteId <= 0)
            throw new("Can't get zone for invalid portal ID: " + siteId);

        var zoneSiteId = oqtSiteGroup.GetPrimaryLocalizationSiteId(siteId);
        if (HasZoneId(zoneSiteId, out var existingZoneId))
            return existingZoneId;

        // Ensure the correct tenant/site is set in Oqtane SiteState before touching EAV/EF
        // This makes SqlPlatformInfo/GlobalConfig resolve the tenant-specific connection string.
        var portalSettings = siteRepository.GetSite(zoneSiteId);
        tenantManager.Value.SetAlias(portalSettings.TenantId, zoneSiteId);

        // Create new zone automatically, now using the proper tenant DB
        var newZoneId = zoneCreatorLazy.Value.Create(portalSettings.Name + " (Site " + zoneSiteId + ")");
        settingRepository.AddSetting(new()
        {
            CreatedBy = "2sxc",
            CreatedOn = DateTime.UtcNow,
            EntityId = zoneSiteId,
            EntityName = EntityNames.Site,
            ModifiedBy = "2sxc",
            ModifiedOn = DateTime.UtcNow,
            SettingName = SiteSettingNames.SiteKeyForZoneId,
            SettingValue = newZoneId.ToString()
        });
        return newZoneId;
    }

    private bool HasZoneId(int siteId, out int zoneId)
    {
        var zoneSetting = settingRepository.GetSetting(EntityNames.Site, siteId, SiteSettingNames.SiteKeyForZoneId);
        if (zoneSetting is not null)
        {
            if (!int.TryParse(zoneSetting.SettingValue, out var parsedZoneId))
            {
                var msg = $"Got value '{zoneSetting.SettingValue}' for ZoneId but can't convert to int";
                Log.A(msg);
                throw new(msg);
            }
            zoneId = parsedZoneId;
            return true;
        }

        zoneId = 0;
        return false;
    }

}
