using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Logging;
using OqtAlias = Oqtane.Models.Alias;
using OqtPageTemplate = Oqtane.Models.PageTemplate;
using OqtSite = Oqtane.Models.Site;
using OqtSiteGroup = Oqtane.Models.SiteGroup;
using OqtSiteGroupMember = Oqtane.Models.SiteGroupMember;

namespace ToSic.Sxc.Oqt.AppApi;

public class AppApiDynamicRouteValueTransformerTests
{
    [Fact]
    public async Task TransformAsync_UsesPrimarySiteForApiFile_ButCurrentSiteForRouteArea()
    {
        // Arrange
        const int tenantId = 1;
        const int primarySiteId = 10;
        const int secondarySiteId = 20;
        const string appFolder = "Blog";
        const string controller = "Posts";
        var alias = new OqtAlias { TenantId = tenantId, SiteId = secondarySiteId, AliasId = 200 };
        var siteGroup = CreateSiteGroupProvider(
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
        var sut = new AppApiDynamicRouteValueTransformer(
            new FakeTenantResolver(alias),
            new FakeWebHostEnvironment("C:\\Oqtane.Server"),
            siteGroup,
            new LogStoreLive());
        var httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().BuildServiceProvider()
        };
        var values = new RouteValueDictionary
        {
            ["alias"] = "current",
            ["appFolder"] = appFolder,
            ["controller"] = controller,
            ["action"] = "Get"
        };

        // Act
        var result = await sut.TransformAsync(httpContext, values);

        // Assert
        Equal($"{secondarySiteId}/{OqtConstants.ApiAppLinkPart}/{appFolder}/api", result["area"]);
        var apiFile = (string)result["apiFile"]!;
        Contains($"2sxc{Path.DirectorySeparatorChar}Tenants{Path.DirectorySeparatorChar}{tenantId}{Path.DirectorySeparatorChar}Sites{Path.DirectorySeparatorChar}{primarySiteId}", apiFile);
        DoesNotContain($"Sites{Path.DirectorySeparatorChar}{secondarySiteId}", apiFile);
        Equal(result["apiFile"], Path.Combine("C:\\Oqtane.Server", (string)httpContext.Items[SourceCodeConstants.SharedCodeRootPathKeyInCache]!, $"{controller}Controller.cs"));
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

    private sealed class FakeTenantResolver(OqtAlias alias) : ITenantResolver
    {
        public OqtAlias GetAlias() => alias;

        public Tenant GetTenant() => new() { TenantId = alias.TenantId };
    }

    private sealed class FakeWebHostEnvironment(string contentRootPath) : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Test";

        public string ApplicationName { get; set; } = "Test";

        public string WebRootPath { get; set; } = contentRootPath;

        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();

        public string ContentRootPath { get; set; } = contentRootPath;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
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
}
