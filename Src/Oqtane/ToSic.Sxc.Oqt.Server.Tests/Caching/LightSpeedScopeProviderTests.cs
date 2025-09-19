using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Oqt.Server.Caching;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Oqt.Data.Tests;

public class LightSpeedScopeProviderTests
{
    private class FakeTenantSiteContextZero : ITenantSiteContext
    {
        public TenantSiteKey Current => new TenantSiteKey(0, 0);
    }

    private class FakeTenantSiteContextNonZero : ITenantSiteContext
    {
        public TenantSiteKey Current => new TenantSiteKey(3, 7);
    }

    [Fact]
    public void Scope_Is_Null_When_SingleTenant()
    {
        var services = new ServiceCollection();
        services.AddTransient<ICacheKeyScopeProvider, OqtCacheKeyScopeProvider>();
        services.AddSingleton<ITenantSiteContext, FakeTenantSiteContextZero>();
        using var sp = services.BuildServiceProvider();
        var scope = sp.GetRequiredService<ICacheKeyScopeProvider>().BuildScopeSegment();
        Null(scope);
    }

    [Fact]
    public void Scope_Has_Tenant_And_Site_When_MultiTenant()
    {
        var services = new ServiceCollection();
        services.AddTransient<ICacheKeyScopeProvider, OqtCacheKeyScopeProvider>();
        services.AddSingleton<ITenantSiteContext, FakeTenantSiteContextNonZero>();
        using var sp = services.BuildServiceProvider();
        var scope = sp.GetRequiredService<ICacheKeyScopeProvider>().BuildScopeSegment();
        Equal("t:3-s:7", scope);
    }
}
