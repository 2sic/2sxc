using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Cms;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtZoneMapper(
    ISiteRepository siteRepository,
    ISettingRepository settingRepository,
    Generator<ISite> site,
    LazySvc<ZoneCreator> zoneCreatorLazy,
    OqtCulture oqtCulture,
    IAppsCatalog appsCat)
    : ZoneMapperBase(appsCat, $"{OqtConstants.OqtLogPrefix}.ZoneMp",
        connect: [siteRepository, settingRepository, site, zoneCreatorLazy, oqtCulture])
{
    public override int GetZoneId(int siteId)
    {
        // additional protection against invalid portalId which may come from bad configs and execute in search-index mode
        // see https://github.com/2sic/2sxc/issues/1054
        if (siteId < 0)
            throw new("Can't get zone for invalid site ID: " + siteId);

        if (HasZoneId(siteId, out var i)) return i;

        // Create new zone automatically
        var portalSettings = siteRepository.GetSite(siteId);
        var zoneId = zoneCreatorLazy.Value.Create(portalSettings.Name + " (Site " + siteId + ")");
        settingRepository.AddSetting(new()
        {
            CreatedBy = "2sxc",
            CreatedOn = DateTime.UtcNow,
            EntityId = siteId,
            EntityName = EntityNames.Site,
            ModifiedBy = "2sxc",
            ModifiedOn = DateTime.UtcNow,
            SettingName = SiteSettingNames.SiteKeyForZoneId,
            SettingValue = zoneId.ToString()
        });
        return zoneId;
    }
    
    private bool HasZoneId(int siteId, out int i)
    {
        var c = settingRepository.GetSettings(EntityNames.Site, siteId).ToList();

        var zoneSetting = c.FirstOrDefault(s => s.SettingName == SiteSettingNames.SiteKeyForZoneId);
        if (zoneSetting != null)
        {
            if (!int.TryParse(zoneSetting.SettingValue, out var zId))
            {
                var msg = $"Got value '{zoneSetting.SettingValue}' for ZoneId but can't convert to int";
                Log.A(msg);
                throw new(msg);
            }
            i = zId;
            return true;
        }

        i = 0;
        return false;
    }

    public override ISite SiteOfZone(int zoneId)
    {
        var sites = siteRepository.GetSites().ToList();
        var found = sites.FirstOrDefault(p => HasZoneId(p.SiteId, out var zoneOfSite) && zoneOfSite == zoneId);
        return found != null ? ((OqtSite)site.New()).Init(found) : null;
    }

    public override List<ISiteLanguageState> CulturesWithState(ISite site)
    {
        if (_supportedCultures != null) return _supportedCultures;
        var availableEavLanguages = AppsCatalog.Zone(site.ZoneId).Languages;
        _supportedCultures = oqtCulture.GetSupportedCultures(site.Id, availableEavLanguages);
        return _supportedCultures;
    }
    private List<ISiteLanguageState> _supportedCultures;
}
