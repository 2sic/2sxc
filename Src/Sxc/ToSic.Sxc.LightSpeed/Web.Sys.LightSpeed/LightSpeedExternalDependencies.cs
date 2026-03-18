using System.Runtime.Caching;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LightSpeedExternalDependencies(MemoryCacheService memoryCacheService)
    : ServiceBase(SxcLogName + ".LsExtDp", connect: [memoryCacheService])
{
    public IReadOnlyList<string> GetOrEnsureCacheKeys(int appId, IEnumerable<string>? dependencies)
    {
        // Every LightSpeed entry also watches the app-wide key so a single touch can invalidate
        // all LightSpeed entries of the app without purging unrelated app caches.
        var appKey = GetOrEnsureAppCacheKey(appId);
        var normalized = NormalizeDependencies(dependencies);
        if (normalized.Count == 0)
            return [appKey];

        var keys = normalized
            .Select(name => OutputCacheKeys.ExternalDependencyKey(appId, name))
            .ToArray();

        foreach (var key in keys)
            EnsureMarker(key);

        return [appKey, .. keys];
    }

    public string GetOrEnsureAppCacheKey(int appId)
    {
        var key = OutputCacheKeys.AppDependencyKey(appId);
        EnsureMarker(key);
        return key;
    }

    public int Touch(int appId, IEnumerable<string>? dependencies)
    {
        var keys = NormalizeDependencies(dependencies)
            .Select(name => OutputCacheKeys.ExternalDependencyKey(appId, name))
            .ToArray();

        foreach (var key in keys)
            SetMarker(key);

        return keys.Length;
    }

    // Used for the app-wide LightSpeed flush path when no specific dependency names are supplied.
    public void TouchApp(int appId)
        => SetMarker(GetOrEnsureAppCacheKey(appId));

    public IReadOnlyList<string> NormalizeDependencies(IEnumerable<string>? dependencies)
    {
        if (dependencies == null)
            return [];

        var normalized = dependencies
            .Select(NormalizeDependency)
            .Where(name => name != null)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.Ordinal)
            .Cast<string>()
            .ToArray();

        return normalized;
    }

    private static string? NormalizeDependency(string? dependency)
    {
        if (dependency == null)
            return null;

        var trimmed = dependency.Trim();
        if (trimmed.Length == 0)
            return null;

        return trimmed.ToLowerInvariant();
    }

    private void EnsureMarker(string cacheKey)
    {
        if (memoryCacheService.TryGet<OutputCacheDependencyMarker>(cacheKey, out _))
            return;

        SetMarker(cacheKey);
    }

    private void SetMarker(string cacheKey)
        => memoryCacheService.Set(
            cacheKey,
            new OutputCacheDependencyMarker(),
            policy => policy.SetAbsoluteExpiration(ObjectCache.InfiniteAbsoluteExpiration)
        );

    private sealed class OutputCacheDependencyMarker : ITimestamped
    {
        long ITimestamped.CacheTimestamp { get; } = DateTime.Now.Ticks;
    }
}
