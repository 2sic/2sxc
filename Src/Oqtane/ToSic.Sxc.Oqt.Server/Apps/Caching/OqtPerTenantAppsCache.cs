using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using System.Collections.Concurrent;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Apps.Sys.Loaders;

namespace ToSic.Sxc.Oqt.Server.Apps.Caching;

/// <summary>
/// Multi-tenant AppsCache isolating state per Oqtane tenant (keyed by TenantId).
/// No static fields; each tenant bucket holds independent Zones/App caches.
/// </summary>
internal sealed class OqtPerTenantAppsCache(IHttpContextAccessor httpContextAccessor)
    : AppsCacheBase, IAppsCacheSwitchable
{
    private sealed class Bucket
    {
        public readonly object Lock = new();
        public volatile IReadOnlyDictionary<int, Zone>? ZonesCache;
        public readonly Dictionary<string, IAppStateCache> AppCaches = new();
    }

    private readonly ConcurrentDictionary<int, Bucket> _buckets = new();

    private Bucket Current => _buckets.GetOrAdd(GetBucketKey(), _ => new Bucket());

    public override string NameId => "OqtPerTenantCache";
    public override bool IsViable() => true;
    public override int Priority => 10;

    public override IReadOnlyDictionary<int, Zone> Zones(IAppLoaderTools tools)
    {
        var b = Current;
        if (b.ZonesCache != null) return b.ZonesCache;

        lock (b.Lock)
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

    // Build a per-request tenant key without injecting scoped services.
    private int GetBucketKey()
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx?.RequestServices == null) return 1;

        // Prefer TenantManager if available
        var tenantManager = ctx.RequestServices.GetService<ITenantManager>();
        var tenantId = tenantManager?.GetTenant()?.TenantId;
        if (tenantId.HasValue) return tenantId.Value;

        // Fallback to alias (in case TenantManager is not available)
        var aliasAccessor = ctx.RequestServices.GetService<IAliasAccessor>();
        var aliasTenantId = aliasAccessor?.Alias?.TenantId;
        if (aliasTenantId.HasValue) return aliasTenantId.Value;

        return 1;
    }
}