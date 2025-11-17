using System.Reflection;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Service for persisting and retrieving compiled Razor assemblies from disk cache.
/// Implements disk-based caching layer below memory cache for improved restart performance.
/// Delegates to shared AssemblyDiskCache for platform-neutral file operations.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyDiskCacheService(
    LazySvc<IFeaturesService> featureService,
    IGlobalConfiguration globalConfiguration,
    AssemblyDiskCache diskCache)
  : ServiceBase("Dnn.AsmDskCch", connect: [featureService, globalConfiguration, diskCache]), IAssemblyDiskCacheService
{
    /// <summary>
    /// Attempts to load a cached assembly from disk for the specified template.
    /// </summary>
    public AssemblyResult? TryLoadFromCache(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash,
        RoslynBuildManager roslynBuildManager,
        CodeFileInfo codeFileInfo)
    {
        var l = Log.Fn<AssemblyResult?>($"app:{spec.AppId}, edition:{spec.Edition}, template:{templateRelativePath}", timer: true);

        // Generate cache key
        var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
        var cacheKey = new CacheKey(spec.AppId, spec.Edition, normalizedPath, contentHash, appCodeHash);
        var cachePath = cacheKey.GetFilePath(GetCacheDirectoryPath());

        // Load assembly bytes from disk using shared cache service
        var assembly = diskCache.TryLoadFromCache(
            cachePath,
            loadAssembly: assemblyBytes => Assembly.Load(File.ReadAllBytes(assemblyBytes)),
            featureFlagCheck: IsEnabled);

        if (assembly == null)
            return l.ReturnNull();

        try
        {
            // Load references so we can get the main type later (Razor-specific logic)
            var (referencedAssemblies, appCodeDependency, appCodeAssembly) = roslynBuildManager.ReferencedAssemblies(codeFileInfo, spec);

            // Create AssemblyResult with MainType populated
            var className = RoslynBuildManager.GetSafeClassName(templateRelativePath);
            var mainType = RoslynBuildManager.FindMainType(assembly, className, isCshtml: true, Log);

            if (mainType == null)
            {
                l.A($"Warning: Could not find main type {className} in assembly");
                return l.ReturnNull();
            }

            var result = new AssemblyResult(assembly)
            {
                MainType = mainType,
                AppCodeDependency = appCodeDependency
            };

            return l.Return(result, "Cache hit - loaded from disk");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            l.A("Error processing cached assembly - will trigger recompilation");
            return l.ReturnNull();
        }
    }

    /// <summary>
    /// Saves a compiled assembly to disk cache for future reuse.
    /// </summary>
    public bool TrySaveToCache(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash,
        AssemblyResult assemblyResult)
    {
        var l = Log.Fn<bool>($"app:{spec.AppId}, edition:{spec.Edition}, template:{templateRelativePath}", timer: true);

        // Generate cache key
        var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
        var cacheKey = new CacheKey(spec.AppId, spec.Edition, normalizedPath, contentHash, appCodeHash);
        var cachePath = cacheKey.GetFilePath(GetCacheDirectoryPath());

        // Get source assembly path
        var sourceAssemblyPath = assemblyResult.Assembly.Location;

        // Delegate to shared disk cache service
        var saved = diskCache.TrySaveToCache(
            sourceAssemblyPath,
            cachePath,
            featureFlagCheck: IsEnabled);

        return saved
            ? l.ReturnTrue("saved")
            : l.ReturnFalse("failed");
    }

    /// <summary>
    /// Invalidates (deletes) cached assemblies for a specific template.
    /// </summary>
    public void InvalidateCache(string templateRelativePath)
    {
        var l = Log.Fn($"template:{templateRelativePath}");

        var cacheDir = GetCacheDirectoryPath();
        var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
        var searchPattern = $"*-{normalizedPath}-*.dll";

        var deletedCount = diskCache.InvalidateCache(cacheDir, searchPattern, SearchOption.AllDirectories);
        l.A($"Invalidated {deletedCount} cache files for template {templateRelativePath}");

        l.Done();
    }

    /// <summary>
    /// Invalidates all cached assemblies for a specific app and edition.
    /// </summary>
    public void InvalidateAppCache(int appId, string edition)
    {
        var l = Log.Fn($"app:{appId}, edition:{edition}");

        var cacheDir = GetCacheDirectoryPath();
        var editionNormalized = CacheKey.NormalizeEdition(edition);
        var appDir = Path.Combine(cacheDir, CacheKey.GetAppFolder(appId), CacheKey.GetEditionFolder(editionNormalized));

        var deletedCount = Directory.Exists(appDir)
            ? diskCache.InvalidateCache(appDir, "*.dll", SearchOption.AllDirectories)
            : 0;
        l.A($"Invalidated {deletedCount} cache files for app {appId} edition {editionNormalized}");

        l.Done();
    }

    /// <summary>
    /// Checks if disk caching is enabled based on feature flag.
    /// </summary>
    public bool IsEnabled()
        // Not cached - check on every call for runtime toggle support
        => featureService.Value.IsEnabled(Sxc.Sys.Configuration.SxcFeatures.RazorCacheCompiledToDisk.NameId);

    /// <summary>
    /// Gets the physical path to the cache directory.
    /// </summary>
    public string GetCacheDirectoryPath()
        // App_Data/2sxc.bin.cshtml
        => globalConfiguration.CshtmlAssemblyFolder();

    /// <summary>
    /// Computes SHA256 hash of the given content string.
    /// </summary>
    /// <remarks>
    /// SHA256 is FIPS-compliant and performant for typical template sizes.
    /// </remarks>
    public string ComputeContentHash(string sourceCode)
        => diskCache.ComputeContentHash(sourceCode);
}
