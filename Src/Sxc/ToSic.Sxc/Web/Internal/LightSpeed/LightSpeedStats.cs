using System.Collections.Concurrent;

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
}