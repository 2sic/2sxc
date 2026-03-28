using System.Globalization;
using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Data.Sys.Dimensions;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

/// <summary>
/// Manage oqtane site culture info
/// </summary>
internal class OqtCulture(
    LazySvc<ILocalizationManager> localizationManager,
    LazySvc<ILanguageRepository> languageRepository,
    LazySvc<OqtSiteGroup> siteGroup,
    IHttpContextAccessor httpContextAccessor)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Cultur",
        connect: [localizationManager, languageRepository, siteGroup, httpContextAccessor])
{
    const string FallbackLanguageCode = "en-us";

    /// <inheritdoc />
    public string DefaultCultureCode => NormalizeCultureCode(localizationManager.Value.GetDefaultCulture());

    // When culture code is not provided for selected default language, use defaultLanguageCode.
    public string DefaultLanguageCode(int siteId)
        => NormalizeCultureCode(languageRepository.Value.GetLanguages(siteId).FirstOrDefault(l => l.IsDefault)?.Code ?? FallbackLanguageCode);

    internal string GetCurrentContentCulture(Site site)
        => NormalizeCultureCode(siteGroup.Value.IsLocalizationGroupSite(site) ? site?.CultureCode : CultureInfo.CurrentUICulture.Name);

    internal string GetPrimaryContentCulture(Site site)
        => siteGroup.Value.IsLocalizationGroupSite(site)
            ? NormalizeCultureCode(siteGroup.Value.GetPrimaryLocalizationSite(site)?.CultureCode ?? site?.CultureCode ?? DefaultCultureCode)
            : DefaultLanguageCode(site.SiteId);

    internal List<ISiteLanguageState> GetSupportedCultures(Site site, List<DimensionDefinition> availableEavLanguages)
    {
        var cultures = siteGroup.Value.IsLocalizationGroupSite(site)
            ? siteGroup.Value.GetLocalizationGroupCultureCodes(site) // site is part of a localized site group - get all languages from the site group
            : GetSingleSiteCultureCodes(site); // use site languages

        //AddCodeIfMissing(cultures, GetCurrentContentCulture(site));
        //AddCodeIfMissing(cultures, GetPrimaryContentCulture(site));
        //if (cultures.Count == 0)
        //    AddCodeIfMissing(cultures, DefaultCultureCode);

        // List of localizations enabled in Oqtane site.
        return SiteLanguageStates(availableEavLanguages, cultures, GetPrimaryContentCulture(site));
    }

    private static List<ISiteLanguageState> SiteLanguageStates(List<DimensionDefinition> availableEavLanguages, List<string> cultures, string defaultLanguageCode)
        => cultures
            .Select(CultureInfo.GetCultureInfo)
            .Select(c => (ISiteLanguageState)new SiteLanguageState(
                c.Name.ToLowerInvariant(),
                c.EnglishName,
                availableEavLanguages.Any(a => a.Active && a.Matches(c.Name))
            ))
            .OrderByDescending(c => c.Code == defaultLanguageCode) // make sure the default language is first in the list
            .ToList();

    /// <summary>
    /// Rebuilds cultures for the current site only
    /// </summary>
    /// <param name="site"></param>
    /// <returns></returns>
    private List<string> GetSingleSiteCultureCodes(Site site)
    {
        var cultures = GetCultures(site);

        if (cultures.Count == 0 && site?.SiteId > 0)
        {
            // use site languages
            cultures.AddRange(languageRepository.Value.GetLanguages(site.SiteId)
                .Select(language => NormalizeCultureCode(language.Code))
                .Distinct(StringComparer.InvariantCultureIgnoreCase));
        }

        AddCodeIfMissing(cultures, site?.CultureCode);
        return cultures;
    }

    internal static void AddCodeIfMissing(ICollection<string> cultures, string cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
            return;

        var normalized = NormalizeCultureCode(cultureCode);
        if (cultures.Contains(normalized, StringComparer.InvariantCultureIgnoreCase))
            return;

        cultures.Add(normalized);
    }

    internal static string NormalizeCultureCode(string culture)
        => MapTwoLetterCulture(culture?.ToLowerInvariant() ?? FallbackLanguageCode).ToLowerInvariant();

    internal static List<string> GetCultures(Site site)
        => site?.Languages?
               .Where(language => !string.IsNullOrWhiteSpace(language.Code))
               .Select(language => NormalizeCultureCode(language.Code))
               .Distinct(StringComparer.InvariantCultureIgnoreCase)
               .ToList()
           ?? [];

    internal static void SetCulture(string culture)
    {
        var cultureInfo = CultureInfo.GetCultureInfo(MapTwoLetterCulture(culture));
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }

    private static string MapTwoLetterCulture(string culture)
    {
        if (string.IsNullOrEmpty(culture))
            return FallbackLanguageCode;

        if (culture.Length > 3)
            return culture;

        // 1. For "en" return "en-us".
        if (culture.ToLowerInvariant() == "en")
            return FallbackLanguageCode;

        // 2. For other cultures first find is there simple de-de culture
        var simpleLanguageCode = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .FirstOrDefault(c => c.Name.ToLowerInvariant() == $"{culture}-{culture}");

        if (simpleLanguageCode != null)
            return simpleLanguageCode.Name.ToLowerInvariant();

        // 3. If not, find first in list and return
        var firstLanguageCode = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .OrderBy(c => c.Name)
            .FirstOrDefault(c => c.TwoLetterISOLanguageName.ToLowerInvariant() == culture
                                 && !c.IsNeutralCulture);

        if (firstLanguageCode != null)
            return firstLanguageCode.Name.ToLowerInvariant();

        // 4. Fallback
        return FallbackLanguageCode;
    }
}
