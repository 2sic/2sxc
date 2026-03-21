namespace ToSic.Sxc.Services.OutputCache;

/// <summary>
/// Service to manage the output cache (LightSpeed) for the current module.
/// To be used within Razor templates to control caching behavior of module output, allowing for dynamic content while still benefiting from caching where appropriate.
/// </summary>
/// <remarks>
/// It allows enabling/disabling the cache, configuring cache settings, and adding dependencies that will invalidate the cache when they change.
/// </remarks>
public interface IModuleOutputCacheService
{
    /// <summary>
    /// The current module ID
    /// </summary>
    [PrivateApi("for internal use only")]
    internal int ModuleId { get; set; }

    /// <summary>
    /// Set the configuration of the output cache for the current module.
    /// </summary>
    /// <remarks>
    /// If only some settings are provided, the others will be filled with default values or values derived from the provided settings.
    /// This means that you can specify only the settings you want to change, and the service will ensure that all necessary settings have valid values.
    /// For example, if Duration is set but DurationUsers is not, then DurationUsers will be set to the same value as Duration.
    /// This allows for flexible configuration while ensuring that all necessary settings have valid values.
    /// </remarks>
    /// <param name="settings"></param>
    /// <returns></returns>
    string Configure(OutputCacheSettings settings);

    /// <summary>
    /// Explicitly disable the output cache for the current module.
    /// This will override any other cache settings and ensure that the module's output is not cached.
    /// </summary>
    /// <returns></returns>
    string Disable();

    /// <summary>
    /// Add a named external dependency that the output cache should depend on.
    /// When the specified dependency changes, the current app's cached output will be invalidated and refreshed.
    /// </summary>
    /// <remarks>
    /// This currently affects output caching only.
    /// Internally the dependency is tracked through a shared named-marker service so future <c>Kit.Cache</c>
    /// APIs can reuse the same invalidation mechanism without inventing a second dependency system.
    /// </remarks>
    /// <param name="key"></param>
    /// <returns></returns>
    string DependOn(string key);
    // TODO: unclear where / how to add such dependencies, that they are properly tracked and trigger cache invalidation when they change.
    // Maybe we need a more generic API for this, that can be used not only for output cache but also for data caching?

    /// <summary>
    /// Flush one or more named output-cache dependencies for the current app.
    /// If <paramref name="dependencies"/> is null or empty, the app-wide output-cache dependency marker is touched.
    /// </summary>
    /// <param name="dependencies"></param>
    /// <returns>
    /// The number of normalized named dependency markers that were touched.
    /// Returns <c>0</c> when the app-wide flush path is used.
    /// </returns>
    int Flush(IEnumerable<string>? dependencies = null);

    /// <summary>
    /// Explicitly enable (or disable) the output cache for the current module.
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    string Enable(bool enable = true);
}
