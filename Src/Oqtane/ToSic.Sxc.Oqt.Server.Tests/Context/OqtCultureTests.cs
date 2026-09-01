using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sys.DI;
using DimensionDefinition = ToSic.Eav.Data.Sys.Dimensions.DimensionDefinition;
using OqtAlias = Oqtane.Models.Alias;
using OqtLanguage = Oqtane.Models.Language;
using OqtPageTemplate = Oqtane.Models.PageTemplate;
using OqtSite = Oqtane.Models.Site;
using OqtSiteGroup = Oqtane.Models.SiteGroup;
using OqtSiteGroupMember = Oqtane.Models.SiteGroupMember;

namespace ToSic.Sxc.Oqt.Context;

public class OqtCultureTests
{
    [Fact]
    public void IsLocalizationGroupSite_DoesNotUseAliasNameSignal()
    {
        var sut = CreateSiteGroupProvider();
        var site = Site(2, "de", [
            Language("de", aliasName: "secondary.example.com"),
            Language("en", aliasName: "primary.example.com")
        ]);

        False(sut.IsLocalizationGroupSite(site));
    }

    [Fact]
    public void IsLocalizationGroupSite_UsesLocalizationSiteGroupMembership()
    {
        var sut = CreateSiteGroupProvider(
            siteGroupMembers:
            [
                SiteGroupMember(2, 1, 1, SiteGroupTypes.Localization)
            ]);
        var site = Site(2, "de");

        True(sut.IsLocalizationGroupSite(site));
    }

    [Fact]
    public void GetCurrentContentCulture_UsesThreadUiCulture_WhenNotInLocalizationGroup()
    {
        using var culture = new CultureScope("en-US", "nl-NL");
        var sut = CreateSut();
        var site = Site(2, "de");

        Equal("nl-nl", sut.GetCurrentContentCulture(site));
    }

    [Fact]
    public void GetCurrentContentCulture_UsesSiteCultureCode_ForLocalizationGroup()
    {
        using var culture = new CultureScope("es-ES");
        var sut = CreateSut(
            siteGroupMembers:
            [
                SiteGroupMember(2, 1, 1, SiteGroupTypes.Localization)
            ]);
        var site = Site(2, "de");

        Equal("de-de", sut.GetCurrentContentCulture(site));
    }

    [Fact]
    public void GetPrimaryContentCulture_UsesDefaultCulture_WhenNotInLocalizationGroup()
    {
        var sut = CreateSut(localizationManager: new FakeLocalizationManager("en"));
        var site = Site(2, "de");

        Equal("en-us", sut.GetPrimaryContentCulture(site));
    }

    [Fact]
    public void GetPrimaryContentCulture_UsesPrimarySiteCulture_ForLocalizationGroup()
    {
        var sut = CreateSut(
            sites:
            [
                Site(1, "en"),
                Site(2, "de", [Language("de", aliasName: "secondary.example.com")])
            ],
            siteGroupMembers:
            [
                SiteGroupMember(2, 1, 1, SiteGroupTypes.Localization),
                SiteGroupMember(1, 1, 1, SiteGroupTypes.Localization)
            ]);
        var site = Site(2, "de", [Language("de", aliasName: "secondary.example.com")]);

        Equal("en-us", sut.GetPrimaryContentCulture(site));
    }

    [Fact]
    public void GetPrimaryLocalizationSiteId_UsesPrimarySite_ForLocalizationGroup()
    {
        var sut = CreateSiteGroupProvider(
            sites:
            [
                Site(1, "en"),
                Site(2, "de")
            ],
            siteGroupMembers:
            [
                SiteGroupMember(2, 1, 1, SiteGroupTypes.Localization),
                SiteGroupMember(1, 1, 1, SiteGroupTypes.Localization)
            ]);

        Equal(1, sut.GetPrimaryLocalizationSiteId(2));
    }

    [Fact]
    public void GetPrimaryLocalizationSiteId_UsesCurrentSite_WhenNotLocalizationGroup()
    {
        var sut = CreateSiteGroupProvider(sites: [Site(2, "de")]);

        Equal(2, sut.GetPrimaryLocalizationSiteId(2));
    }

