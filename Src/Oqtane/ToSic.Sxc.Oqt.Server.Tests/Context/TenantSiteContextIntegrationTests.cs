using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Oqt.Context.Tests;

public class TenantSiteContextIntegrationTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

    // Enable LazySvc<T> resolution used by core services
    services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));

        // Minimal Oqtane plumbing for AliasResolver
        services.AddSingleton<SiteState>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAliasAccessor, AliasAccessor>();

        // Supply repositories via simple fakes for this test
        services.AddSingleton<IAliasRepository, FakeAliasRepository>();
        services.AddSingleton<ITenantManager, FakeTenantManager>();

        services.AddScoped<AliasResolver>();
        services.AddScoped<ITenantSiteContext, OqtTenantSiteContext>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Current_Uses_SiteState_Alias_When_Present()
    {
        using var provider = BuildProvider();
        var state = provider.GetRequiredService<SiteState>();
        state.Alias = new Alias { AliasId = 1, SiteId = 11, TenantId = 5, Name = "example.localhost" };

        var ctx = provider.GetRequiredService<ITenantSiteContext>();
        TenantSiteKey key = ctx.Current;
        Equal(5, key.TenantId);
        Equal(11, key.SiteId);
    }

    private class AliasAccessor : IAliasAccessor
    {
        public Alias? Alias { get; set; }
    }

    private class FakeAliasRepository : IAliasRepository
    {
        public IEnumerable<Alias> GetAliases() => new List<Alias>
        {
            new Alias { AliasId = 1, SiteId = 11, TenantId = 5, Name = "example.localhost" }
        };

        public Alias GetAlias(int aliasid) => new Alias { AliasId = aliasid, SiteId = 11, TenantId = 5, Name = "example.localhost" };
        public Alias GetAlias(int aliasid, bool tracking = true) => new Alias { AliasId = 1, SiteId = 11, TenantId = 5, Name = "example.localhost" };
        public Alias GetAlias(string name) => new Alias { AliasId = 1, SiteId = 11, TenantId = 5, Name = name };
        public Alias AddAlias(Alias alias) => alias;
        public Alias UpdateAlias(Alias alias) => alias;
        public void DeleteAlias(int aliasid) {}
    }

    private class FakeTenantManager : ITenantManager
    {
        public Alias GetAlias() => new Alias { AliasId = 1, SiteId = 11, TenantId = 5, Name = "example.localhost" };
        public string SitePath(int siteId) => string.Empty;
        public string GetAlias(string url) => string.Empty;
        public string AddAlias(string url) => string.Empty;
        public string DeleteAlias(string url) => string.Empty;
        public string CloneAlias(string from, string to) => string.Empty;
        public Tenant GetTenant() => new Tenant { TenantId = 1 };
        public void SetAlias(Alias alias) {}
        public void SetAlias(int tenantId, int siteId) {}
        public void SetTenant(int tenantId) {}
    }
}
