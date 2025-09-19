namespace ToSic.Sxc.Web.Sys.LightSpeed;

/// <summary>
/// Optional hook to augment LightSpeed cache keys with a platform-specific scope.
/// Implementations may return a short, deterministic segment like "t:3-s:7" to ensure isolation.
/// The default implementation returns null to preserve existing behavior.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICacheKeyScopeProvider
{
    /// <summary>
    /// Build a short scope segment to append to LightSpeed cache keys, or null if not applicable.
    /// </summary>
    string? BuildScopeSegment();
}
