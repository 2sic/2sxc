using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;

namespace ToSic.Sxc.Oqt.Server.Context;

internal sealed class OqtRuntimeKeyService(IHttpContextAccessor httpContextAccessor) : IRuntimeKeyService
{
    public string AppRuntimeKey(IAppIdentity appIdentity)
    {
        if (appIdentity.AppId == KnownAppsConstants.PresetAppId
            || appIdentity.AppId == KnownAppsConstants.GlobalPresetAppId)
            return $"t{KnownAppsConstants.PresetTenantId:D2}-z{KnownAppsConstants.PresetZoneId:D3}-a{appIdentity.AppId:D5}";

        var tenantId = GetTenantIdOrThrow();
        return $"t{tenantId:D2}-z{appIdentity.ZoneId:D3}-a{appIdentity.AppId:D5}"; // app runtime identifier
    }

    private int GetTenantIdOrThrow()
    {
        var requestServices = httpContextAccessor?.HttpContext?.RequestServices;
        var context = requestServices?.GetService<IOqtTenantContext>()?.Get();
        return context?.TenantId
               ?? throw new InvalidOperationException("Tenant not found");
    }
}
