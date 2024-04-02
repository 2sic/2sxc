using System.Collections.Concurrent;
using System.Runtime.Caching;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

// note: this should probably not be in eav.core
/// <summary>
/// Static statistics for LightSpeed.
/// They are not in DI on purpose, because otherwise it could end up being added to the cache,
/// and thereby also adding the temporary parent object.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LightSpeedStats
{
    public static ConcurrentDictionary<int, int> ItemsCount { get; } = new();
    public static ConcurrentDictionary<int, long> Size { get; } = new();

    public static void AddStatic(int appId, int size)
    {
        ItemsCount.AddOrUpdate(appId, 1, (id, count) => count + 1);
        Size.AddOrUpdate(appId, size, (id, before) => before + size);
    }

    public static void RemoveStatic(int appId, int size)
    {
        ItemsCount.AddOrUpdate(appId, /*1*/ 0 /* this is probably more correct*/, (id, count) => count - 1);
        Size.AddOrUpdate(appId, 0, (id, before) => before - size);
    }

    /// <summary>
    /// This is a very important aspect of the cache, because it is used to create a callback that can be used to remove the cache entry.
    ///
    /// IMPORTANT: The call which is generated here MUST be a static, standalone method.
    /// If it were part of the normal code, it would create a reference to the parent object, which would then be added to the cache.
    /// This is because the Lambda could in theory access objects of the surrounding reference.
    ///
    /// So it's really, really important that this is always only called through this method, and that it is always a standalone method.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static CacheEntryUpdateCallback CreateNonCapturingRemoveCall(int appId, int size)
        => _ => RemoveStatic(appId, size);
}