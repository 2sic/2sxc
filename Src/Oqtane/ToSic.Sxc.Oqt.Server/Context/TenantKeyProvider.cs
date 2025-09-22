using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Sxc.Oqt.Server.Context;

internal interface ITenantKeyProvider
{
    string GetKey();
}

/// <summary>
/// Provides a stable per-tenant cache key based on the current request Alias.TenantId.
/// Falls back to "t:global" if no alias is available (eg. background tasks).
/// </summary>
internal sealed class TenantKeyProvider(AliasResolver aliasResolver) : ITenantKeyProvider
{
    public string GetKey()
    {
        // AliasResolver will init SiteState/Alias on demand
        var alias = aliasResolver.Alias;
        return alias != null
            ? $"t:{alias.TenantId}"
            : "t:1"; // fallback to Master tenant (scheduled jobs live there)
    }
}