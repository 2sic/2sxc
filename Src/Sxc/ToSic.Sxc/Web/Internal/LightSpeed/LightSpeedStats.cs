using System.Runtime.Caching;
using ToSic.Eav.Caching;
using ToSic.Eav.DataSource.Internal.Caching;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

// note: this should probably not be in eav.core
/// <summary>
/// Statistics for LightSpeed
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LightSpeedStats(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".LightSpeedStats", connect: [memoryCacheService])
{
    const string KeyPrefix = "LightSpeedStats-";
    public Dictionary<int, int> ItemsCount => All.GroupBy(i => i.AppId).ToDictionary(g => g.Key, g => g.Count());
    public Dictionary<int, int> Size => All.GroupBy(i => i.AppId).ToDictionary(g => g.Key, g => g.Sum(i => i.Size));

    public void AddSize(int appId, int size, string dependOnCacheKey)
    {
        var cacheKey = $"{KeyPrefix}{dependOnCacheKey}";
        var value = new LightSpeedCacheItemSize { AppId = appId, Size = size, CachePartialKey = KeyPrefix , CacheFullKey = cacheKey, CacheTimestamp = DateTime.Now.Ticks};
        memoryCacheService.Set(cacheKey, value, absoluteExpiration: ObjectCache.InfiniteAbsoluteExpiration, cacheKeys: [dependOnCacheKey]);
    }

    private List<LightSpeedCacheItemSize> All => _all.Get(()=> MemoryCache.Default
        .Where(pair => pair.Key.StartsWith(KeyPrefix))
        .Select(pair => (LightSpeedCacheItemSize)pair.Value)
        .ToList());
    private readonly GetOnce<List<LightSpeedCacheItemSize>> _all = new();

    private struct LightSpeedCacheItemSize : ICacheInfo
    {
        public int AppId { get; init; }
        public int Size { get; init; }
        public string CachePartialKey { get; init; }
        public string CacheFullKey { get; init; }
        public long CacheTimestamp { get; init; }
        public bool CacheChanged(long dependentTimeStamp) => true;
    }
}
