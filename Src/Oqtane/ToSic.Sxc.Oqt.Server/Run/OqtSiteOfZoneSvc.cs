using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Cms;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtSiteOfZoneSvc(ISiteRepository siteRepository, ISettingRepository settingRepository, Generator<ISite> site, OqtSiteGroup oqtSiteGroup)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.ZoneMp",
        connect: [siteRepository, settingRepository, site, oqtSiteGroup]), ISiteOfZoneSvc
{

    public ISite SiteOfZone(int zoneId)
    {
        var siteIdsWithZoneId = settingRepository.GetSettings(EntityNames.Site)
            .Where(setting => setting.SettingName == SiteSettingNames.SiteKeyForZoneId && setting.SettingValue == zoneId.ToString())
            .Select(setting => setting.EntityId);

        var sites = siteRepository.GetSites()
            .Where(s => siteIdsWithZoneId.Contains(s.SiteId))
            .ToList();

        // ReSharper disable once AssignNullToNotNullAttribute
        return sites.Count switch
        {
            0 => (ISite)null,
            1 => ((OqtSite)site.New()).Init(sites.Single()),
            _ => ((OqtSite)site.New()).Init(oqtSiteGroup.GetPrimaryLocalizationSite(sites.First()))
        };
    }

}
