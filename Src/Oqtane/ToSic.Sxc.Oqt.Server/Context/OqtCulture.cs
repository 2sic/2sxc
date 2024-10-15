using System.Globalization;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using ToSic.Eav.Cms.Internal.Languages;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

/// <summary>
/// Manage oqtane site culture info
/// </summary>
internal class OqtCulture(
    LazySvc<ILocalizationManager> localizationManager,
    LazySvc<ILanguageRepository> languageRepository)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Cultur", connect: [localizationManager, languageRepository])
{
    const string FallbackLanguageCode = "en-us";

    /// <inheritdoc />
    public string DefaultCultureCode => MapTwoLetterCulture(localizationManager.Value.GetDefaultCulture().ToLowerInvariant()) ?? FallbackLanguageCode;

    // When culture code is not provided for selected default language, use defaultLanguageCode.
    public string DefaultLanguageCode(int siteId)
    {
        return (languageRepository.Value.GetLanguages(siteId).FirstOrDefault(l => l.IsDefault)?.Code ?? FallbackLanguageCode).ToLowerInvariant();
    }

    public string CurrentCultureCode => MapTwoLetterCulture(CultureInfo.CurrentCulture.Name).ToLowerInvariant();

    public List<ISiteLanguageState> GetSupportedCultures(int siteId, List<Eav.Data.DimensionDefinition>  availableEavLanguages)
    {
        var cultures = new List<string>(new[] { DefaultCultureCode });
        cultures.AddRange(languageRepository.Value.GetLanguages(siteId).Select(language => MapTwoLetterCulture(language.Code)));

        // List of localizations enabled in Oqtane site.
        var siteCultures = cultures
            .Select(CultureInfo.GetCultureInfo)
            .Select(c => new SiteLanguageState(
                c.Name.ToLowerInvariant(), 
                c.EnglishName, 
                availableEavLanguages.Any(a => a.Active && a.Matches(c.Name))
            ))
            .ToList();
            
        return siteCultures
            .OrderByDescending(c => c.Code == DefaultLanguageCode(siteId))
            .Cast<ISiteLanguageState>()
            .ToList();
    }

    public static void SetCulture(string culture)
    {
        var cultureInfo = CultureInfo.GetCultureInfo(MapTwoLetterCulture(culture));
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }

    public static string MapTwoLetterCulture(string culture)
    {
        if (string.IsNullOrEmpty(culture)) return FallbackLanguageCode;

        if (culture.Length > 3) return culture;

        // 1. For "en" return "en-us".
        if (culture.ToLowerInvariant() == "en") return FallbackLanguageCode;

        // 2. For other cultures first find is there simple de-de culture
        var simpleLanguageCode = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .FirstOrDefault(c => c.Name.ToLowerInvariant() == $"{culture}-{culture}");

        if (simpleLanguageCode != null) return simpleLanguageCode.Name.ToLowerInvariant();

        // 3. If not, find first in list and return
        var firstLanguageCode = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .OrderBy(c => c.Name)
            .FirstOrDefault(c => c.TwoLetterISOLanguageName.ToLowerInvariant() == culture 
                                 && c.IsNeutralCulture == false);

        if (firstLanguageCode != null) return firstLanguageCode.Name.ToLowerInvariant();

        // 4. Fallback
        return FallbackLanguageCode;
    }
}