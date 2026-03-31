using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcWithDb))]
public class NamedCacheDependencyServiceTests(ExecutionContext exCtx)
{
    private const string DataCacheScope = "data-cache";

    private INamedCacheDependencyService Dependencies => exCtx.GetService<INamedCacheDependencyService>(reuse: true);

    private ModuleOutputCacheService OutputCache => exCtx.GetService<ModuleOutputCacheService>(reuse: true);

    private ModuleHtmlService ModuleHtml => (ModuleHtmlService)exCtx.GetService<IModuleHtmlService>(reuse: true);

    private MemoryCacheService Cache => exCtx.GetService<MemoryCacheService>(reuse: true);

    [Fact]
    public void KeysAreScopedByAppAndScope()
    {
        var outputApp7Keys = Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, [" Products ", "pricing", "PRODUCTS"]);
        var outputApp8Keys = Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 8, ["products"]);
        var dataApp7Keys = Dependencies.GetOrEnsureKeys(DataCacheScope, 7, ["products"]);

        Equal(["pricing", "products"], Dependencies.NormalizeNames([" Products ", "pricing", "PRODUCTS"]));
        Equal(3, outputApp7Keys.Count);
        Equal(2, outputApp8Keys.Count);
        Equal(2, dataApp7Keys.Count);
        NotEqual(outputApp7Keys[0], outputApp8Keys[0]);
        NotEqual(outputApp7Keys[0], dataApp7Keys[0]);
        NotEqual(outputApp7Keys[1], dataApp7Keys[1]);
    }

    [Fact]
    public void GetOrEnsureKeysWithoutDependenciesReturnsAppKeyOnly()
    {
        var keys = Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, null);

        Single(keys);
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
        var dependencyKeysA = Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, ["products"]);
        var dependencyKeysB = Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, ["pricing"]);

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(dependencyKeysA));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(dependencyKeysB));

        NotNull(Cache.Get<OutputCacheItem>(entryKeyA));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyB));

        Dependencies.Touch(CacheDependencyScopes.OutputCache, 7, ["products"]);

        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyB));
    }

    [Fact]
    public void TouchAppInvalidatesAllMatchingAppCacheEntriesOnly()
    {
        var entryKeyA = nameof(TouchAppInvalidatesAllMatchingAppCacheEntriesOnly) + "-a";
        var entryKeyB = nameof(TouchAppInvalidatesAllMatchingAppCacheEntriesOnly) + "-b";
        var entryKeyC = nameof(TouchAppInvalidatesAllMatchingAppCacheEntriesOnly) + "-c";

        Cache.Set(entryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, ["products"])));
        Cache.Set(entryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, ["pricing"])));
        Cache.Set(entryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 8, ["products"])));

        Dependencies.TouchApp(CacheDependencyScopes.OutputCache, 7);

        Null(Cache.Get<OutputCacheItem>(entryKeyA));
        Null(Cache.Get<OutputCacheItem>(entryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(entryKeyC));
    }

    [Fact]
    public void TouchUnknownKeyDoesNotInvalidateExistingEntries()
    {
        var entryKey = nameof(TouchUnknownKeyDoesNotInvalidateExistingEntries);
        var dependencyKeys = Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, 7, ["products"]);

        Cache.Set(entryKey, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(dependencyKeys));
        NotNull(Cache.Get<OutputCacheItem>(entryKey));

        Dependencies.Touch(CacheDependencyScopes.OutputCache, 7, ["unknown"]);

        NotNull(Cache.Get<OutputCacheItem>(entryKey));
    }
}

public class StartupSxcWithDbWithoutLightSpeed : StartupSxcWithDb
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.RemoveAll<IOutputCache>();
    }
}

internal class TestApp(int appId) : IApp
{
    public int AppId => appId;
    public int ZoneId => 0;
    public string Name => "Test App";
    public string Folder => "TestApp";
    public string NameId => "test-app";
    public string AppGuid => NameId;
    public IAppData Data => null!;
    public IAppConfiguration Configuration => null!;
    public dynamic? Settings => null;
    public dynamic? Resources => null;
    public IDictionary<string, IQuery> Query { get; } = new Dictionary<string, IQuery>();
    public IQuery GetQuery(string name) => throw new NotSupportedException();
    public string Path => "/test-app";
    public string PhysicalPath => @"c:\test-app";
    public string PathShared => "/test-app-shared";
    public string PhysicalPathShared => @"c:\test-app-shared";
    public string RelativePath => "test-app";
    public string RelativePathShared => "test-app-shared";
    public string? Thumbnail => null;
    public IMetadata Metadata => null!;
}
