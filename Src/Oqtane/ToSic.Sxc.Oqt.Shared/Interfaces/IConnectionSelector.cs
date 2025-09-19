using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Shared.Interfaces;

/// <summary>
/// Selects the connection string name/value to use for a given tenant/site context.
/// Implementations may look up tenant-specific mappings or fall back to defaults.
/// </summary>
public interface IConnectionSelector
{
    /// <summary>
    /// Get the connection string to use for the provided key.
    /// Returns null when the default connection should be used.
    /// </summary>
    string? GetConnectionString(TenantSiteKey key);
}
