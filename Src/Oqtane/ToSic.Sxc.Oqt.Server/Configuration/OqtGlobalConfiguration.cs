using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sys.Configuration;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Configuration;

/// <summary>
/// Oqtane-specific GlobalConfiguration which resolves ConnectionString dynamically per-request/tenant.
/// For all other configuration values it falls back to the base GlobalConfiguration storage.
/// </summary>
internal class OqtGlobalConfiguration(IHttpContextAccessor httpContextAccessor) : GlobalConfiguration, IGlobalConfiguration
{
    private const string KeyConnectionString = nameof(GlobalConfigDb.ConnectionString);

    public new string GetThis(string key)
    {
        // Only special-case the ConnectionString; everything else defer to base implementation
        if (key.HasValue() && key.EqualsInsensitive(KeyConnectionString))
        {
            // Only attempt tenant-based resolution when a request/tenant scope exists.
            var cs = TryResolveTenantConnectionString();
            if (cs.HasValue())
                return cs;
        }
        return base.GetThis(key);
    }

    public new string GetThisOrSet(Func<string> generator, string key)
    {
        // Delegate to the interface GetThis which we override for ConnectionString
        var current = GetThis(key);
        if (current.HasValue())
            return current!;

        var value = generator();
        SetThis(value, key);
        return value;
    }

    public new string GetThisErrorOnNull(string key)
    {
        var value = GetThis(key);
        return value ?? throw new ArgumentNullException(ErrorMessageNullNotAllowed(nameof(key)));
    }

    public new string SetThis(string value, string key)
        // For ConnectionString we still allow setting a default/fallback value (eg. during boot),
        // but per-request access will prefer the dynamic tenant value when available.
        => base.SetThis(value, key);

    private string TryResolveTenantConnectionString()
    {
        // Only resolve tenant when a request scope is present; otherwise use the base/fallback value.
        var requestServices = httpContextAccessor?.HttpContext?.RequestServices;
        if (requestServices == null)
            return null;

        var ctx = requestServices.GetService<IOqtTenantContext>()?.Get();
        return ctx is { } info && info.ConnectionString.HasValue()
            ? info.ConnectionString
            : null;
    }
}