    [Fact]
    public void GetSupportedCultures_UsesHydratedGroupLanguages_WhenAvailable()
    {
        var sut = CreateSut(
            sites:
            [
                Site(1, "en"),
                Site(2, "de"),
                Site(3, "fr")
            ],
            siteGroupMembers:
            [
                SiteGroupMember(2, 1, 1, SiteGroupTypes.Localization),
                SiteGroupMember(1, 1, 1, SiteGroupTypes.Localization),
                SiteGroupMember(3, 1, 1, SiteGroupTypes.Localization)
            ]);
        var site = Site(2, "de",
        [
            Language("de", aliasName: "secondary.example.com"),
            Language("en", aliasName: "primary.example.com"),
            Language("fr", aliasName: "third.example.com")
        ]);

        var cultures = sut.GetSupportedCultures(site, [Dimension("en-us"), Dimension("de-de")]);

        Equal(["en-us", "de-de", "fr-fr"], cultures.Select(c => c.Code).ToArray());
        True(cultures.Single(c => c.Code == "en-us").IsEnabled);
        True(cultures.Single(c => c.Code == "de-de").IsEnabled);
        False(cultures.Single(c => c.Code == "fr-fr").IsEnabled);
    }

    [Fact]
    public void GetSupportedCultures_FallsBackToRepositories_WhenLocalizationLanguagesMissing()
    {
        var sut = CreateSut(
            sites:
            [
                Site(1, "en"),
                Site(2, "de"),
                Site(3, "fr")
            ],
            siteGroupMembers:
            [
                SiteGroupMember(2, 1, 1, SiteGroupTypes.Localization),
                SiteGroupMember(1, 1, 1, SiteGroupTypes.Localization),
                SiteGroupMember(3, 1, 1, SiteGroupTypes.Localization)
            ]);
        var site = Site(2, "de");

        var cultures = sut.GetSupportedCultures(site, [Dimension("en-us"), Dimension("fr-fr")]);

        Equal(["en-us", "de-de", "fr-fr"], cultures.Select(c => c.Code).ToArray());
        False(cultures.Single(c => c.Code == "de-de").IsEnabled);
    }

    [Fact]
    public void GetSupportedCultures_UsesSiteLanguagesAndSiteCulture_WhenNotInLocalizationGroup()
    {
        using var culture = new CultureScope("fr-FR");
        var sut = CreateSut(languageRepository: new FakeLanguageRepository(new Dictionary<int, List<OqtLanguage>>
        {
            [5] = [Language("es"), Language("it", isDefault: true)]
        }));
        var site = Site(5, "de");

        var cultures = sut.GetSupportedCultures(site, [Dimension("it-it")]);

        Equal(["it-it", "es-es", "de-de"], cultures.Select(c => c.Code).ToArray());
        True(cultures.Single(c => c.Code == "it-it").IsEnabled);
        False(cultures.Single(c => c.Code == "es-es").IsEnabled);
        False(cultures.Single(c => c.Code == "de-de").IsEnabled);
        DoesNotContain(cultures, c => c.Code == "fr-fr");
    }

    private static OqtCulture CreateSut(
        IEnumerable<OqtSite>? sites = null,
        IEnumerable<OqtSiteGroupMember>? siteGroupMembers = null,
        ILanguageRepository? languageRepository = null,
        ILocalizationManager? localizationManager = null,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        localizationManager ??= new FakeLocalizationManager("en");
        languageRepository ??= new FakeLanguageRepository();
        httpContextAccessor ??= new HttpContextAccessor();
        var siteGroupProvider = CreateSiteGroupProvider(sites, siteGroupMembers);

        return new(
            CreateLazy(localizationManager),
            CreateLazy(languageRepository),
            CreateLazy(siteGroupProvider),
            httpContextAccessor
        );
    }

    private static Server.Context.OqtSiteGroup CreateSiteGroupProvider(
        IEnumerable<OqtSite>? sites = null,
        IEnumerable<OqtSiteGroupMember>? siteGroupMembers = null)
        => new(
            new FakeSiteRepository(sites ?? []),
            new FakeSiteGroupMemberRepository(siteGroupMembers ?? [])
        );

    private static readonly ServiceProvider EmptyServices = new ServiceCollection().BuildServiceProvider();

    private static LazySvc<T> CreateLazy<T>(T service)
        where T : class
    {
        var lazy = new LazySvc<T>(EmptyServices);
        lazy.Inject(service);
        return lazy;
    }

    private static OqtSite Site(int siteId, string cultureCode, List<OqtLanguage>? languages = null)
        => new()
        {
            SiteId = siteId,
            CultureCode = cultureCode,
            Languages = languages ?? [],
            Name = $"Site {siteId}",
            Settings = [],
            Pages = [],
            Themes = []
        };

