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
    // Make the default connection-string name tenant-aware.
    // If the current tenant has a named connection, use that; otherwise fall back to Oqtane default.
    public override string DefaultConnectionStringName
        => tenantContext.Value.Get() is { } ctx && ctx.ConnectionStringName.HasValue()
            ? ctx.ConnectionStringName
            : SettingKeys.ConnectionStringKey;

    public override string FindConnectionString(string name)
    {
        if (tenantContext.Value.Get() is { } tenantContextInfo && tenantContextInfo.ConnectionString.HasValue())
        {
            if (name.EqualsInsensitive(SettingKeys.ConnectionStringKey))
                return tenantContextInfo.ConnectionString;

            if (tenantContextInfo.ConnectionStringName.HasValue() && name.EqualsInsensitive(tenantContextInfo.ConnectionStringName))
                return tenantContextInfo.ConnectionString;
        }

        var config = configManager.Value;
        if (name.EqualsInsensitive(SettingKeys.ConnectionStringKey))
        {
            var defaultConnection = config.GetConnectionString();
            if (defaultConnection.HasValue())
                return defaultConnection;
        }

        var resolved = config.GetConnectionString(name);
        if (resolved.HasValue())
            return resolved;

        resolved = config.GetSetting($"ConnectionStrings:{name}", "");
        if (resolved.HasValue())
            return resolved;

        return base.FindConnectionString(name);
    }

}
