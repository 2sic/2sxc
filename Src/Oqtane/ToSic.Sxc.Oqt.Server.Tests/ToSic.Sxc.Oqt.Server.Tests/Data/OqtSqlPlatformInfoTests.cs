using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using ToSic.Eav.DataSources.Sys;
using ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Oqt.Data.Tests;

public class OqtSqlPlatformInfoTests
{
    private class FakeTenantSiteContext : ITenantSiteContext
    {
        public TenantSiteKey Current => new TenantSiteKey(9, 42);
    }

    private class FakeTenantSiteContextZero : ITenantSiteContext
    {
        public TenantSiteKey Current => new TenantSiteKey(0, 0);
    }

    private class FakeSelector : IConnectionSelector
    {
        public string? GetConnectionString(TenantSiteKey key)
            => key.TenantId == 9 && key.SiteId == 42 ? "Server=(local);Database=TenantDb;Trusted_Connection=True;" : null;
    }

    private class FakeConfigManager : IConfigManager
    {
        private readonly Dictionary<string, string> _settings = new();

        public FakeConfigManager()
        {
            // Seed platform default connection to simulate real environment
            _settings["ConnectionStrings:DefaultConnection"] = "Server=(local);Database=DefaultDb;Trusted_Connection=True;";
        }

        public string GetSetting(string settingname, string defaultvalue)
            => _settings.TryGetValue(settingname, out var val) ? val : defaultvalue;

        public void SetSetting(string settingname, string settingvalue, bool ispublic)
            => _settings[settingname] = settingvalue;

        public Microsoft.Extensions.Configuration.IConfigurationSection GetSection(string key)
            => new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build().GetSection(key);

        public T GetSetting<T>(string settingname, T defaultvalue)
            => _settings.TryGetValue(settingname, out var val) && val is T tVal ? tVal : defaultvalue;

        public T GetSetting<T>(string settingname, string tenant, T defaultvalue)
            => defaultvalue;

        public Dictionary<string, string> GetSettings(string settingname)
            => new(_settings);

        public void AddOrUpdateSetting<T>(string settingname, T settingvalue, bool ispublic)
            => _settings[settingname] = settingvalue?.ToString() ?? string.Empty;

        public void AddOrUpdateSetting<T>(string settingname, string tenant, T settingvalue, bool ispublic)
            => _settings[$"{settingname}:{tenant}"] = settingvalue?.ToString() ?? string.Empty;

        public void RemoveSetting(string settingname, bool ispublic)
            => _settings.Remove(settingname);

        public void RemoveSetting(string settingname, string tenant, bool ispublic)
            => _settings.Remove($"{settingname}:{tenant}");

        public void Reload() { }

        public string GetConnectionString() => GetSetting("ConnectionStrings:DefaultConnection", "");
        public string GetConnectionString(string tenant) => GetSetting($"ConnectionStrings:{tenant}:DefaultConnection", "");
        public bool IsInstalled() => true;
        public string GetInstallationId() => "test-installation";
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        services.AddSingleton<IConfigManager, FakeConfigManager>();
        services.AddSingleton<ITenantSiteContext, FakeTenantSiteContext>();
        services.AddSingleton<IConnectionSelector, FakeSelector>();
        services.AddSingleton<SqlPlatformInfo, OqtSqlPlatformInfo>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void DefaultConnection_Uses_Tenant_Override_When_Available()
    {
        using var provider = BuildProvider();
        var spi = provider.GetRequiredService<SqlPlatformInfo>();
        var conn = spi.FindConnectionString(spi.DefaultConnectionStringName);
        Equal("Server=(local);Database=TenantDb;Trusted_Connection=True;", conn);
    }

    [Fact]
    public void DefaultConnection_Falls_Back_To_Platform_Default()
    {
    var services = new ServiceCollection();
    services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
    services.AddSingleton<IConfigManager, FakeConfigManager>();
    services.AddSingleton<ITenantSiteContext>(new FakeTenantSiteContext());
    services.AddSingleton<IConnectionSelector>(new FakeSelectorNoMatch());
    services.AddSingleton<SqlPlatformInfo, OqtSqlPlatformInfo>();
    using var provider = services.BuildServiceProvider();

        var spi = provider.GetRequiredService<SqlPlatformInfo>();
        var conn = spi.FindConnectionString(spi.DefaultConnectionStringName);
        Equal("Server=(local);Database=DefaultDb;Trusted_Connection=True;", conn);
    }

