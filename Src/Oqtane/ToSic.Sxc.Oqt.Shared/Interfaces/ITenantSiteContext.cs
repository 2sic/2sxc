using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

/// <summary>
/// Provides access to the current Oqtane tenant/site composite key.
/// </summary>
public interface ITenantSiteContext
{
    TenantSiteKey Current { get; }
}
