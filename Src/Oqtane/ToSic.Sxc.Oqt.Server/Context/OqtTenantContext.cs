using Microsoft.Extensions.Configuration;
using Oqtane.Infrastructure;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Context;


internal class OqtTenantContext(
    ITenantManager tenantManager,
    IConfiguration configuration)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.TenCtx", connect: [tenantManager]), IOqtTenantContext
{
    public OqtTenantContextInfo? Get()
    {
        var l = Log.Fn<OqtTenantContextInfo?>();

        var alias = tenantManager.GetAlias();
        if (alias == null)
            return l.ReturnNull("alias not resolved");

        var tenant = tenantManager.GetTenant();
        if (tenant == null)
            return l.ReturnNull("tenant not resolved");

        var tenantConnection = tenant.DBConnectionString;
        var connectionString = ResolveConnectionString(tenantConnection);
        if (!connectionString.HasValue())
            return l.ReturnNull("connection string missing");

        var context = new OqtTenantContextInfo(
            TenantId: tenant.TenantId,
            SiteId: alias.SiteId,
            ConnectionStringName: tenantConnection.HasValue() && !tenantConnection.Contains('=')
                ? tenantConnection
                : "",
            ConnectionString: connectionString
        );

        return l.Return(context, $"tenant:{tenant.TenantId}, site:{alias.SiteId}");
    }

    public OqtTenantContextInfo GetRequired()
        => Get() ?? throw new InvalidOperationException("Unable to resolve Oqtane tenant context for the current execution scope.");

    private string ResolveConnectionString(string keyOrValue)
    {
        if (keyOrValue.IsEmpty())
            return null;

        if (keyOrValue.Contains('='))
            return keyOrValue;

        var connection = configuration.GetConnectionString(keyOrValue);
        return connection.HasValue()
            ? connection
            : null;
    }
}
