using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Sxc.Oqt.Server.Context;

internal interface ITenantIdProvider
{
    int GetTenantId();
}

/// <summary>
/// Provides a stable per-tenant cache key based on the current request Alias.TenantId.
/// Falls back to "t:global" if no alias is available (eg. background tasks).
/// </summary>
internal sealed class TenantIdProvider(AliasResolver aliasResolver) : ITenantIdProvider
{
    public int GetTenantId()
        // AliasResolver will init SiteState/Alias on demand
        => aliasResolver.Alias?.TenantId ?? 1; // fallback to Master tenant (scheduled jobs live there)
}