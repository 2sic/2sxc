using Oqtane.Infrastructure;
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
    OqtCulture oqtCulture,
    OqtSiteGroup oqtSiteGroup,
    IAppsCatalog appsCat)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.ZoneMp",
        connect: [siteRepository, settingRepository, site, oqtCulture, oqtSiteGroup, appsCat]), IZoneMapper
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

    public List<ISiteLanguageState> CulturesWithState(ISite oqtSite)
    {
        if (_supportedCultures != null)
            return _supportedCultures;
        var availableEavLanguages = appsCat.Zone(oqtSite.ZoneId).Languages;
        var oqtaneSite = (oqtSite as IWrapper<Oqtane.Models.Site>)?.GetContents() ?? siteRepository.GetSite(oqtSite.Id);
        _supportedCultures = oqtaneSite == null
            ? []
            : oqtCulture.GetSupportedCultures(oqtaneSite, availableEavLanguages);
        return _supportedCultures;
    }
    private List<ISiteLanguageState> _supportedCultures;
}
