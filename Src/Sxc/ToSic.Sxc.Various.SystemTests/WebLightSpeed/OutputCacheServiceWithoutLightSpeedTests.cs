using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcWithDbWithoutLightSpeed))]
public class OutputCacheServiceWithoutLightSpeedTests
{
    private const int CurrentAppId = 7;
    private readonly ExecutionContext exCtx;

    public OutputCacheServiceWithoutLightSpeedTests(ExecutionContext exCtx)
    {
        this.exCtx = exCtx;
        ((IExCtxAttachApp)exCtx).AttachApp(new TestApp(CurrentAppId));
    }

    private IOutputCacheService OutputCache => exCtx.GetService<IOutputCacheService>(reuse: true);
    private INamedCacheDependencyService Dependencies => exCtx.GetService<INamedCacheDependencyService>(reuse: true);
    private MemoryCacheService Cache => exCtx.GetService<MemoryCacheService>(reuse: true);
    private int OtherAppId => CurrentAppId + 1;

    [Fact]
    public void OutputCacheServiceResolvesWithoutLightSpeed()
        => NotNull(OutputCache);

    [Fact]
    public void FlushSelectiveDependenciesWithoutLightSpeed()
    {
        var entryKeyA = nameof(FlushSelectiveDependenciesWithoutLightSpeed) + "-a";
        var entryKeyB = nameof(FlushSelectiveDependenciesWithoutLightSpeed) + "-b";
        var entryKeyC = nameof(FlushSelectiveDependenciesWithoutLightSpeed) + "-c";

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["products"])));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, CurrentAppId, ["pricing"])));
        Cache.Set(entryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, OtherAppId, ["products"])));

        var touched = OutputCache.Flush([" products ", "PRODUCTS"]);

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

        var touched = OutputCache.Flush();

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

        var touched = OutputCache.Flush([]);

        Equal(0, touched);
        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        Null(Cache.Get<OutputCacheItem>(entryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyC));
    }
}
