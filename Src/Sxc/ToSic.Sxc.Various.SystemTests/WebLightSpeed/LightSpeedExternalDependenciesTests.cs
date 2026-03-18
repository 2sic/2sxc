using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcWithDb))]
public class LightSpeedExternalDependenciesTests(ExecutionContext exCtx)
{
    private LightSpeedExternalDependencies Dependencies => exCtx.GetService<LightSpeedExternalDependencies>(reuse: true);

    private OutputCacheService OutputCache => exCtx.GetService<OutputCacheService>(reuse: true);

    private ModuleHtmlService ModuleHtml => (ModuleHtmlService)exCtx.GetService<IModuleHtmlService>(reuse: true);

    private MemoryCacheService Cache => exCtx.GetService<MemoryCacheService>(reuse: true);

    [Fact]
    public void NormalizeDependenciesAreScopedByApp()
    {
        var app7Keys = Dependencies.GetOrEnsureCacheKeys(7, [" Products ", "pricing", "PRODUCTS"]);
        var app8Keys = Dependencies.GetOrEnsureCacheKeys(8, ["products"]);

        Equal(3, app7Keys.Count);
        Contains(OutputCacheKeys.AppDependencyKey(7), app7Keys);
        Contains(OutputCacheKeys.ExternalDependencyKey(7, "products"), app7Keys);
        Contains(OutputCacheKeys.ExternalDependencyKey(7, "pricing"), app7Keys);
        Equal(2, app8Keys.Count);
        Contains(OutputCacheKeys.AppDependencyKey(8), app8Keys);
        Contains(OutputCacheKeys.ExternalDependencyKey(8, "products"), app8Keys);
    }

    [Fact]
    public void GetOrEnsureCacheKeysWithoutDependenciesReturnsAppKeyOnly()
    {
        var keys = Dependencies.GetOrEnsureCacheKeys(7, null);

        Equal([OutputCacheKeys.AppDependencyKey(7)], keys);
    }

    [Fact]
    public void DependOnMergesKeysOncePerModuleRender()
    {
        OutputCache.ModuleId = 42;

        OutputCache.DependOn("Products");
        OutputCache.DependOn(" products ");

        var settings = ModuleHtml.GetOutputCache(42);

        NotNull(settings);
        var dependencies = settings!.ExternalDependencyKeys;
        NotNull(dependencies);
        var dependency = Single(dependencies!);
        Equal("Products", dependency);
    }

    [Fact]
    public void TouchInvalidatesOnlyMatchingCacheEntries()
    {
        var entryKeyA = nameof(TouchInvalidatesOnlyMatchingCacheEntries) + "-a";
        var entryKeyB = nameof(TouchInvalidatesOnlyMatchingCacheEntries) + "-b";
        var dependencyKeysA = Dependencies.GetOrEnsureCacheKeys(7, ["products"]);
        var dependencyKeysB = Dependencies.GetOrEnsureCacheKeys(7, ["pricing"]);

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(dependencyKeysA));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(dependencyKeysB));

        NotNull(Cache.Get<OutputCacheItem>(entryKeyA));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyB));

        Dependencies.Touch(7, ["products"]);

        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyB));
    }

    [Fact]
    public void TouchAppInvalidatesAllMatchingAppCacheEntriesOnly()
    {
        var entryKeyA = nameof(TouchAppInvalidatesAllMatchingAppCacheEntriesOnly) + "-a";
        var entryKeyB = nameof(TouchAppInvalidatesAllMatchingAppCacheEntriesOnly) + "-b";
        var entryKeyC = nameof(TouchAppInvalidatesAllMatchingAppCacheEntriesOnly) + "-c";

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureCacheKeys(7, ["products"])));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureCacheKeys(7, ["pricing"])));
        Cache.Set(entryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureCacheKeys(8, ["products"])));

        Dependencies.TouchApp(7);

        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        Null(Cache.Get<OutputCacheItem>(entryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyC));
    }

    [Fact]
    public void TouchUnknownKeyDoesNotInvalidateExistingEntries()
    {
        var entryKey = nameof(TouchUnknownKeyDoesNotInvalidateExistingEntries);
        var dependencyKeys = Dependencies.GetOrEnsureCacheKeys(7, ["products"]);

        Cache.Set(entryKey, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(dependencyKeys));
        NotNull(Cache.Get<OutputCacheItem>(entryKey));

        Dependencies.Touch(7, ["unknown"]);

        NotNull(Cache.Get<OutputCacheItem>(entryKey));
    }
}
