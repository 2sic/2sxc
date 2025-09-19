using System;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using ToSic.Sxc.Oqt.Server.Data;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Oqt.Data.Tests;

public class OqtConnectionSelectorTests
{
    [Fact]
    public void Returns_Null_For_Default()
    {
        var sel = new OqtConnectionSelector(new FakeConfigManager());
        var result = sel.GetConnectionString(new TenantSiteKey(3, 7));
        Null(result);
    }

    [Fact]
    public void Returns_Null_When_TenantId_Zero()
    {
        var sel = new OqtConnectionSelector(new FakeConfigManager());
        // TenantId = 0 should not attempt overrides and return null
        Null(sel.GetConnectionString(new TenantSiteKey(0, 0)));
        Null(sel.GetConnectionString(new TenantSiteKey(0, 7)));
    }

    [Fact]
    public void Returns_Tenant_Override_When_Site_Missing()
    {
        var cfg = new FakeConfigManager();
        // Only tenant-level override exists
        cfg.AddOrUpdateSetting("ConnectionStrings:Tenants:3:DefaultConnection", "TenantConn", ispublic: false);

        var sel = new OqtConnectionSelector(cfg);
        var result = sel.GetConnectionString(new TenantSiteKey(3, 7));
        Equal("TenantConn", result);
    }

    [Fact]
    public void Returns_Site_Override_When_Both_Site_And_Tenant_Exist()
    {
        var cfg = new FakeConfigManager();
        // Both tenant and site-level overrides exist; site should take precedence
        cfg.AddOrUpdateSetting("ConnectionStrings:Tenants:3:DefaultConnection", "TenantConn", ispublic: false);
        cfg.AddOrUpdateSetting("ConnectionStrings:Tenants:3:Sites:7:DefaultConnection", "SiteConn", ispublic: false);

        var sel = new OqtConnectionSelector(cfg);
        var result = sel.GetConnectionString(new TenantSiteKey(3, 7));
        Equal("SiteConn", result);
    }

    private class FakeConfigManager : IConfigManager
    {
        private readonly System.Collections.Generic.Dictionary<string, string> _settings = new();

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

        public System.Collections.Generic.Dictionary<string, string> GetSettings(string settingname)
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

        public string GetConnectionString() => string.Empty;
        public string GetConnectionString(string tenant) => string.Empty;
        public bool IsInstalled() => true;
        public string GetInstallationId() => "test-installation";
    }
}
