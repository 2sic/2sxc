using System;

namespace ToSic.Sxc.Oqt.Shared.Models;

/// <summary>
/// Composite identifier for an Oqtane tenant/site context.
/// Used to scope cache keys, logs, and storage.
/// </summary>
public readonly record struct TenantSiteKey(int TenantId, int SiteId)
{
    public override string ToString() => $"t:{TenantId}/s:{SiteId}";

    public static TenantSiteKey From(int tenantId, int siteId) => new(tenantId, siteId);
}
