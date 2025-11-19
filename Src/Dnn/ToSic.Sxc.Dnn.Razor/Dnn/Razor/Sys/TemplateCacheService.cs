using System.Reflection;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Service responsible for managing template cache operations (memory and disk).
/// </summary>
public class TemplateCacheService(
    AssemblyCacheManager assemblyCacheManager,
    IAssemblyDiskCacheService diskCacheService,
    LazySvc<AppCodeLoader> appCodeLoader)
    : ServiceBase("Dnn.TmpCchSvc", connect: [assemblyCacheManager, diskCacheService, appCodeLoader])
{
    /// <summary>
    /// Try to get cached assembly from memory or disk cache.
    /// </summary>
    public AssemblyResult TryGetFromCache(CodeFileInfo codeFileInfo, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{codeFileInfo}");

        // Check memory cache first
        var memoryResult = assemblyCacheManager.TryGetTemplate(codeFileInfo.FullPath!);
        if (memoryResult?.MainType != null)
            return l.Return(memoryResult, "from memory cache");

        // Memory cache miss - try disk cache
        l.A("Memory cache miss - checking disk cache");

        if (!diskCacheService.IsEnabled())
        {
            l.A("Disk cache disabled via feature flag");
            return l.ReturnNull("disk cache disabled");
        }

        // Use the same AppCode snapshot (hash + dependency) for disk lookups and later caching
        var appCodeInfo = GetAppCodeCacheInfo(spec);
        var contentHash = diskCacheService.ComputeContentHash(codeFileInfo.SourceCode);
        var diskResult = diskCacheService.TryLoadFromCache(
            spec,
            codeFileInfo.RelativePath,
            contentHash,
            appCodeInfo.Hash,
            appCodeInfo.AssemblyResult,
            codeFileInfo);
        
        if (diskResult == null)
            return l.ReturnNull("disk cache miss");

        l.A($"Disk cache hit for template: {codeFileInfo.RelativePath}");
        
        // Add to memory cache for faster subsequent access
        AddToMemoryCache(codeFileInfo, diskResult, [codeFileInfo.FullPath], diskResult.AppCodeDependency == null ? null : [diskResult.AppCodeDependency]);
        
        return l.Return(diskResult, "from disk cache");
    }

    /// <summary>
    /// Add compiled assembly to memory and disk cache.
    /// </summary>
    public void AddToCache(CodeFileInfo codeFileInfo, HotBuildSpec spec, AssemblyResult assemblyResult, 
        string contentHash, string appCodeHash, AssemblyResult appCodeAssemblyResult)
    {
        var l = Log.Fn($"{codeFileInfo}");

        // Add to memory cache
        var dependencies = appCodeAssemblyResult == null ? null : new ICanBeCacheDependency[] { appCodeAssemblyResult };
        AddToMemoryCache(codeFileInfo, assemblyResult, [codeFileInfo.FullPath], dependencies);

        // Save to disk cache
        diskCacheService.TrySaveToCache(spec, codeFileInfo.RelativePath, contentHash, appCodeHash, assemblyResult);

        l.Done();
    }

    /// <summary>
    /// Gets the hash of the AppCode assembly for cache invalidation.
    /// </summary>
    public string GetAppCodeHash(HotBuildSpec spec)
        => GetAppCodeCacheInfo(spec).Hash;

    internal AppCodeCacheInfo GetAppCodeCacheInfo(HotBuildSpec spec)
    {
        var l = Log.Fn<AppCodeCacheInfo>($"{spec}");

        var (appCodeAssemblyResult, _) = appCodeLoader.Value.GetAppCode(spec);
        var hash = ComputeAppCodeHash(appCodeAssemblyResult?.Assembly);

        return l.Return(new AppCodeCacheInfo(appCodeAssemblyResult, hash));
    }

    private static string ComputeAppCodeHash(Assembly appCodeAssembly)
    {
        if (appCodeAssembly == null)
            return string.Empty;

        var assemblyFullName = appCodeAssembly.FullName ?? string.Empty;

        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(assemblyFullName);
        var hashBytes = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    private void AddToMemoryCache(CodeFileInfo codeFileInfo, AssemblyResult assemblyResult, 
        string[] filePaths, ICanBeCacheDependency[] dependencies = null)
    {
        assemblyResult.CacheDependencyId = AssemblyCacheManager.KeyTemplate(codeFileInfo.FullPath!);
        assemblyCacheManager.Add(
            cacheKey: assemblyResult.CacheDependencyId,
            data: assemblyResult,
            slidingDuration: CacheConstants.DurationRazorAndCode,
            filePaths: filePaths,
            dependencies: dependencies
        );
    }

    internal sealed class AppCodeCacheInfo(AssemblyResult assemblyResult, string hash)
    {
        public AssemblyResult AssemblyResult { get; } = assemblyResult;
        public string Hash { get; } = hash;
    }
}
