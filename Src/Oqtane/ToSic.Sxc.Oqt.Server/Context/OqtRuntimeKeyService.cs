using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;

namespace ToSic.Sxc.Oqt.Server.Context;

internal sealed class OqtRuntimeKeyService(IHttpContextAccessor httpContextAccessor) : IRuntimeKeyService
{
    private const int FallbackTenantId = 1;
    private const string Prefix = "ari"; // app runtime identifier

    public string AppRuntimeKey(IAppIdentity appIdentity)
    {
        var tenantId = GetTenantIdOrThrow();
        return $"{Prefix}{tenantId}-{appIdentity.ZoneId}-{appIdentity.AppId}";
    }

    private int GetTenantIdOrThrow()
    {
        var requestServices = httpContextAccessor?.HttpContext?.RequestServices;
        if (requestServices == null)
            return FallbackTenantId;

        var context = requestServices.GetService<IOqtTenantContext>()?.Get();
        return context?.TenantId
               ?? throw new InvalidOperationException("Tenant not found");
    }
}
