using Oqtane.Infrastructure;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Data;

/// <summary>
/// Default connection selector that currently falls back to the platform default connection.
/// Future implementations can consult per-tenant settings to select another connection string.
/// </summary>
public class OqtConnectionSelector : IConnectionSelector
{
    private readonly LazySvc<IConfigManager> _configLazy;
    private readonly IConfigManager _configDirect;

    public OqtConnectionSelector(LazySvc<IConfigManager> config) => _configLazy = config;

    // Convenience ctor for tests without LazySvc
    public OqtConnectionSelector(IConfigManager configManager) => _configDirect = configManager;

    public string? GetConnectionString(TenantSiteKey key)
    {
        // See also: Docs/Configuration.MultiDatabase.md for configuration details
        // Lookup order (override -> fallback):
        // 1) ConnectionStrings:Tenants:{TenantId}:Sites:{SiteId}:DefaultConnection
        // 2) ConnectionStrings:Tenants:{TenantId}:DefaultConnection
        // else null (use platform default)

        var t = key.TenantId;
        var s = key.SiteId;

    var config = _configDirect ?? _configLazy.Value;

        if (t > 0 && s > 0)
        {
            var siteKey = $"ConnectionStrings:Tenants:{t}:Sites:{s}:DefaultConnection";
            var csSite = config.GetSetting(siteKey, "");
            if (!string.IsNullOrWhiteSpace(csSite)) return csSite;
        }

        if (t > 0)
        {
            var tenantKey = $"ConnectionStrings:Tenants:{t}:DefaultConnection";
            var csTenant = config.GetSetting(tenantKey, "");
            if (!string.IsNullOrWhiteSpace(csTenant)) return csTenant;
        }

        return null;
    }
}
