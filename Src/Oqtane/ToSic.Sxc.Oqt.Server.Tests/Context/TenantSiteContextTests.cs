using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Context.Tests;

public class TenantSiteContextTests
{
    private class FakeTenantSiteContext : ITenantSiteContext
    {
        public TenantSiteKey Current => new TenantSiteKey(3, 7);
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITenantSiteContext, FakeTenantSiteContext>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Current_Returns_Key_With_Deterministic_Format()
    {
        using var provider = BuildProvider();
        var ctx = provider.GetRequiredService<ITenantSiteContext>();
        TenantSiteKey key = ctx.Current;

        Assert.Equal(3, key.TenantId);
        Assert.Equal(7, key.SiteId);
        Assert.Equal("t:3/s:7", key.ToString());
    }
}
