namespace ToSic.Sxc.Oqt.Shared;

/// <summary>
/// Lightweight identifier combining Oqtane tenant and site information.
/// </summary>
public readonly record struct OqtTenantSiteIdentity(int TenantId, int SiteId)
{
    /// <summary>
    /// True when both tenant and site ids are non-negative (Oqtane uses positive ids for valid entries).
    /// </summary>
    public bool IsValid => TenantId >= 0 && SiteId >= 0;
}
