using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Shared;
using OqtAlias = Oqtane.Models.Alias;
using OqtPageTemplate = Oqtane.Models.PageTemplate;
using OqtSite = Oqtane.Models.Site;
using OqtSiteGroup = Oqtane.Models.SiteGroup;
using OqtSiteGroupMember = Oqtane.Models.SiteGroupMember;

namespace ToSic.Sxc.Oqt.Adam;

public class OqtAssetsFileHelperTests : IDisposable
{
    private readonly string _contentRoot = Path.Combine(Path.GetTempPath(), $"{nameof(OqtAssetsFileHelperTests)}-{Guid.NewGuid():N}");

    [Fact]
    public void GetFilePath_UsesPrimarySite_ForLocalizationGroup()
    {
        // Arrange
        const int tenantId = 1;
        const int primarySiteId = 10;
        const int secondarySiteId = 20;
        const string appName = "Blog";
        const string filePath = "logo.png";
        var primaryFilePath = Path.GetFullPath(Path.Combine(
            _contentRoot,
            string.Format(OqtConstants.AppRootTenantSiteBase, tenantId, primarySiteId),
            appName,
            filePath));
        Directory.CreateDirectory(Path.GetDirectoryName(primaryFilePath)!);
        File.WriteAllText(primaryFilePath, "primary");

        var siteGroupProvider = CreateSiteGroupProvider(
            sites:
            [
                Site(primarySiteId, "en"),
                Site(secondarySiteId, "de")
            ],
            siteGroupMembers:
            [
                SiteGroupMember(primarySiteId, 1, primarySiteId),
                SiteGroupMember(secondarySiteId, 1, primarySiteId)
            ]);
        var sut = new OqtAssetsFileHelper(siteGroupProvider);
        var alias = new OqtAlias { TenantId = tenantId, SiteId = secondarySiteId };

        // Act
        var result = sut.GetFilePath(_contentRoot, alias, OqtAssetsFileHelper.RouteAssets, appName, filePath);

        // Assert
        Equal(primaryFilePath, result);
    }

    [Fact]
    public void GetFilePath_RejectsHiddenFolder_WithMixedSeparators()
    {
        var sut = new OqtAssetsFileHelper();
        var alias = new OqtAlias { TenantId = 1, SiteId = 2 };

        var result = sut.GetFilePath(_contentRoot, alias, OqtAssetsFileHelper.RouteAssets, "Blog", @"images\.private/logo.png");

        Empty(result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_contentRoot))
            Directory.Delete(_contentRoot, recursive: true);
    }

    private static Server.Context.OqtSiteGroup CreateSiteGroupProvider(IEnumerable<OqtSite> sites, IEnumerable<OqtSiteGroupMember> siteGroupMembers)
        => new(
            new FakeSiteRepository(sites),
            new FakeSiteGroupMemberRepository(siteGroupMembers)
        );

    private static OqtSite Site(int siteId, string cultureCode)
        => new()
        {
            SiteId = siteId,
            CultureCode = cultureCode,
            Languages = [],
            Name = $"Site {siteId}",
            Settings = [],
            Pages = [],
            Themes = []
        };

    private static OqtSiteGroupMember SiteGroupMember(int siteId, int siteGroupId, int primarySiteId)
        => new()
        {
            SiteId = siteId,
            SiteGroupId = siteGroupId,
            SiteGroup = new OqtSiteGroup
            {
                SiteGroupId = siteGroupId,
                PrimarySiteId = primarySiteId,
                Type = SiteGroupTypes.Localization,
                Name = $"Group {siteGroupId}"
            }
        };

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
}