    private static OqtLanguage Language(string code, bool isDefault = false, string? aliasName = null)
        => new()
        {
            Code = code,
            IsDefault = isDefault,
            AliasName = aliasName ?? "",
            Name = code
        };

    private static OqtSiteGroupMember SiteGroupMember(int siteId, int siteGroupId, int primarySiteId, string type)
        => new()
        {
            SiteId = siteId,
            SiteGroupId = siteGroupId,
            SiteGroup = new OqtSiteGroup
            {
                SiteGroupId = siteGroupId,
                PrimarySiteId = primarySiteId,
                Type = type,
                Name = $"Group {siteGroupId}"
            }
        };

    private static DimensionDefinition Dimension(string key, bool active = true)
        => new()
        {
            Name = key,
            Key = key.ToLowerInvariant(),
            EnvironmentKey = key.ToLowerInvariant(),
            Active = active
        };

    private sealed class FakeLocalizationManager(string defaultCulture) : ILocalizationManager
    {
        public string GetDefaultCulture() => defaultCulture;

        public string[] GetSupportedCultures() => [];

        public string[] GetInstalledCultures() => [];

        public string[] GetNeutralCultures() => [];
    }

    private sealed class FakeLanguageRepository(Dictionary<int, List<OqtLanguage>>? languagesBySiteId = null) : ILanguageRepository
    {
        private readonly Dictionary<int, List<OqtLanguage>> _languagesBySiteId = languagesBySiteId ?? [];

        public IEnumerable<OqtLanguage> GetLanguages(int siteId)
            => _languagesBySiteId.TryGetValue(siteId, out var languages) ? languages : [];

        public OqtLanguage AddLanguage(OqtLanguage language) => throw new NotSupportedException();

        public void UpdateLanguage(OqtLanguage language) => throw new NotSupportedException();

        public OqtLanguage GetLanguage(int languageId) => throw new NotSupportedException();

        public void DeleteLanguage(int languageId) => throw new NotSupportedException();
    }

    private sealed class FakeSiteRepository(IEnumerable<OqtSite> sites) : ISiteRepository
    {
        private readonly Dictionary<int, OqtSite> _sites = sites.ToDictionary(site => site.SiteId);

        public IEnumerable<OqtSite> GetSites() => _sites.Values;

        public OqtSite AddSite(OqtSite site) => throw new NotSupportedException();

        public OqtSite UpdateSite(OqtSite site) => throw new NotSupportedException();

        public OqtSite GetSite(int siteId) => _sites.TryGetValue(siteId, out var site) ? site : null!;

        public OqtSite GetSite(int siteId, bool tracking) => GetSite(siteId);

        public void DeleteSite(int siteId) => throw new NotSupportedException();

        public void InitializeSite(OqtAlias alias) => throw new NotSupportedException();

        public void CreatePages(OqtSite site, List<OqtPageTemplate> pageTemplates, OqtAlias alias) => throw new NotSupportedException();
    }

    private sealed class FakeSiteGroupMemberRepository(IEnumerable<OqtSiteGroupMember> members) : ISiteGroupMemberRepository
    {
        private readonly List<OqtSiteGroupMember> _members = members.ToList();

        public IEnumerable<OqtSiteGroupMember> GetSiteGroupMembers()
            => _members;

        public IEnumerable<OqtSiteGroupMember> GetSiteGroupMembers(int siteId, int siteGroupId)
            => _members.Where(member =>
                (siteId == -1 || member.SiteId == siteId) &&
                (siteGroupId == -1 || member.SiteGroupId == siteGroupId));

        public OqtSiteGroupMember AddSiteGroupMember(OqtSiteGroupMember siteGroupMember) => throw new NotSupportedException();

        public OqtSiteGroupMember UpdateSiteGroupMember(OqtSiteGroupMember siteGroupMember) => throw new NotSupportedException();

        public OqtSiteGroupMember GetSiteGroupMember(int siteGroupMemberId) => throw new NotSupportedException();

        public OqtSiteGroupMember GetSiteGroupMember(int siteGroupMemberId, bool tracking) => throw new NotSupportedException();

        public void DeleteSiteGroupMember(int siteGroupMemberId) => throw new NotSupportedException();
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _originalUiCulture = CultureInfo.CurrentUICulture;

        public CultureScope(string culture, string? uiCulture = null)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(uiCulture ?? culture);
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUiCulture;
        }
    }
}
