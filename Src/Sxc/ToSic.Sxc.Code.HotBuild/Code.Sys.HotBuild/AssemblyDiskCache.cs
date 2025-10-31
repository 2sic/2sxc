using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ToSic.Sxc.Code.Sys.HotBuild;

/// <summary>
/// Shared service for persisting and retrieving compiled assemblies from disk cache.
/// Supports both Razor templates and AppCode assemblies across .NET Framework and .NET.
/// Platform-neutral implementation with configurable assembly loading.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyDiskCache(NoParamOrder protector = default, object[]? connect = default)
    : ServiceBase("Sxc.AsmDskCch", connect: connect)
{
    /// <summary>
    /// Attempts to load a cached assembly from disk.
    /// </summary>
    /// <param name="cachePath">Full path to the cached assembly file</param>
    /// <param name="loadAssembly">Platform-specific assembly loader function</param>
    /// <param name="featureFlagCheck">Optional feature flag check (null = always enabled)</param>
    /// <returns>Assembly if found and valid; null if cache miss or error</returns>
    /// <remarks>
    /// Gracefully handles:
    /// - File not found (cache miss)
    /// - Corrupted assemblies (auto-deletes)
    /// - I/O errors
    /// - Feature flag disabled
    /// </remarks>
    public Assembly? TryLoadFromCache(
        string cachePath,
        Func<string, Assembly> loadAssembly,
        Func<bool>? featureFlagCheck = null)
    {
        var l = Log.Fn<Assembly?>($"path:{cachePath}", timer: true);

        // Check feature flag if provided
        if (featureFlagCheck != null && !featureFlagCheck())
        {
            l.A("Disk cache disabled via feature flag");
            return l.ReturnNull();
        }

        try
        {
            var fileInfo = new FileInfo(cachePath);
            if (!fileInfo.Exists)
            {
                l.A($"Cache miss: file not found at {cachePath}");
                return l.ReturnNull();
            }

            l.A($"Loading assembly from cache ({fileInfo.Length / 1024}KB)");
            var assembly = loadAssembly(cachePath);
            l.A($"Successfully loaded assembly: {assembly.FullName}");

            return l.Return(assembly, "Cache hit");
        }
        catch (BadImageFormatException ex)
        {
            l.Ex(ex);
            HandleCorruptedCache(cachePath, l);
            return l.ReturnNull();
        }
        catch (IOException ex)
        {
            l.Ex(ex);
            l.A("I/O error loading from cache");
            return l.ReturnNull();
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull();
        }
    }

    /// <summary>
    /// Saves a compiled assembly to disk cache.
    /// </summary>
    /// <param name="sourceAssemblyPath">Source assembly location</param>
    /// <param name="cachePath">Target cache path</param>
    /// <param name="featureFlagCheck">Optional feature flag check</param>
    /// <returns>True if save successful; false otherwise</returns>
    /// <remarks>
    /// Gracefully handles:
    /// - Directory creation
    /// - Disk space exhaustion
    /// - Permission errors
    /// - Assembly already at target location (skip copy)
    /// </remarks>
    public bool TrySaveToCache(
 string sourceAssemblyPath,
        string cachePath,
        Func<bool>? featureFlagCheck = null)
    {
        var l = Log.Fn<bool>($"source:{sourceAssemblyPath} -> cache:{cachePath}", timer: true);

        // Check feature flag if provided
        if (featureFlagCheck != null && !featureFlagCheck())
        {
            l.A("Disk cache disabled via feature flag");
            return l.ReturnFalse("disabled");
        }

        try
        {
            if (string.IsNullOrEmpty(sourceAssemblyPath))
            {
                l.A("Assembly has no location - cannot cache to disk");
                return l.ReturnFalse("no-location");
            }

            // Ensure cache directory exists
            var cacheDir = Path.GetDirectoryName(cachePath);
            if (!string.IsNullOrEmpty(cacheDir) && !Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
                l.A($"Created cache directory: {cacheDir}");
            }

            // Skip if already at target location
            if (string.Equals(Path.GetFullPath(sourceAssemblyPath), Path.GetFullPath(cachePath), StringComparison.OrdinalIgnoreCase))
            {
                l.A($"Assembly already at cache location: {cachePath}");
                return l.ReturnTrue("already-in-cache");
            }

            // Copy assembly to cache
            File.Copy(sourceAssemblyPath, cachePath, overwrite: true);
            var fileInfo = new FileInfo(cachePath);
            l.A($"Saved {fileInfo.Length / 1024}KB to disk cache: {cachePath}");

            return l.ReturnTrue("saved");
        }
        catch (IOException ex)
        {
            l.Ex(ex);
            l.A("I/O error saving to cache - will fallback to memory-only caching");
            return l.ReturnFalse("io-error");
        }
        catch (UnauthorizedAccessException ex)
        {
            l.Ex(ex);
            l.A("Permission error saving to cache - will fallback to memory-only caching");
            return l.ReturnFalse("permission-error");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    /// <summary>
    /// Loads assembly from disk with retry logic for transient errors.
    /// Useful for .NET Framework where antivirus/file locks are common.
    /// </summary>
    /// <param name="assemblyPath">Path to the assembly file</param>
    /// <param name="loadAssembly">Platform-specific assembly loader function</param>
    /// <param name="retryDelayMs">Base delay between retry attempts in milliseconds</param>
    /// <param name="timeoutMs">Maximum time to retry in milliseconds</param>
    /// <returns>Loaded assembly</returns>
    /// <exception cref="IOException">If assembly cannot be loaded after all retries</exception>
    public Assembly LoadWithRetry(
        string assemblyPath,
        Func<string, Assembly> loadAssembly,
        int retryDelayMs = 100,
        int timeoutMs = 3000)
    {
        var l = Log.Fn<Assembly>($"path:{assemblyPath}", timer: true);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var attempt = 0;
        Exception? lastError = null;

        while (stopwatch.ElapsedMilliseconds <= timeoutMs)
        {
            attempt++;
            try
            {
                l.A($"Attempt {attempt} to load cached assembly");

                // Test file accessibility
                using (new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Successfully opened; dispose immediately
                }

                var assembly = loadAssembly(assemblyPath);
                return l.Return(assembly, $"Loaded on attempt {attempt} after {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex) when (IsTransientLoadException(ex))
            {
                lastError = ex;
                var delay = retryDelayMs + new Random().Next(0, 100);
                l.A($"Attempt {attempt} failed ({ex.GetType().Name}). Retry in {delay}ms");
                Thread.Sleep(delay);
            }
            catch (Exception ex)
            {
                l.A($"Non-transient error on attempt {attempt}, giving up");
                l.Ex(ex);
                throw;
            }
        }

        var message = $"Failed to load after {attempt} attempts in {stopwatch.ElapsedMilliseconds}ms (last: {lastError?.GetType().Name}: {lastError?.Message})";
        l.E(message);
        throw new IOException(message, lastError);
    }

    /// <summary>
    /// Invalidates (deletes) cached assemblies matching a search pattern.
    /// Safe to call even if directory doesn't exist.
    /// </summary>
    /// <param name="cacheDirectory">Cache directory path</param>
    /// <param name="searchPattern">File search pattern (e.g., "app-123-*.dll")</param>
    /// <returns>Number of files deleted</returns>
    public int InvalidateCache(string cacheDirectory, string searchPattern)
    {
        var l = Log.Fn<int>($"dir:{cacheDirectory}, pattern:{searchPattern}");

        try
        {
            if (!Directory.Exists(cacheDirectory))
            {
                l.A("Cache directory does not exist - nothing to invalidate");
                return l.Return(0, "no-directory");
            }

            var matchingFiles = Directory.GetFiles(cacheDirectory, searchPattern);
            var deletedCount = 0;

            foreach (var file in matchingFiles)
            {
                try
                {
                    File.Delete(file);
                    l.A($"Deleted: {file}");
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    // Non-critical - log and continue
                    l.Ex(ex);
                }
            }

            return l.Return(deletedCount, $"Invalidated {deletedCount} files");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(0, "error");
        }
    }

    /// <summary>
    /// Computes SHA256 hash of content string.
    /// </summary>
    /// <param name="content">Content to hash</param>
    /// <returns>Lowercase hex string representing SHA256 hash; empty if content is null/empty</returns>
    /// <remarks>
    /// SHA256 is FIPS-compliant and performant for typical content sizes.
    /// Used for cache key generation based on source code content.
    /// </remarks>
    public string ComputeContentHash(string content)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Handles corrupted cache files by attempting deletion.
    /// Called automatically when BadImageFormatException occurs during load.
    /// </summary>
    /// <param name="cachePath">Path to corrupted cache file</param>
    /// <param name="log">Log for diagnostic output</param>
    private void HandleCorruptedCache(string cachePath, ILog log)
    {
        log.A("Corrupted cache file detected - will trigger recompilation");
        try
        {
            if (File.Exists(cachePath))
            {
                File.Delete(cachePath);
                log.A($"Deleted corrupted file: {cachePath}");
            }
        }
        catch (Exception ex)
        {
            log.Ex(ex);
        }
    }

    /// <summary>
    /// Determines if an exception is transient (worth retrying).
    /// </summary>
    /// <param name="ex">Exception to check</param>
    /// <returns>True if exception is likely transient (file lock, antivirus scan, etc.)</returns>
    private static bool IsTransientLoadException(Exception ex)
        => ex is FileNotFoundException
            or IOException
            or UnauthorizedAccessException
            or BadImageFormatException;
}
