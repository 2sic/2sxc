using System.Runtime.Caching;
using ToSic.Eav.Caching;
using ToSic.Lib.Helpers;
using ToSic.Lib.Memory;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

/// <summary>
/// Statistics for LightSpeed
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LightSpeedStats(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".LightSpeedStats", connect: [memoryCacheService])
{
    public Dictionary<int, int> ItemsCount => All
        .GroupBy(i => i.Data.AppId)
        .ToDictionary(
            g => g.Key,
            g => g.Count()
        );

    public Dictionary<int, int> Size => All
        .GroupBy(i => i.Data.AppId)
        .ToDictionary(
            g => g.Key,
            g => new MemorySizeEstimator(Log).EstimateMany(g.ToArray<object>()).Total
        );

    private List<OutputCacheItem> All => _all.Get(() => MemoryCache.Default
        .Where(pair => pair.Key.StartsWith(OutputCacheManager.GlobalCacheRoot))
        .Select(pair => (OutputCacheItem)pair.Value)
        .ToList()
    );
    private readonly GetOnce<List<OutputCacheItem>> _all = new();
}
