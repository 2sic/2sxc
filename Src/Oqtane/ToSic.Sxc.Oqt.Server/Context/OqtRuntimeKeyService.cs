using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;

namespace ToSic.Sxc.Oqt.Server.Context;

internal sealed class OqtRuntimeKeyService(IHttpContextAccessor httpContextAccessor) : IRuntimeKeyService
{
    private const string Prefix = "ari"; // app runtime identifier

    public string AppRuntimeKey(IAppIdentity appIdentity)
    {
        if (appIdentity.AppId == KnownAppsConstants.PresetAppId
            || appIdentity.AppId == KnownAppsConstants.GlobalPresetAppId)
            return $"{Prefix}{KnownAppsConstants.PresetTenantId}-{KnownAppsConstants.PresetZoneId}-{appIdentity.AppId}";

        var tenantId = GetTenantIdOrThrow();
        return $"{Prefix}{tenantId}-{appIdentity.ZoneId}-{appIdentity.AppId}";
    }

    private int GetTenantIdOrThrow()
    {
        var requestServices = httpContextAccessor?.HttpContext?.RequestServices;
        var context = requestServices?.GetService<IOqtTenantContext>()?.Get();
        return context?.TenantId
               ?? throw new InvalidOperationException("Tenant not found");
    }
}
