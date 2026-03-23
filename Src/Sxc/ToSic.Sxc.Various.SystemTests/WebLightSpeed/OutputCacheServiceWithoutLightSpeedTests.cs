using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcWithDbWithoutLightSpeed))]
public class OutputCacheManagementServiceWithoutLightSpeedTests
{
    private const int CurrentAppId = 7;
    private readonly ExecutionContext exCtx;

    public OutputCacheManagementServiceWithoutLightSpeedTests(ExecutionContext exCtx) => this.exCtx = exCtx;

    private IOutputCacheManagementService OutputCacheManagement => exCtx.GetService<IOutputCacheManagementService>(reuse: true);
    private INamedCacheDependencyService Dependencies => exCtx.GetService<INamedCacheDependencyService>(reuse: true);
    private MemoryCacheService Cache => exCtx.GetService<MemoryCacheService>(reuse: true);
    private int OtherAppId => CurrentAppId + 1;

    [Fact]
    public void OutputCacheManagementServiceResolvesWithoutLightSpeed()
        => NotNull(OutputCacheManagement);

    [Fact]
    public void FlushSelectiveDependenciesWithoutLightSpeed()
    {
        var entryKeyA = nameof(FlushSelectiveDependenciesWithoutLightSpeed) + "-a";
        var entryKeyB = nameof(FlushSelectiveDependenciesWithoutLightSpeed) + "-b";
        var entryKeyC = nameof(FlushSelectiveDependenciesWithoutLightSpeed) + "-c";

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["products"])));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["pricing"])));
        Cache.Set(entryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, OtherAppId, ["products"])));

        var touched = OutputCacheManagement.Flush(CurrentAppId, [" products ", "PRODUCTS"]);

        Equal(1, touched);
        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyC));
    }

    [Fact]
    public void FlushNullTouchesAppWideMarkerForCurrentApp()
    {
        var entryKeyA = nameof(FlushNullTouchesAppWideMarkerForCurrentApp) + "-a";
        var entryKeyB = nameof(FlushNullTouchesAppWideMarkerForCurrentApp) + "-b";
        var entryKeyC = nameof(FlushNullTouchesAppWideMarkerForCurrentApp) + "-c";

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["products"])));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["pricing"])));
        Cache.Set(entryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, OtherAppId, ["products"])));

        var touched = OutputCacheManagement.Flush(CurrentAppId);

        Equal(0, touched);
        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        Null(Cache.Get<OutputCacheItem>(entryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyC));
    }

    [Fact]
    public void FlushEmptyTouchesAppWideMarkerForCurrentApp()
    {
        var entryKeyA = nameof(FlushEmptyTouchesAppWideMarkerForCurrentApp) + "-a";
        var entryKeyB = nameof(FlushEmptyTouchesAppWideMarkerForCurrentApp) + "-b";
        var entryKeyC = nameof(FlushEmptyTouchesAppWideMarkerForCurrentApp) + "-c";

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["products"])));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["pricing"])));
        Cache.Set(entryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, OtherAppId, ["products"])));

        var touched = OutputCacheManagement.Flush(CurrentAppId, []);

        Equal(0, touched);
        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        Null(Cache.Get<OutputCacheItem>(entryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyC));
    }
}
