using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Service for persisting and retrieving compiled Razor assemblies from disk cache.
/// Implements disk-based caching layer below memory cache for improved restart performance.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyDiskCacheService(
    LazySvc<IFeaturesService> featureService,
    IGlobalConfiguration globalConfiguration)
    : ServiceBase("Dnn.AsmDskCch", connect: [featureService, globalConfiguration]), IAssemblyDiskCacheService
{
    private const string CacheSubFolder = "cshtml";

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

        // Check feature flag first
        if (!IsEnabled())
        {
            l.A("Disk cache disabled via feature flag");
            return l.ReturnNull();
        }

        // Performance measurement
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Generate cache key
            var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
            var cacheKey = new CacheKey(spec.AppId, spec.Edition, normalizedPath, contentHash, appCodeHash);
            var cachePath = cacheKey.GetFilePath(GetCacheDirectoryPath());

            // Use FileInfo for metadata checks before full read (performance optimization)
            var fileInfo = new FileInfo(cachePath);
            if (!fileInfo.Exists)
            {
                l.A($"Cache miss: file not found at {cachePath}");
                return l.ReturnNull();
            }

            // Load assembly bytes from file
            var assemblyBytes = File.ReadAllBytes(cachePath);
            l.A($"Loaded {assemblyBytes.Length} bytes from disk cache in {fileInfo.Length / 1024}KB");

            // Create assembly from bytes
            var assembly = Assembly.Load(assemblyBytes);
            l.A($"Successfully loaded assembly: {assembly.FullName}");

            // Load references so we can get the main type later
            var (referencedAssemblies, appCodeAssemblyResult, appCodeAssembly) = roslynBuildManager.ReferencedAssemblies(codeFileInfo, spec);

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
                MainType = mainType
            };

            // Log performance metrics
            stopwatch.Stop();
            var loadTimeMs = stopwatch.ElapsedMilliseconds;
            l.A($"Disk cache load completed in {loadTimeMs}ms (size: {assemblyBytes.Length / 1024}KB)");
            
            // Warn if exceeding performance target
            if (loadTimeMs > 100)
                l.A($"⚠️ Performance: Load time {loadTimeMs}ms exceeds 100ms target");

            return l.Return(result, $"Cache hit - loaded from disk in {loadTimeMs}ms");
        }
        catch (BadImageFormatException ex)
        {
            // Corrupted cache - auto-recovery
            l.Ex(ex);
            l.A("Corrupted cache file detected - will trigger recompilation");
            
            // Delete corrupted file
            try
            {
                var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
                var cacheKey = new CacheKey(spec.AppId, spec.Edition, normalizedPath, contentHash, appCodeHash);
                var cachePath = cacheKey.GetFilePath(GetCacheDirectoryPath());
                if (File.Exists(cachePath))
                {
                    File.Delete(cachePath);
                    l.A($"Deleted corrupted cache file: {cachePath}");
                }
            }
            catch (Exception deleteEx)
            {
                l.Ex(deleteEx);
            }

            return l.ReturnNull();
        }
        catch (IOException ex)
        {
            // Graceful handling of I/O errors
            l.Ex(ex);
            l.A("I/O error loading from cache - will trigger recompilation");
            return l.ReturnNull();
        }
        catch (Exception ex)
        {
            // Catch-all for unexpected errors
            l.Ex(ex);
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

        // Check feature flag first
        if (!IsEnabled())
        {
            l.A("Disk cache disabled via feature flag");
            return l.ReturnFalse("disabled");
        }

        // Performance measurement
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Generate cache key
            var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
            var cacheKey = new CacheKey(spec.AppId, spec.Edition, normalizedPath, contentHash, appCodeHash);
            
            // Ensure cache directory exists
            var cacheDir = GetCacheDirectoryPath();
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
                l.A($"Created cache directory: {cacheDir}");
            }

            // Get source assembly path
            var sourceAssemblyPath = assemblyResult.Assembly.Location;
            if (string.IsNullOrEmpty(sourceAssemblyPath))
            {
                l.A("Assembly has no location - cannot cache to disk");
                return l.ReturnFalse("no-location");
            }

            var cachePath = cacheKey.GetFilePath(cacheDir);

            // If assembly already compiled to the target cache path, skip copying
            if (string.Equals(Path.GetFullPath(sourceAssemblyPath), Path.GetFullPath(cachePath), StringComparison.OrdinalIgnoreCase))
            {
                stopwatch.Stop();
                l.A($"Assembly already at cache location: {cachePath}");
                return l.ReturnTrue("already-in-cache");
            }

            // Copy assembly DLL to cache path
            // Using synchronous File.Copy for reliability
            File.Copy(sourceAssemblyPath, cachePath, overwrite: true);
            
            var fileInfo = new FileInfo(cachePath);
            
            // Log performance metrics
            stopwatch.Stop();
            var saveTimeMs = stopwatch.ElapsedMilliseconds;
            l.A($"Saved {fileInfo.Length / 1024}KB to disk cache in {saveTimeMs}ms: {cachePath}");
            
            // Warn if exceeding performance target
            if (saveTimeMs > 200)
                l.A($"⚠️ Performance: Save time {saveTimeMs}ms exceeds 200ms target");

            return l.ReturnTrue($"saved in {saveTimeMs}ms");
        }
        catch (IOException ex)
        {
            // Graceful handling (disk space exhaustion, etc.)
            l.Ex(ex);
            l.A("I/O error saving to cache - will fallback to memory-only caching");
            return l.ReturnFalse("io-error");
        }
        catch (UnauthorizedAccessException ex)
        {
            // Permission errors
            l.Ex(ex);
            l.A("Permission error saving to cache - will fallback to memory-only caching");
            return l.ReturnFalse("permission-error");
        }
        catch (Exception ex)
        {
            // Catch-all for unexpected errors
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    /// <summary>
    /// Invalidates (deletes) cached assemblies for a specific template.
    /// </summary>
    public void InvalidateCache(string templateRelativePath)
    {
        var l = Log.Fn($"template:{templateRelativePath}");

        try
        {
            var cacheDir = GetCacheDirectoryPath();
            if (!Directory.Exists(cacheDir))
            {
                l.A("Cache directory does not exist - nothing to invalidate");
                l.Done();
                return;
            }

            // Search for files matching template path pattern
            var normalizedPath = CacheKey.NormalizePath(templateRelativePath);
            var searchPattern = $"*-{normalizedPath}-*.dll";
            
            var matchingFiles = Directory.GetFiles(cacheDir, searchPattern);
            foreach (var file in matchingFiles)
            {
                try
                {
                    File.Delete(file);
                    l.A($"Deleted cache file: {file}");
                }
                catch (Exception ex)
                {
                    // Non-critical failure - log and continue
                    l.Ex(ex);
                }
            }

            l.A($"Invalidated {matchingFiles.Length} cache files for template {templateRelativePath}");
        }
        catch (Exception ex)
        {
            // Non-critical failure
            l.Ex(ex);
        }

        l.Done();
    }

    /// <summary>
    /// Invalidates all cached assemblies for a specific app and edition.
    /// </summary>
    public void InvalidateAppCache(int appId, string edition)
    {
        var l = Log.Fn($"app:{appId}, edition:{edition}");

        try
        {
            var cacheDir = GetCacheDirectoryPath();
            if (!Directory.Exists(cacheDir))
            {
                l.A("Cache directory does not exist - nothing to invalidate");
                l.Done();
                return;
            }

            // Search for files matching app pattern
            var searchPattern = $"app-{appId}-{edition}-*.dll";
            
            var matchingFiles = Directory.GetFiles(cacheDir, searchPattern);
            foreach (var file in matchingFiles)
            {
                try
                {
                    File.Delete(file);
                    l.A($"Deleted cache file: {file}");
                }
                catch (Exception ex)
                {
                    // Non-critical failure - log and continue
                    l.Ex(ex);
                }
            }

            l.A($"Invalidated {matchingFiles.Length} cache files for app {appId} edition {edition}");
        }
        catch (Exception ex)
        {
            // Non-critical failure
            l.Ex(ex);
        }

        l.Done();
    }

    /// <summary>
    /// Checks if disk caching is enabled based on feature flag.
    /// </summary>
    public bool IsEnabled() =>
        // Not cached - check on every call for runtime toggle support
        featureService.Value.IsEnabled(Sxc.Sys.Configuration.SxcFeatures.RazorCacheCompiledToDisk.NameId);

    /// <summary>
    /// Gets the physical path to the cache directory.
    /// </summary>
    public string GetCacheDirectoryPath()
    {
        // Get base temp assembly folder (App_Data/2sxc.bin)
        var tempAssemblyFolder = globalConfiguration.TempAssemblyFolder();
        
        // Add cshtml subfolder
        return Path.Combine(tempAssemblyFolder, CacheSubFolder);
    }

    /// <summary>
    /// Computes SHA256 hash of the given content string.
    /// </summary>
    /// <remarks>
    /// SHA256 is FIPS-compliant and performant for typical template sizes.
    /// </remarks>
    public string ComputeContentHash(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(sourceCode);
        var hashBytes = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
