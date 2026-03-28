using System.Globalization;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
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
    LazySvc<ISiteRepository> siteRepository,
    LazySvc<ISiteGroupMemberRepository> siteGroupMemberRepository)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Cultur",
        connect: [localizationManager, languageRepository, siteRepository, siteGroupMemberRepository])
{
    const string FallbackLanguageCode = "en-us";

    /// <inheritdoc />
    public string DefaultCultureCode => NormalizeCultureCode(localizationManager.Value.GetDefaultCulture());

    // When culture code is not provided for selected default language, use defaultLanguageCode.
    public string DefaultLanguageCode(int siteId)
        => NormalizeCultureCode(languageRepository.Value.GetLanguages(siteId).FirstOrDefault(l => l.IsDefault)?.Code ?? FallbackLanguageCode);

    internal bool IsLocalizationGroupSite(Site? site)
    {
        if (site == null) return false;
        if (site.Languages?.Any(language => !string.IsNullOrWhiteSpace(language.AliasName)) == true)
            return true;

        return GetLocalizationSiteGroupMemberships(site.SiteId).Any();
    }

    internal string GetCurrentContentCulture(Site? site)
        => NormalizeCultureCode(site?.CultureCode ?? CultureInfo.CurrentCulture.Name);

    internal string GetPrimaryContentCulture(Site? site)
    {
        var primaryCulture = GetPrimaryLocalizationSite(site)?.CultureCode ?? site?.CultureCode ?? DefaultCultureCode;
        return NormalizeCultureCode(primaryCulture);
    }

    internal List<ISiteLanguageState> GetSupportedCultures(Site site, List<DimensionDefinition> availableEavLanguages)
    {
        var cultures = IsLocalizationGroupSite(site)
            ? GetLocalizationGroupCultureCodes(site)
            : GetSingleSiteCultureCodes(site);

        AddCodeIfMissing(cultures, GetCurrentContentCulture(site));
        AddCodeIfMissing(cultures, GetPrimaryContentCulture(site));
        if (cultures.Count == 0)
            AddCodeIfMissing(cultures, DefaultCultureCode);

        MoveCodeToFront(cultures, GetPrimaryContentCulture(site));

        // List of localizations enabled in Oqtane site.
        return cultures
            .Select(CultureInfo.GetCultureInfo)
            .Select(c => (ISiteLanguageState)new SiteLanguageState(
                c.Name.ToLowerInvariant(),
                c.EnglishName,
                availableEavLanguages.Any(a => a.Active && a.Matches(c.Name))
            ))
            .ToList();
    }

    private List<string> GetSingleSiteCultureCodes(Site? site)
    {
        var cultures = site?.Languages?
                           .Where(language => !string.IsNullOrWhiteSpace(language.Code))
                           .Select(language => NormalizeCultureCode(language.Code))
                           .Distinct(StringComparer.InvariantCultureIgnoreCase)
                           .ToList()
                       ?? [];

        if (cultures.Count == 0 && site?.SiteId > 0)
        {
            cultures.AddRange(languageRepository.Value.GetLanguages(site.SiteId)
                .Select(language => NormalizeCultureCode(language.Code))
                .Distinct(StringComparer.InvariantCultureIgnoreCase));
        }

        AddCodeIfMissing(cultures, site?.CultureCode);
        return cultures;
    }

    private List<string> GetLocalizationGroupCultureCodes(Site site)
    {
        var cultures = site.Languages?
                           .Where(language => !string.IsNullOrWhiteSpace(language.Code))
                           .Select(language => NormalizeCultureCode(language.Code))
                           .Distinct(StringComparer.InvariantCultureIgnoreCase)
                           .ToList()
                       ?? [];

        if (cultures.Count > 0)
            return cultures;

        var memberships = GetLocalizationSiteGroupMemberships(site.SiteId);
        if (memberships.Count == 0)
            return GetSingleSiteCultureCodes(site);

        var sitesById = siteRepository.Value
            .GetSites()
            .GroupBy(siteInfo => siteInfo.SiteId)
            .ToDictionary(group => group.Key, group => group.First());

        foreach (var siteGroupId in memberships.Select(member => member.SiteGroupId).Distinct())
        {
            foreach (var groupMember in siteGroupMemberRepository.Value.GetSiteGroupMembers(-1, siteGroupId))
            {
                var cultureCode = groupMember.SiteId == site.SiteId
                    ? site.CultureCode
                    : sitesById.TryGetValue(groupMember.SiteId, out var siteInfo)
                        ? siteInfo.CultureCode
                        : null;
                AddCodeIfMissing(cultures, cultureCode);
            }
        }

        return cultures;
    }

    private Site? GetPrimaryLocalizationSite(Site? site)
    {
        if (site == null || !IsLocalizationGroupSite(site))
            return null;

        var primarySiteId = GetLocalizationSiteGroupMemberships(site.SiteId)
            .Select(member => member.SiteGroup?.PrimarySiteId ?? 0)
            .FirstOrDefault(id => id > 0);

        if (primarySiteId <= 0)
            return null;

        if (primarySiteId == site.SiteId)
            return !string.IsNullOrWhiteSpace(site.CultureCode)
                ? site
                : siteRepository.Value.GetSite(primarySiteId, false) ?? site;

        return siteRepository.Value.GetSite(primarySiteId, false);
    }

    private List<SiteGroupMember> GetLocalizationSiteGroupMemberships(int siteId)
    {
        if (siteId <= 0)
            return [];

        return siteGroupMemberRepository.Value
            .GetSiteGroupMembers(siteId, -1)
            .Where(member => member.SiteGroup?.Type == SiteGroupTypes.Localization)
            .OrderBy(member => member.SiteGroupId)
            .ToList();
    }

    private static void AddCodeIfMissing(ICollection<string> cultures, string? cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
            return;

        var normalized = NormalizeCultureCode(cultureCode);
        if (cultures.Contains(normalized, StringComparer.InvariantCultureIgnoreCase))
            return;

        cultures.Add(normalized);
    }

    private static void MoveCodeToFront(List<string> cultures, string? cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
            return;

        var normalized = NormalizeCultureCode(cultureCode);
        var existingIndex = cultures.FindIndex(code => code.Equals(normalized, StringComparison.InvariantCultureIgnoreCase));
        if (existingIndex < 0)
            return;

        var existing = cultures[existingIndex];
        cultures.RemoveAt(existingIndex);
        cultures.Insert(0, existing);
    }

    private static string NormalizeCultureCode(string? culture)
        => MapTwoLetterCulture(culture?.ToLowerInvariant() ?? FallbackLanguageCode).ToLowerInvariant();

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
