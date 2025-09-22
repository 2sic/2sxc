using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Apps.Sys.Loaders;
using ToSic.Eav.Persistence.Sys.AppState;
using ToSic.Sxc.Oqt.Server.Context;

namespace ToSic.Sxc.Oqt.Server.Apps.Caching;

/// <summary>
/// Multi-tenant AppsCache isolating state per Oqtane tenant (keyed by TenantId).
/// No static fields; each tenant bucket holds independent Zones/App caches.
/// </summary>
internal sealed class OqtPerTenantAppsCache(ITenantKeyProvider keyProvider)
    : AppsCacheBase, IAppsCacheSwitchable
{
    private sealed class Bucket
    {
        public volatile IReadOnlyDictionary<int, Zone>? ZonesCache;
        public readonly object ZonesLoadLock = new();
        public readonly Dictionary<string, IAppStateCache> AppCaches = new();
    }

    private readonly ConcurrentDictionary<string, Bucket> _buckets = new(StringComparer.Ordinal);

    private Bucket Current => _buckets.GetOrAdd(keyProvider.GetKey(), _ => new Bucket());

    public override string NameId => "OqtPerTenantCache";
    public override bool IsViable() => true;
    public override int Priority => 10;

    public override IReadOnlyDictionary<int, Zone> Zones(IAppLoaderTools tools)
    {
        var b = Current;
        if (b.ZonesCache != null) return b.ZonesCache;

        lock (b.ZonesLoadLock)
        {
            b.ZonesCache ??= LoadZones(tools);
            return b.ZonesCache;
        }
    }

    protected override bool Has(string cacheKey) => Current.AppCaches.ContainsKey(cacheKey);

    protected override void Set(string key, IAppStateCache item)
    {
        var bucket = Current;
        lock (bucket.AppCaches)
            bucket.AppCaches[key] = item;
    }

    protected override IAppStateCache? Get(string key)
        => Current.AppCaches.TryGetValue(key, out var v) ? v : null;

    protected override void Remove(string key)
        => Current.AppCaches.Remove(key);

    public override void PurgeZones() => Current.ZonesCache = null;
}