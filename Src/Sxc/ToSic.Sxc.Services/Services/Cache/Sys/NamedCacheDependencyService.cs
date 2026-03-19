using ToSic.Sys.Caching;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Shared marker service for app-scoped named cache dependencies.
/// Output cache is the first public consumer, but the service is intentionally generic
/// so future data-cache APIs can watch and touch the same kind of dependency markers.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class NamedCacheDependencyService(MemoryCacheService memoryCacheService)
    : ServiceBase($"{SxcLogName}.NmCacDp", connect: [memoryCacheService]), INamedCacheDependencyService
{
    private const string DependencyRoot = "Sxc-Dependency.";
    private static readonly DateTimeOffset MarkerExpiration = DateTimeOffset.MaxValue;

    public IReadOnlyList<string> GetOrEnsureKeys(string scope, int appId, IEnumerable<string>? names)
    {
        // Every scope also has an app-wide marker so callers can invalidate all entries for the app
        // without needing to enumerate existing cache keys.
        var appKey = GetOrEnsureAppKey(scope, appId);
        var normalized = NormalizeNames(names);
        if (normalized.Count == 0)
            return [appKey];

        var keys = normalized
            .Select(name => GetNamedKey(scope, appId, name))
            .ToArray();

        foreach (var key in keys)
            EnsureMarker(key);

        return [appKey, .. keys];
    }

    public string GetOrEnsureAppKey(string scope, int appId)
    {
        var key = GetAppKey(scope, appId);
        EnsureMarker(key);
        return key;
    }

    public int Touch(string scope, int appId, IEnumerable<string>? names)
    {
        var keys = NormalizeNames(names)
            .Select(name => GetNamedKey(scope, appId, name))
            .ToArray();

        foreach (var key in keys)
            SetMarker(key);

        return keys.Length;
    }

    public void TouchApp(string scope, int appId)
        => SetMarker(GetOrEnsureAppKey(scope, appId));

    public IReadOnlyList<string> NormalizeNames(IEnumerable<string>? names)
    {
        if (names == null)
            return [];

        return names
            .Select(NormalizeName)
            .Where(name => name != null)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.Ordinal)
            .Cast<string>()
            .ToArray();
    }

    private static string? NormalizeName(string? name)
    {
        if (name == null)
            return null;

        var trimmed = name.Trim();
        return trimmed.Length == 0
            ? null
            : trimmed.ToLowerInvariant();
    }

    private static string NormalizeScope(string scope)
    {
        if (string.IsNullOrWhiteSpace(scope))
            throw new ArgumentException("Dependency scope must not be empty.", nameof(scope));

        return scope.Trim().ToLowerInvariant();
    }

    private static string GetAppKey(string scope, int appId)
        => $"{DependencyRoot}s:{NormalizeScope(scope)}.a:{appId}";

    private static string GetNamedKey(string scope, int appId, string name)
        => $"{GetAppKey(scope, appId)}.k:{name}";

    private void EnsureMarker(string cacheKey)
    {
        if (memoryCacheService.TryGet<CacheDependencyMarker>(cacheKey, out _))
            return;

        SetMarker(cacheKey);
    }

    private void SetMarker(string cacheKey)
        => memoryCacheService.Set(
            cacheKey,
            new CacheDependencyMarker(),
            policy => policy.SetAbsoluteExpiration(MarkerExpiration)
        );

    private sealed class CacheDependencyMarker : ITimestamped
    {
        long ITimestamped.CacheTimestamp { get; } = DateTime.UtcNow.Ticks;
    }
}
