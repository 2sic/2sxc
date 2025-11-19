using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Service for persisting and retrieving compiled Razor assemblies from disk cache.
/// Implements disk-based caching layer below memory cache for improved restart performance.
/// </summary>
public interface IAssemblyDiskCacheService
{
    /// <summary>
    /// Attempts to load a cached assembly from disk for the specified template.
    /// </summary>
    /// <param name="spec">Hot build specification containing app and edition info</param>
    /// <param name="templateRelativePath">Relative path to the CSHTML template</param>
    /// <param name="contentHash">Hash of the template's source code content</param>
    /// <param name="appCodeHash">Hash of the AppCode assembly (includes version info)</param>
    /// <param name="appCodeDependency">The AppCode dependency to attach to the loaded assembly</param>
    /// <param name="codeFileInfo">Code file information</param>
    /// <returns>Cached assembly result if found and valid; null if cache miss or invalid</returns>
    /// <remarks>
    /// Returns null in these scenarios:
    /// - Cache file does not exist
    /// - Content hash mismatch (template changed)
    /// - AppCode hash mismatch (dependencies changed)
    /// - Assembly load fails (corruption)
    /// - Feature flag is disabled
    /// </remarks>
    AssemblyResult TryLoadFromCache(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash,
        AssemblyResult appCodeDependency,
        CodeFileInfo codeFileInfo);

    /// <summary>
    /// Saves a compiled assembly to disk cache for future reuse.
    /// </summary>
    /// <param name="spec">Hot build specification containing app and edition info</param>
    /// <param name="templateRelativePath">Relative path to the CSHTML template</param>
    /// <param name="contentHash">Hash of the template's source code content</param>
    /// <param name="appCodeHash">Hash of the AppCode assembly</param>
    /// <param name="assemblyResult">The compiled assembly to cache</param>
    /// <returns>True if save successful; false if error occurred (logged but not thrown)</returns>
    /// <remarks>
    /// Gracefully handles failures:
    /// - Directory creation failures
    /// - Disk space exhaustion → fallback to memory-only
    /// - Permission errors
    /// - Feature flag disabled (no-op)
    /// All errors are logged but do not throw exceptions.
    /// </remarks>
    bool TrySaveToCache(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash,
        AssemblyResult assemblyResult);

    /// <summary>
    /// Invalidates (deletes) cached assemblies for a specific template.
    /// </summary>
    /// <param name="templateRelativePath">Relative path to the CSHTML template</param>
    /// <remarks>
    /// Called when file watcher detects template modification.
    /// Safe to call even if cache doesn't exist.
    /// Supports wildcard matching for path variations.
    /// </remarks>
    void InvalidateCache(string templateRelativePath);

    /// <summary>
    /// Invalidates all cached assemblies for a specific app and edition.
    /// </summary>
    /// <param name="appId">Application identifier</param>
    /// <param name="edition">App edition (e.g., "live", "staging")</param>
    /// <remarks>
    /// Called when AppCode assembly changes for the app.
    /// Bulk operation - may delete multiple cache files.
    /// </remarks>
    void InvalidateAppCache(int appId, string edition);

    /// <summary>
    /// Checks if disk caching is enabled based on feature flag.
    /// </summary>
    /// <returns>True if RazorCacheCompiledToDisk feature is enabled</returns>
    /// <remarks>
    /// Checked on every cache operation (not cached) for runtime toggle support.
    /// </remarks>
    bool IsEnabled();

    /// <summary>
    /// Gets the physical path to the cache directory.
    /// </summary>
    /// <returns>Full path to App_Data/2sxc.bin/cshtml/</returns>
    string GetCacheDirectoryPath();

    /// <summary>
    /// Computes SHA256 hash of the given content string for cache key generation.
    /// </summary>
    /// <param name="sourceCode">The source code content to hash</param>
    /// <returns>Lowercase hex string representing the SHA256 hash; empty string if input is null/empty</returns>
    /// <remarks>
    /// SHA256 is FIPS-compliant and performant for typical template sizes.
    /// </remarks>
    string ComputeContentHash(string sourceCode);
}
