using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Abstraction for persisting compiled assemblies for Razor/code files.
/// </summary>
public interface IAssemblyDiskCacheService
{
    /// <summary>
    /// Attempts to load a cached assembly from disk for the specified template.
    /// </summary>
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
    bool TrySaveToCache(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash,
        AssemblyResult assemblyResult);

    /// <summary>
    /// Builds the full cache file path for the given template/spec combination.
    /// </summary>
    string GetCacheFilePath(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash);

    /// <summary>
    /// Invalidates cached assemblies for a specific template path / edition combination.
    /// </summary>
    void InvalidateCache(string templateRelativePath, string edition, string appPath = null);

    /// <summary>
    /// Invalidates (deletes) cached assemblies for a specific app and edition.
    /// </summary>
    void InvalidateAppCache(int appId, string edition);

    /// <summary>
    /// Checks if disk caching is enabled based on feature flag.
    /// </summary>
    bool IsEnabled();

    /// <summary>
    /// Gets the physical path to the cache directory.
    /// </summary>
    string GetCacheDirectoryPath();

    /// <summary>
    /// Computes SHA256 hash of the given content string.
    /// </summary>
    string ComputeContentHash(string sourceCode);
}
