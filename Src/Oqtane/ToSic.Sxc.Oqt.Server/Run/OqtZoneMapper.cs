using Oqtane.Repository;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtZoneMapper(ISiteRepository siteRepository, OqtCulture oqtCulture, IAppsCatalog appsCat)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.ZoneMp", connect: [siteRepository, oqtCulture, appsCat]), IZoneMapper
{

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
