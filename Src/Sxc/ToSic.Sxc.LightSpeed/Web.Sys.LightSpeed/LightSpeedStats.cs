using System.Runtime.Caching;
using ToSic.Sys.Caching;
using ToSic.Sys.Memory;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

/// <summary>
/// Statistics for LightSpeed
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LightSpeedStats(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".LightSpeedStats", connect: [memoryCacheService])
{
    private MemorySizeEstimator Estimator => field ??= new(Log);

    private static string[] CacheKeyPrefixes =>
    [
        OutputCacheKeys.GlobalCacheKeyModuleRoot,
        OutputCacheKeys.GlobalCacheKeyPartialRoot
    ];

    public Dictionary<int, LightSpeedStat> GetStats()
    {
        var all = MemoryCache.Default
            .Where(pair => CacheKeyPrefixes.Any(prefix => pair.Key.StartsWith(prefix)))
            .Select(pair => (pair.Value as OutputCacheItem)!)
            .Where(p => p != null!)
            .ToList();

        var allStats = all
            .GroupBy(i => i.AppId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var stats = Estimator.EstimateMany(g.ToArray<object>());
                    var compressed = g.Where(i => (i?.Data as IOptimizeMemory)?.UseCompression == true);
                    var compressedStats = Estimator.EstimateMany(compressed.ToArray<object>());

                    return new LightSpeedStat(
                        g.Count(),
                        stats.Total,
                        compressedStats.Total,
                        stats.Total - compressedStats.Total,
                        compressedStats.Expanded,
                        stats.Total - compressedStats.Total + compressedStats.Expanded
                    );
                });

        return allStats;
    }

}

public record LightSpeedStat(int Count, long MemoryUse, long Compressed, long Uncompressed, long Expanded, long GrandTotal);