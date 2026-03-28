using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcWithDbWithoutLightSpeed))]
public class OutputCacheManagementServiceWithoutLightSpeedTests(ExecutionContext exCtx)
{
    public class LightSpeedTestCase(ExecutionContext exCtx, string key)
    {
        public int CurrentAppId = 7;

        private INamedCacheDependencyService Dependencies =>
            exCtx.GetService<INamedCacheDependencyService>(reuse: true);

        private MemoryCacheService Cache =>
            exCtx.GetService<MemoryCacheService>(reuse: true);

        public int OtherAppId =>
            CurrentAppId + 1;

        public string EntryKeyA => key + "-a";
        public string EntryKeyB => key + "-b";
        public string EntryKeyC => key + "-c";

        public LightSpeedTestCase Setup()
        {
            Cache.Set(EntryKeyA, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(GetDependencies(CurrentAppId, ["products"])));
            Cache.Set(EntryKeyB, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(GetDependencies(CurrentAppId, ["pricing"])));
            Cache.Set(EntryKeyC, new OutputCacheItem(new RenderResult()), policy => policy.WatchCacheKeys(GetDependencies(OtherAppId, ["products"])));

            // Verify everything is as it should be
            NotNull(Cache.Get<OutputCacheItem>(EntryKeyA));
            NotNull(Cache.Get<OutputCacheItem>(EntryKeyB));
            NotNull(Cache.Get<OutputCacheItem>(EntryKeyC));
            return this;
        }

        private IEnumerable<string> GetDependencies(int appId, IEnumerable<string> keys) =>
            Dependencies.GetOrEnsureKeys(CacheDependencyScopes.OutputCache, appId, keys);
    }

    private IOutputCacheManagementService OutputCacheManagement =>
        exCtx.GetService<IOutputCacheManagementService>(reuse: true);

    private MemoryCacheService Cache =>
        exCtx.GetService<MemoryCacheService>(reuse: true);


    [Fact]
    public void OutputCacheManagementServiceResolvesWithoutLightSpeed()
        => NotNull(OutputCacheManagement);



    [Theory]
    [InlineData(1, "basic case, run 1x")]
    [InlineData(2, "repeat case, verify second flush works")]
    public void FlushSelectiveDependenciesWithoutLightSpeed(int runCount, string comment)
    {
        var testCase = new LightSpeedTestCase(exCtx, nameof(FlushSelectiveDependenciesWithoutLightSpeed));

        var touched = 0;
        for (var i = 0; i < runCount; i++)
        {
            testCase.Setup();
            touched = OutputCacheManagement.Flush(testCase.CurrentAppId, dependencies: [" products ", "PRODUCTS"]);
        }

        Equal(1, touched);
        Null(Cache.Get<OutputCacheItem>(testCase.EntryKeyA));
        NotNull(Cache.Get<OutputCacheItem>(testCase.EntryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(testCase.EntryKeyC));
    }

    [Theory]
    [InlineData(1, true, "basic case, run 1x")]
    [InlineData(1, false, "basic case, run 1x")]
    [InlineData(2, true, "repeat case, verify second flush works")]
    public void FlushNullTouchesAppWideMarkerForCurrentApp(int runCount, bool nullDependencies, string comment)
    {
        var testCase = new LightSpeedTestCase(exCtx, nameof(FlushNullTouchesAppWideMarkerForCurrentApp) + nullDependencies);

        var touched = 0;
        for (var i = 0; i < runCount; i++)
        {
            testCase.Setup();
            touched = nullDependencies
                ? OutputCacheManagement.Flush(testCase.CurrentAppId)
                : OutputCacheManagement.Flush(testCase.CurrentAppId, dependencies: []);
        }

        Equal(0, touched);
        Null(Cache.Get<OutputCacheItem>(testCase.EntryKeyA));
        Null(Cache.Get<OutputCacheItem>(testCase.EntryKeyB));
        NotNull(Cache.Get<OutputCacheItem>(testCase.EntryKeyC));
    }

    //[Fact]
    //public void FlushEmptyTouchesAppWideMarkerForCurrentApp()
    //{
    //    var testCase = new LightSpeedTestCase(exCtx, nameof(FlushEmptyTouchesAppWideMarkerForCurrentApp))
    //        .Setup();

    //    var touched = OutputCacheManagement.Flush(testCase.CurrentAppId, dependencies: []);

    //    Equal(0, touched);
    //    Null(Cache.Get<OutputCacheItem>(testCase.EntryKeyA));
    //    Null(Cache.Get<OutputCacheItem>(testCase.EntryKeyB));
    //    NotNull(Cache.Get<OutputCacheItem>(testCase.EntryKeyC));
    //}
}
