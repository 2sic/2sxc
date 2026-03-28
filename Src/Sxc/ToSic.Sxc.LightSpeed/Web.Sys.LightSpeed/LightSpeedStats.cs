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

    public Dictionary<int, LightSpeedStat> AppsWithCount =>
        All
            .GroupBy(i => i.AppId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var size = Estimator.EstimateMany(g.ToArray<object>()).Total;
                    var compressed = g.Where(i => (i?.Data as IOptimizeMemory)?.UseCompression == true);
                    var compressedSize = Estimator.EstimateMany(compressed.ToArray<object>()).Total;
                    return new LightSpeedStat(g.Count(), size, compressedSize, size - compressedSize);
                });


    //public Dictionary<int, int> Size => All
    //    .GroupBy(i => i.AppId)
    //    .ToDictionary(
    //        g => g.Key,
    //        g => Estimator.EstimateMany(g.ToArray<object>()).Total
    //    );

    private List<OutputCacheItem> All => _all.Get(() => MemoryCache.Default
        .Where(pair => pair.Key.StartsWith(OutputCacheKeys.GlobalCacheKeyModuleRoot) || pair.Key.StartsWith(OutputCacheKeys.GlobalCacheKeyPartialRoot))
        .Select(pair => (pair.Value as OutputCacheItem)!)
        .Where(p => p != null!)
        .ToList()
    )!;
    private readonly GetOnce<List<OutputCacheItem>> _all = new();
}

public record LightSpeedStat(int Count, long Total, long Compressed, long Uncompressed);