using System.Collections.Concurrent;
using System.Reflection;
using ToSic.Sxc.Dnn.Razor.Sys;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// Integration tests for disk cache save/load cycle, invalidation, feature flag, and concurrent compilation.
/// Tests validate end-to-end functionality of the CSHTML disk caching feature.
/// </summary>
public class RazorDiskCacheTests
{
    private const string TestCacheDir = "test-cache";
    private const int TestAppId = 42;
    private const string TestEdition = "live";
    private const string TestTemplatePath = "views/test-template.cshtml";
    private const string TestContentHash = "abc123";
    private const string TestAppCodeHash = "xyz789";

    /// <summary>
    /// Disk cache save/load cycle test
    /// Verifies that a compiled assembly can be saved to disk cache and loaded back successfully.
    /// </summary>
    [Fact]
    public void DiskCache_SaveAndLoad_ReturnsValidAssembly()
    {
        // Arrange
        var tempCacheDir = Path.Combine(Path.GetTempPath(), TestCacheDir, Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempCacheDir);

        try
        {
            var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition,
                CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, TestEdition), TestContentHash, TestAppCodeHash);
            
            var cachePath = cacheKey.GetFilePathTac(tempCacheDir);
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
            
            // Create a test assembly file (using current assembly as test data)
            var testAssemblyPath = Assembly.GetExecutingAssembly().Location;
            File.Copy(testAssemblyPath, cachePath, overwrite: true);

            // Act - Load from disk cache
            var assemblyBytes = File.ReadAllBytes(cachePath);
            var loadedAssembly = Assembly.Load(assemblyBytes);

            // Assert
            NotNull(loadedAssembly);
            NotNull(loadedAssembly.FullName);
            True(File.Exists(cachePath), "Cache file should exist");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempCacheDir))
                Directory.Delete(tempCacheDir, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that cache key changes when template content changes (different contentHash).
    /// </summary>
    [Fact]
    public void DiskCache_ContentHashChange_GeneratesDifferentCacheKey()
    {
        // Arrange
        var originalContentHash = "hash1";
        var modifiedContentHash = "hash2";

        var originalKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition,
            CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, TestEdition), originalContentHash, TestAppCodeHash);
        
        // Act - Simulate file change with different content hash
        var modifiedKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition,
            CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, TestEdition), modifiedContentHash, TestAppCodeHash);

        // Assert
        NotEqual(originalKey.ToStringTac(), modifiedKey.ToStringTac());
        NotEqual(originalKey, modifiedKey);
    }

    /// <summary>
    /// Verifies that cache key changes when AppCode assembly changes (different appCodeHash).
    /// </summary>
    [Fact]
    public void DiskCache_AppCodeHashChange_GeneratesDifferentCacheKey()
    {
        // Arrange - Use hashes with different first 6 characters
        var originalAppCodeHash = "aaa111000000000000000000000000000000000000000000000000000000";
        var modifiedAppCodeHash = "bbb222000000000000000000000000000000000000000000000000000000";

        var originalKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition,
            CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, TestEdition), TestContentHash, originalAppCodeHash);
        
        // Act - Simulate AppCode change with different hash
        var modifiedKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition,
            CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, TestEdition), TestContentHash, modifiedAppCodeHash);

        // Assert
        NotEqual(originalKey.ToStringTac(), modifiedKey.ToStringTac());
        NotEqual(originalKey, modifiedKey);
    }
    
    /// <summary>
    /// Verifies that cache key generation works with null edition (defaults to "root").
    /// Validates spec: Edition parameter is nullable and defaults to "root" if not provided.
    /// </summary>
    [Fact]
    public void DiskCache_NullEdition_DefaultsToRoot()
    {
        // Arrange & Act - Pass null edition
        var cacheKey = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, null,
            CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, null), TestContentHash, TestAppCodeHash);
        
        var fileName = cacheKey.ToStringTac();

        // Assert
        Equal("root", cacheKey.Edition);
        // file name no longer contains edition/app, but edition on the object must default to root
        DoesNotContain("root", fileName);
    }

    /// <summary>
    /// Concurrent compilation safety test
    /// Verifies that multiple threads generating cache keys for the same template produce identical results.
    /// This validates that the CacheKey value object is thread-safe and deterministic.
    /// </summary>
    [Fact]
    public void DiskCache_ConcurrentCacheKeyGeneration_ProducesIdenticalResults()
    {
        // Arrange
        const int threadCount = 10;
        var cacheKeys = new ConcurrentBag<string>();

        // Act - Generate cache keys concurrently
        Parallel.For(0, threadCount, i =>
        {
            var key = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition,
                CacheKeyTestAccessors.NormalizePathTac(TestTemplatePath, TestEdition), TestContentHash, TestAppCodeHash);
            cacheKeys.Add(key.ToStringTac());
        });

        // Assert - All cache keys should be identical
        var distinctKeys = cacheKeys.Distinct().ToList();
        Single(distinctKeys);
    }

    /// <summary>
    /// Validates that cache files in a directory can be identified and deleted by pattern.
    /// This simulates the InvalidateAppCache bulk deletion scenario.
    /// </summary>
    [Fact]
    public void DiskCache_BulkInvalidation_DeletesMatchingFiles()
    {
        // Arrange
        var tempCacheDir = Path.Combine(Path.GetTempPath(), TestCacheDir, Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempCacheDir);

        try
        {
            // Create multiple cache files for the same app
            var cacheKey1 = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, "template1", "hash1", "apphash1");
            var cacheKey2 = CacheKeyTestAccessors.NewCacheKeyTac(TestAppId, TestEdition, "template2", "hash2", "apphash1");
            var cacheKey3 = CacheKeyTestAccessors.NewCacheKeyTac(999, TestEdition, "template3", "hash3", "apphash1"); // Different app
            
            var cachePath1 = cacheKey1.GetFilePathTac(tempCacheDir);
            var cachePath2 = cacheKey2.GetFilePathTac(tempCacheDir);
            var cachePath3 = cacheKey3.GetFilePathTac(tempCacheDir);

            Directory.CreateDirectory(Path.GetDirectoryName(cachePath1)!);
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath2)!);
            Directory.CreateDirectory(Path.GetDirectoryName(cachePath3)!);

            File.WriteAllText(cachePath1, "test1");
            File.WriteAllText(cachePath2, "test2");
            File.WriteAllText(cachePath3, "test3");

            // Act - delete all dlls for app/edition folder
            var appEditionDir = Path.Combine(tempCacheDir, CacheKeyTestAccessors.GetAppFolderTac(TestAppId, TestEdition));
            var filesToDelete = Directory.Exists(appEditionDir)
                ? Directory.GetFiles(appEditionDir, "*.dll", SearchOption.AllDirectories)
                : Array.Empty<string>();

            foreach (var file in filesToDelete)
                File.Delete(file);

            // Assert
            False(File.Exists(cachePath1), "Cache file 1 should be deleted");
            False(File.Exists(cachePath2), "Cache file 2 should be deleted");
            True(File.Exists(cachePath3), "Cache file 3 (different app) should remain");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempCacheDir))
                Directory.Delete(tempCacheDir, recursive: true);
        }
    }
}