    private class FakeSelectorNoMatch : IConnectionSelector
    {
        public string? GetConnectionString(TenantSiteKey key) => null;
    }

    [Fact]
    public void DefaultConnection_SingleTenant_ZeroIds_Uses_Platform_Default()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        services.AddSingleton<IConfigManager, FakeConfigManager>();
        services.AddSingleton<ITenantSiteContext, FakeTenantSiteContextZero>();
        services.AddSingleton<IConnectionSelector, FakeSelectorNoMatch>();
        services.AddSingleton<SqlPlatformInfo, OqtSqlPlatformInfo>();
        using var provider = services.BuildServiceProvider();

        var spi = provider.GetRequiredService<SqlPlatformInfo>();
        var conn = spi.FindConnectionString(spi.DefaultConnectionStringName);
        Equal("Server=(local);Database=DefaultDb;Trusted_Connection=True;", conn);
    }

    [Fact]
    public void NamedConnection_Uses_Site_Then_Tenant_Then_Platform()
    {
        // Arrange a config manager with platform and tenant/site named connections
        var cfg = new FakeConfigManager();
    var services = new ServiceCollection();
    services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        services.AddSingleton<IConfigManager>(cfg);
        services.AddSingleton<ITenantSiteContext, FakeTenantSiteContext>();
        services.AddSingleton<IConnectionSelector, FakeSelectorNoMatch>();
        services.AddSingleton<SqlPlatformInfo, OqtSqlPlatformInfo>();

        // Only platform level first
        using (var provider = services.BuildServiceProvider())
        {
            var spi = provider.GetRequiredService<SqlPlatformInfo>();
            var name = "ReportingConnection";
            var cm = provider.GetRequiredService<IConfigManager>();
            cm.AddOrUpdateSetting($"ConnectionStrings:{name}", "PlatformNamed", false);
            Equal("PlatformNamed", spi.FindConnectionString(name));
        }

        // Add tenant-level override
    var services2 = new ServiceCollection();
    services2.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        var cfg2 = new FakeConfigManager();
        cfg2.SetSetting("ConnectionStrings:ReportingConnection", "PlatformNamed", false);
        cfg2.SetSetting("ConnectionStrings:Tenants:9:ReportingConnection", "TenantNamed", false);
        services2.AddSingleton<IConfigManager>(cfg2);
        services2.AddSingleton<ITenantSiteContext, FakeTenantSiteContext>();
        services2.AddSingleton<IConnectionSelector, FakeSelectorNoMatch>();
        services2.AddSingleton<SqlPlatformInfo, OqtSqlPlatformInfo>();
        using (var provider2 = services2.BuildServiceProvider())
        {
            var spi2 = provider2.GetRequiredService<SqlPlatformInfo>();
            Equal("TenantNamed", spi2.FindConnectionString("ReportingConnection"));
        }

        // Add site-level override (highest precedence)
    var services3 = new ServiceCollection();
    services3.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        var cfg3 = new FakeConfigManager();
        cfg3.SetSetting("ConnectionStrings:ReportingConnection", "PlatformNamed", false);
        cfg3.SetSetting("ConnectionStrings:Tenants:9:ReportingConnection", "TenantNamed", false);
        cfg3.SetSetting("ConnectionStrings:Tenants:9:Sites:42:ReportingConnection", "SiteNamed", false);
        services3.AddSingleton<IConfigManager>(cfg3);
        services3.AddSingleton<ITenantSiteContext, FakeTenantSiteContext>();
        services3.AddSingleton<IConnectionSelector, FakeSelectorNoMatch>();
        services3.AddSingleton<SqlPlatformInfo, OqtSqlPlatformInfo>();
        using var provider3 = services3.BuildServiceProvider();
        var spi3 = provider3.GetRequiredService<SqlPlatformInfo>();
        Equal("SiteNamed", spi3.FindConnectionString("ReportingConnection"));
    }
}
