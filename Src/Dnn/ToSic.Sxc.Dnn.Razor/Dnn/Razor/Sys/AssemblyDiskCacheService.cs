using System.Reflection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Configuration;
using ISite = ToSic.Eav.Context.ISite;

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
    AssemblyDiskCache diskCache,
    AssemblyUtilities assemblyUtilities,
    AssemblyResolver assemblyResolver,
    IAppReaderFactory appReadFac,
    LazySvc<IAppPathsMicroSvc> appPathsLazy,
    ISite site)
  : ServiceBase("Dnn.AsmDskCch", connect: [featureService, globalConfiguration, diskCache, assemblyUtilities, assemblyResolver, appReadFac, appPathsLazy, site]), IAssemblyDiskCacheService
{
    /// <summary>
    /// Attempts to load a cached assembly from disk for the specified template.
    /// </summary>
    public AssemblyResult TryLoadFromCache(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash,
        AssemblyResult appCodeDependency,
        CodeFileInfo codeFileInfo)
    {
        var l = Log.Fn<AssemblyResult>($"app:{spec.AppId}, edition:{spec.Edition}, template:{templateRelativePath}", timer: true);

        // Generate cache key
        var cachePath = GetCacheFilePath(spec, templateRelativePath, contentHash, appCodeHash);

        // Ensure assembly resolver can provide AppCode dependencies when the cached assembly is loaded
        if (appCodeDependency?.Assembly != null)
            assemblyResolver.AddAssembly(appCodeDependency.Assembly, GetAppRelativePath(spec));

        // Load assembly bytes from disk using shared cache service
        var assembly = diskCache.TryLoadFromCache(
            cachePath,
            loadAssembly: assemblyBytes => Assembly.Load(File.ReadAllBytes(assemblyBytes)),
            featureFlagCheck: IsEnabled);

        if (assembly == null)
            return l.ReturnNull();

        try
        {
            // Create AssemblyResult with MainType and AppCodeDependency populated
            var className = assemblyUtilities.GetSafeClassName(templateRelativePath);
            var mainType = assemblyUtilities.FindMainType(assembly, className, isCshtml: true);

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
        var cachePath = GetCacheFilePath(spec, templateRelativePath, contentHash, appCodeHash);

        // Get source assembly path
        var sourceAssemblyPath = assemblyResult.Assembly?.Location;

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
    /// Builds the full cache file path for the given template/spec combination.
    /// </summary>
    public string GetCacheFilePath(
        HotBuildSpec spec,
        string templateRelativePath,
        string contentHash,
        string appCodeHash)
    {
        var cacheKey = BuildCacheKey(spec, templateRelativePath, contentHash, appCodeHash);
        return cacheKey.GetFilePath(GetCacheDirectoryPath());
    }

    private CacheKey BuildCacheKey(HotBuildSpec spec, string templateRelativePath, string contentHash, string appCodeHash)
        => new(spec.AppId, spec.Edition, templateRelativePath, contentHash, appCodeHash, GetAppRelativePath(spec));

    private string GetAppRelativePath(HotBuildSpec spec)
    {
        if (spec.AppId <= 0)
            return string.Empty;

        return appPathsLazy.Value.Get(appReadFac.Get(spec.AppId), site).RelativePath;
    }

    /// <summary>
    /// Invalidates (deletes) cached assemblies for a specific template.
    /// </summary>
    public void InvalidateCache(string templateRelativePath, string edition, string appPath = null)
    {
        var l = Log.Fn($"template:{templateRelativePath}");

        var cacheDir = GetCacheDirectoryPath();
        var normalizedPath = CacheKey.NormalizePath(templateRelativePath, edition, appPath);
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
        var appDir = Path.Combine(cacheDir, CacheKey.GetAppFolder(appId, edition));

        var deletedCount = Directory.Exists(appDir)
            ? diskCache.InvalidateCache(appDir, "*.dll", SearchOption.AllDirectories)
            : 0;
        l.A($"Invalidated {deletedCount} cache files for app {appId} edition {edition}");

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
