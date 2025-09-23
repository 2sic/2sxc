using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Oqt.Server.Configuration;

/// <summary>
/// Oqtane-specific GlobalConfiguration which resolves ConnectionString dynamically per-request/tenant.
/// For all other configuration values it falls back to the base GlobalConfiguration storage.
/// </summary>
internal class OqtGlobalConfiguration(IHttpContextAccessor? httpContextAccessor) : GlobalConfiguration, IGlobalConfiguration
{
    string? IGlobalConfiguration.GetThis(string? key)
    {
        // Only special-case the ConnectionString; everything else defer to base implementation
        if (!string.IsNullOrWhiteSpace(key) && key.Equals("ConnectionString", StringComparison.InvariantCultureIgnoreCase))
        {
            // Only attempt tenant-based resolution when a request/tenant scope exists.
            var cs = TryResolveTenantConnectionString();
            if (!string.IsNullOrWhiteSpace(cs)) return cs;
        }
        return base.GetThis(key);
    }

    string? IGlobalConfiguration.GetThisOrSet(Func<string> generator, string? key)
    {
        // Delegate to the interface GetThis which we override for ConnectionString
        var current = ((IGlobalConfiguration)this).GetThis(key);
        if (!string.IsNullOrEmpty(current)) return current!;

        var value = generator();
        ((IGlobalConfiguration)this).SetThis(value, key);
        return value;
    }

    string IGlobalConfiguration.GetThisErrorOnNull(string? key)
    {
        var value = ((IGlobalConfiguration)this).GetThis(key);
        return value ?? throw new ArgumentNullException(ErrorMessageNullNotAllowed(nameof(key)));
    }

    string? IGlobalConfiguration.SetThis(string? value, string? key)
    {
        // For ConnectionString we still allow setting a default/fallback value (eg. during boot),
        // but per-request access will prefer the dynamic tenant value when available.
        return base.SetThis(value, key);
    }

    private string? TryResolveTenantConnectionString()
    {
        // Only resolve tenant when a request scope is present; otherwise use the base/fallback value.
        var httpContext = httpContextAccessor?.HttpContext;
        if (httpContext == null) return null;

        var requestServices = httpContext.RequestServices;
        var tm = requestServices.GetService<ITenantManager>();
        if (tm == null) return null;

        var tenant = tm.GetTenant();

        var configManager = requestServices.GetService<IConfigManager>();
        if (configManager == null) return null;
        
        return configManager.GetSetting("ConnectionStrings:" + tenant.DBConnectionString, "");
    }
}
