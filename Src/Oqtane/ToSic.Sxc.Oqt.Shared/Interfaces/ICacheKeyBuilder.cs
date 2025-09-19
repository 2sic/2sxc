using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

/// <summary>
/// Builds deterministic cache keys. Implementations should be stable across versions to avoid cache churn.
/// </summary>
public interface ICacheKeyBuilder
{
    /// <summary>
    /// Build a cache key given a namespace, parts and optional tenant-site key.
    /// If key is provided, it must be included to ensure tenant isolation.
    /// </summary>
    string Build(string @namespace, string[] parts, TenantSiteKey? key = null);
}
