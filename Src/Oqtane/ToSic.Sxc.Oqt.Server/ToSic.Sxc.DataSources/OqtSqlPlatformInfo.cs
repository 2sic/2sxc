using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Eav.DataSources.Sys;
using ToSic.Sys.Utils;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources;

internal class OqtSqlPlatformInfo(
    LazySvc<IConfigManager> configManager,
    LazySvc<ITenantSiteContext> ctx,
    LazySvc<IConnectionSelector> selector
) : SqlPlatformInfo
{
    public override string DefaultConnectionStringName => SettingKeys.ConnectionStringKey;

    public override string FindConnectionString(string name)
    {
        if (name.EqualsInsensitive(DefaultConnectionStringName))
        {
            // See also: Docs/Configuration.MultiDatabase.md for configuration details
            // Try tenant-specific override first
            TenantSiteKey key = ctx.Value.Current;
            var overrideConn = selector.Value.GetConnectionString(key);
            if (!string.IsNullOrWhiteSpace(overrideConn))
                return overrideConn!;

            // Fallback to platform default from configuration
            return configManager.Value.GetSetting("ConnectionStrings:" + SettingKeys.ConnectionStringKey, "");
        }

        // Named connection: support tenant/site overrides with same precedence as default
        // Lookup order:
        // 1) ConnectionStrings:Tenants:{TenantId}:Sites:{SiteId}:{name}
        // 2) ConnectionStrings:Tenants:{TenantId}:{name}
        // 3) ConnectionStrings:{name}

        var current = ctx.Value.Current;

        if (current.TenantId > 0 && current.SiteId > 0)
        {
            var siteKey = $"ConnectionStrings:Tenants:{current.TenantId}:Sites:{current.SiteId}:{name}";
            var csSite = configManager.Value.GetSetting(siteKey, "");
            if (!string.IsNullOrWhiteSpace(csSite)) return csSite;
        }

        if (current.TenantId > 0)
        {
            var tenantKey = $"ConnectionStrings:Tenants:{current.TenantId}:{name}";
            var csTenant = configManager.Value.GetSetting(tenantKey, "");
            if (!string.IsNullOrWhiteSpace(csTenant)) return csTenant;
        }

        var platformKey = $"ConnectionStrings:{name}";
        var csPlatform = configManager.Value.GetSetting(platformKey, "");
        if (!string.IsNullOrWhiteSpace(csPlatform)) return csPlatform;

        return base.FindConnectionString(name);
    }

}