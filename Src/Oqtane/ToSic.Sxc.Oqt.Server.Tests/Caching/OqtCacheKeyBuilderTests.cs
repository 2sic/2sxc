using ToSic.Sxc.Oqt.Server.Caching;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Caching.Tests;

public class OqtCacheKeyBuilderTests
{
    [Fact]
    public void Build_With_Key_Includes_TenantSite()
    {
        var b = new OqtCacheKeyBuilder();
        var key = new TenantSiteKey(3, 7);
        var result = b.Build("ns", new[] { "a", "b" }, key);
        Equal("ns|t:3/s:7|a~b", result);
    }

    [Fact]
    public void Build_Without_Key_Just_Namespace_And_Parts()
    {
        var b = new OqtCacheKeyBuilder();
        var result = b.Build("ns", new[] { "a" });
        Equal("ns|a", result);
    }
}
