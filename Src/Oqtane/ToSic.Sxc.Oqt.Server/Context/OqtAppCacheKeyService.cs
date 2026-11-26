using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Web;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Context;

internal sealed class OqtAppCacheKeyService(IHttpContextAccessor httpContextAccessor) : IAppCacheKeyService
{
    public string AppCacheKey(IAppIdentity appIdentity)
    {
        // Short-Circuit if we already have a runtime key
        if (appIdentity is IAppStateCache { CacheKey: { } runtimeKey } && runtimeKey.HasValue())
            return runtimeKey;

        if (appIdentity.AppId == KnownAppsConstants.PresetAppId
            || appIdentity.AppId == KnownAppsConstants.GlobalPresetAppId)
            return $"t{KnownAppsConstants.PresetTenantId:D2}-z{KnownAppsConstants.PresetZoneId:D3}-a{appIdentity.AppId:D5}";

        var tenantId = GetTenantIdOrThrow();
        return $"t{tenantId:D2}-z{appIdentity.ZoneId:D3}-a{appIdentity.AppId:D5}"; // app runtime identifier
    }

    private int GetTenantIdOrThrow()
    {
        // A batch may run under an HTTP request belonging to another tenant. Its explicit
        // background identity must win because the shared app cache spans DI scopes.
        if (OqtBackgroundTenantScope.TenantId is { } backgroundTenantId)
            return backgroundTenantId;

        var requestServices = httpContextAccessor?.HttpContext?.GetSysRequestServices();
        var context = requestServices?.GetService<IOqtTenantContext>()?.Get();
        return context?.TenantId
               ?? throw new InvalidOperationException("Tenant not found");
    }
}
