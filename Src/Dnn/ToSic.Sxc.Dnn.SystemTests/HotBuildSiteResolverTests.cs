using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sys.Logging;
using Xunit;

namespace ToSic.Sxc.Dnn;

public class HotBuildSiteResolverTests
{
    [Fact]
    public void ResolveForApp_KeepsCurrentSite_WhenZoneMatches()
    {
        var currentSite = new TestSite(id: 7, zoneId: 11);
        var zoneMapper = new TrackingZoneMapper(currentSite);

        var result = HotBuildSiteResolver.ResolveForApp(currentSite, new AppIdentity(11, 42), zoneMapper);

        Same(currentSite, result);
        Equal(1, zoneMapper.SiteOfAppCalls);
        Equal(42, zoneMapper.LastAppId);
    }

    [Fact]
    public void ResolveForApp_UsesMappedSite_WhenZoneMatchesButSiteDiffers()
    {
        var currentSite = new TestSite(id: 7, zoneId: 11);
        var mappedSite = new TestSite(id: 9, zoneId: 11);
        var zoneMapper = new TrackingZoneMapper(mappedSite);

        var result = HotBuildSiteResolver.ResolveForApp(currentSite, new AppIdentity(11, 42), zoneMapper);

        Same(mappedSite, result);
        Equal(1, zoneMapper.SiteOfAppCalls);
        Equal(42, zoneMapper.LastAppId);
    }

    [Fact]
    public void ResolveForApp_UsesMappedSite_WhenZoneDiffers()
    {
        var currentSite = new TestSite(id: 7, zoneId: 11);
        var mappedSite = new TestSite(id: 9, zoneId: 22);
        var zoneMapper = new TrackingZoneMapper(mappedSite);

        var result = HotBuildSiteResolver.ResolveForApp(currentSite, new AppIdentity(22, 42), zoneMapper);

        Same(mappedSite, result);
        Equal(1, zoneMapper.SiteOfAppCalls);
        Equal(42, zoneMapper.LastAppId);
    }

    private sealed class TrackingZoneMapper(ISite mappedSite) : IZoneMapper
    {
        public ILog? Log => null;

        public int SiteOfAppCalls { get; private set; }

        public int LastAppId { get; private set; }

        public int GetZoneId(int siteId) => mappedSite.ZoneId;

        public ISite SiteOfZone(int zoneId) => mappedSite;

        public ISite SiteOfApp(int appId)
        {
            SiteOfAppCalls++;
            LastAppId = appId;
            return mappedSite;
        }

        public List<ISiteLanguageState> CulturesWithState(ISite site) => [];

        public List<ISiteLanguageState> CulturesEnabledWithState(ISite site) => [];
    }

    private sealed class TestSite(int id, int zoneId) : ISite
    {
        public ISite Init(int siteId, ILog? parentLogOrNull) => this;

        public int Id => id;

        public string DefaultLanguage => "en-us";

        public string Name => $"site-{id}";

        public string Url => $"https://site-{id}.example";

        public string UrlRoot => $"site-{id}.example";

        public string AppsRootPhysical => $"/Portals/{id}/2sxc";

        public string AppsRootPhysicalFull => $@"C:\Portals\{id}\2sxc";

        public string AppAssetsLinkTemplate => $"/Portals/{id}/2sxc/[AppFolder]";

        public string ContentPath => $"/Portals/{id}";

        public int ZoneId => zoneId;

        public string CurrentCultureCode => "en-us";

        public string DefaultCultureCode => "en-us";
    }
}
