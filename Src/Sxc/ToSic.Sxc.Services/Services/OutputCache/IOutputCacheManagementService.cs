namespace ToSic.Sxc.Services.OutputCache;

/// <summary>
/// Service on <c>Kit.OutputCacheManagement</c> to invalidate LightSpeed output-cache markers for a specific app.
/// </summary>
/// <remarks>
/// This is intentionally separate from <see cref="IModuleOutputCacheService"/>, which only affects the
/// current render. Management operations require the target <paramref name="appId"/> explicitly, so they do not
/// depend on ambient execution context.
/// </remarks>
[PublicApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IOutputCacheManagementService
{
    /// <summary>
    /// Flush output-cache entries for a specific app.
    /// </summary>
    /// <param name="appId">The app whose output-cache markers should be touched.</param>
    /// <param name="dependencies">
    /// Optional named dependencies to flush selectively.
    /// If omitted or empty after normalization, the app-wide output-cache marker is touched.
    /// </param>
    /// <returns>
    /// The number of normalized named dependency markers that were touched.
    /// Returns <c>0</c> when the app-wide flush path is used.
    /// </returns>
    int Flush(int appId, IEnumerable<string>? dependencies = null);
}
