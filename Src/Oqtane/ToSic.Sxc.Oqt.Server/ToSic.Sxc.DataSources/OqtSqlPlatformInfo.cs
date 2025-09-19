using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Eav.DataSources.Sys;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources;

internal class OqtSqlPlatformInfo(
    LazySvc<IConfigManager> configManager,
    LazySvc<IOqtTenantContext> tenantContext) : SqlPlatformInfo
{
    public override string DefaultConnectionStringName => SettingKeys.ConnectionStringKey;

    public override string FindConnectionString(string name)
    {
        var tenantInfoProvider = tenantContext.Value;
        if (tenantInfoProvider.TryGet(out var tenantContextInfo) && !string.IsNullOrWhiteSpace(tenantContextInfo.ConnectionString))
        {
            if (name.EqualsInsensitive(DefaultConnectionStringName) || name.EqualsInsensitive(tenantContextInfo.ConnectionStringName))
                return tenantContextInfo.ConnectionString;
        }

        if (name.EqualsInsensitive(DefaultConnectionStringName))
        {
            var fallback = configManager.Value.GetConnectionString();
            if (!string.IsNullOrWhiteSpace(fallback))
                return fallback;

            return configManager.Value.GetSetting("ConnectionStrings:" + SettingKeys.ConnectionStringKey, "");
        }

        var resolved = configManager.Value.GetConnectionString(name);
        if (!string.IsNullOrWhiteSpace(resolved))
            return resolved;

        resolved = configManager.Value.GetSetting("ConnectionStrings:" + name, "");
        if (!string.IsNullOrWhiteSpace(resolved))
            return resolved;

        return base.FindConnectionString(name);
    }

}
