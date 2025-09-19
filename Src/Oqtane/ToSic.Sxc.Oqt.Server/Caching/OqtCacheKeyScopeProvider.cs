using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Oqt.Server.Caching;

/// <summary>
/// Builds a tenant/site scope segment to ensure LightSpeed cache keys are isolated across tenants and sites.
/// </summary>
public class OqtCacheKeyScopeProvider(ITenantSiteContext ctx) : ICacheKeyScopeProvider
{
    public string? BuildScopeSegment()
    {
        var key = ctx.Current;
        // If single-tenant (0/0), don't add extra scope to preserve identical keys
        if (key.TenantId == 0 && key.SiteId == 0) return null;
        return $"t:{key.TenantId}-s:{key.SiteId}";
    }
}
