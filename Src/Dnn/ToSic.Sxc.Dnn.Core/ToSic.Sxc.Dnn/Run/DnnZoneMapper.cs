using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Context.Sys.ZoneMapper;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnZoneMapper(IAppsCatalog appsCatalog)
    : ServiceBase("DNN.ZoneMp", connect: [appsCatalog]), IZoneMapper
{

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
