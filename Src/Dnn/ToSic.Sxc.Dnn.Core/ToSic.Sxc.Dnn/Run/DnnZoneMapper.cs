using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.Abstractions.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Cms;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnZoneMapper(LazySvc<DnnSiteFactory> siteFactory /* lazy is very important, otherwise cyclical logs */, IAppsCatalog appsCatalog)
    : ServiceBase("DNN.ZoneMp", connect: [siteFactory, appsCatalog]), IZoneMapper
{

    public ISite SiteOfZone(int zoneId)
    {
        var l = Log.Fn<ISite>($"{zoneId}");
        var portalController = PortalController.Instance;
        var allPortals = portalController.GetPortals();
        l.A($"Sites/Portals Count: {allPortals.Count}");
        var found = allPortals
            .Cast<PortalInfo>()
            .Select(p =>
            {
                var pSettings = portalController.GetPortalSettings(((IPortalInfo)p).PortalId);
                if (!pSettings.TryGetValue(SiteSettingNames.SiteKeyForZoneId, out var portalZoneId))
                    return null;
                if (!int.TryParse(portalZoneId, out var zid))
                    return null;
                return zid == zoneId
                    ? new PortalSettings(p)
                    : null;
            })
            .FirstOrDefault(f => f != null);

        // 2026-10-18 2dm: must check the consequences of this
        // It may have had a side effect till now, updating the site
        // scoped TODO: #VerifySiteFactorySplitIsOk
        return found == null
            ? l.ReturnNull("not found")
            : l.Return(siteFactory.Value.CreateFromPortal(found), $"found {found.PortalId}");
    }

    /// <inheritdoc />
    public List<ISiteLanguageState> CulturesWithState(ISite site)
    {
        if (_supportedCultures != null)
            return _supportedCultures;

        var availableEavLanguages = appsCatalog.Zone(site.ZoneId).Languages;
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
