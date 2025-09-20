using System;
using Microsoft.Extensions.Configuration;
using Oqtane.Infrastructure;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtTenantContext : ServiceBase, IOqtTenantContext
{
    private readonly ITenantManager _tenantManager;
    private readonly IConfiguration _configuration;

    public OqtTenantContext(ITenantManager tenantManager, IConfiguration configuration)
        : base($"{OqtConstants.OqtLogPrefix}.TenCtx")
    {
        _tenantManager = tenantManager;
        _configuration = configuration;
        ConnectLogs([tenantManager]);
    }

    public bool TryGetConnection(out OqtTenantContextInfo context)
    {
        var l = Log.Fn<bool>();
        context = default;

        var alias = _tenantManager.GetAlias();
        if (alias == null)
            return l.ReturnFalse("alias not resolved");

        var tenant = _tenantManager.GetTenant();
        if (tenant == null)
            return l.ReturnFalse("tenant not resolved");

        var connectionString = ResolveConnectionString(tenant.DBConnectionString);
        if (string.IsNullOrWhiteSpace(connectionString))
            return l.ReturnFalse("connection string missing");

        context = new OqtTenantContextInfo(tenant.TenantId, alias.SiteId, tenant.DBConnectionString, connectionString);
        return l.ReturnTrue($"tenant:{tenant.TenantId}, site:{alias.SiteId}");
    }

    public OqtTenantContextInfo GetConnection()
    {
        if (TryGetConnection(out var context)) return context;
        throw new InvalidOperationException("Unable to resolve Oqtane tenant context for the current execution scope.");
    }

    public bool TryGetIdentity(out OqtTenantSiteIdentity identity)
    {
        if (TryGetConnection(out var context))
        {
            identity = context.Identity;
            return true;
        }

        identity = default;
        return false;
    }

    public OqtTenantSiteIdentity GetIdentity()
    {
        if (TryGetIdentity(out var identity)) return identity;
        throw new InvalidOperationException("Unable to resolve Oqtane tenant context for the current execution scope.");
    }

    private string ResolveConnectionString(string keyOrValue)
    {
        if (string.IsNullOrWhiteSpace(keyOrValue))
            return null;

        if (keyOrValue.Contains('='))
            return keyOrValue;

        var connection = _configuration.GetConnectionString(keyOrValue);
        if (!string.IsNullOrWhiteSpace(connection))
            return connection;

        connection = _configuration[$"ConnectionStrings:{keyOrValue}"];
        return string.IsNullOrWhiteSpace(connection) ? null : connection;
    }
}
