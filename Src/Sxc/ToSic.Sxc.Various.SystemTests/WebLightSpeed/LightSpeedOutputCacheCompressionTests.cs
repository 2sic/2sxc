using System.Diagnostics;
using System.Text;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Memory;
using Xunit.Abstractions;

namespace ToSic.Sxc.WebLightSpeed;

public class LightSpeedOutputCacheCompressionTests(ITestOutputHelper output)
{
    [Theory(Skip = "Disabled benchmark/report test.")]
    [MemberData(nameof(LightSpeedOutputCacheCompressionTestData.HtmlSizesAndProfiles), MemberType = typeof(LightSpeedOutputCacheCompressionTestData))]
    public void GZipProfiles_RoundTrip_And_Report(HtmlCompressionCase testCase, TestCompressionProfile profile)
    {
        var html = LightSpeedOutputCacheCompressionTestData.CreateRealisticHtml(testCase.ApproxChars);
        var compressed = LightSpeedOutputCacheCompressionTestData.Compress(html, profile);
        var decompressed = LightSpeedOutputCacheCompressionTestData.Decompress(compressed);

        Equal(html, decompressed);

        var utf8Bytes = LightSpeedOutputCacheCompressionTestData.GetUtf8ByteCount(html);
        var estimatedStringPayloadBytes = html.Length * sizeof(char);
        var compressDuration = Measure(() => LightSpeedOutputCacheCompressionTestData.Compress(html, profile), iterations: 100);
        var decompressDuration = Measure(() => LightSpeedOutputCacheCompressionTestData.Decompress(compressed), iterations: 100);

        output.WriteLine($"{testCase.Name} | {profile} | chars: {html.Length} | utf8: {utf8Bytes} B | string payload: {estimatedStringPayloadBytes} B | compressed: {compressed.Length} B | compress x100: {compressDuration.TotalMilliseconds:F2} ms | decompress x100: {decompressDuration.TotalMilliseconds:F2} ms");
    }

    [Theory(Skip = "Disabled benchmark/report test.")]
    [MemberData(nameof(LightSpeedOutputCacheCompressionTestData.HtmlSizes), MemberType = typeof(LightSpeedOutputCacheCompressionTestData))]
    public void Lifecycle_Report_For_CacheLike_Reads(HtmlCompressionCase testCase)
    {
        var html = LightSpeedOutputCacheCompressionTestData.CreateRealisticHtml(testCase.ApproxChars);

        foreach (var reads in new[] { 1, 10, 50 })
        {
            var compressedDuration = Measure(() =>
            {
                var compressed = LightSpeedOutputCacheCompressionTestData.Compress(html, TestCompressionProfile.GZipFastest);
                var lastLength = 0;
                for (var index = 0; index < reads; index++)
                    lastLength = LightSpeedOutputCacheCompressionTestData.Decompress(compressed).Length;
                return lastLength;
            }, iterations: 50);

            var rawDuration = Measure(() =>
            {
                var lastLength = 0;
                for (var index = 0; index < reads; index++)
                    lastLength = html.Length;
                return lastLength;
            }, iterations: 50);

            output.WriteLine($"{testCase.Name} | 1 write + {reads} reads | compressed x50: {compressedDuration.TotalMilliseconds:F2} ms | raw x50: {rawDuration.TotalMilliseconds:F2} ms");
        }
    }

    [Fact]
    public void OutputCacheItem_DoesNotCompress_WhenDisabled()
    {
        var html = LightSpeedOutputCacheCompressionTestData.CreateRealisticHtml(10_000);
        var result = CreateRenderResult(html);

        var cacheItem = new OutputCacheItem(LightSpeedDataCompression.OptimizeForCache(result, useCompression: false, minBytes: 5_000));

        False(IsCompressed(cacheItem.Data));
        Same(result, cacheItem.Data);
    }

    [Fact]
    public void OutputCacheItem_DoesNotCompress_WhenBelowThreshold()
    {
        var html = LightSpeedOutputCacheCompressionTestData.CreateRealisticHtml(1_000);
        var result = CreateRenderResult(html);

        var cacheItem = new OutputCacheItem(LightSpeedDataCompression.OptimizeForCache(result, useCompression: true, minBytes: 5_000));

        False(IsCompressed(cacheItem.Data));
        Same(result, cacheItem.Data);
    }

    [Fact]
    public void OutputCacheItem_DoesNotCompress_WhenCompressionDoesNotSaveSpace()
    {
        const string html = "<p class=\"teaser\">Hi</p>";
        var result = CreateRenderResult(html);

        var cacheItem = new OutputCacheItem(LightSpeedDataCompression.OptimizeForCache(result, useCompression: true, minBytes: 1));

        False(IsCompressed(cacheItem.Data));
        Same(result, cacheItem.Data);
    }

    [Fact]
    public void OutputCacheItem_Compresses_And_RoundTrips_WhenEnabled_AndUseful()
    {
        var html = LightSpeedOutputCacheCompressionTestData.CreateRealisticHtml(10_000);
        var result = CreateRenderResult(html) with
        {
            AppId = 27,
            ModuleId = 99,
            IsPartial = false,
            Errors = ["sample-error"],
        };
        result.DependentApps.Add(new TestDependentApp(7));

        var cacheItem = new OutputCacheItem(LightSpeedDataCompression.OptimizeForCache(result, useCompression: true, minBytes: 5_000));
        var compressedResult = (RenderResult)cacheItem.Data;

        True(IsCompressed(cacheItem.Data));
        var compressedBytes = result.CompressedHtml?.Length;
        NotNull(compressedBytes);
        NotNull(compressedResult.CompressedTrueSize);
        True(compressedBytes < compressedResult.CompressedTrueSize);

        var cachedResult = cacheItem.Data;
        Equal(html, cachedResult.Html);
        Equal(result.AppId, cachedResult.AppId);
        Equal(result.ModuleId, cachedResult.ModuleId);
        Equal(result.CanCache, cachedResult.CanCache);
        Equal(result.IsPartial, cachedResult.IsPartial);
        Equal(result.Errors, cachedResult.Errors);
        Single(cachedResult.DependentApps!);
        NotSame(result, cachedResult);
    }

    [Fact]
    public void RenderResult_ReDecompressesHtml_OnEachRead_WhenCompressed()
    {
        var html = LightSpeedOutputCacheCompressionTestData.CreateRealisticHtml(10_000);
        var result = CreateRenderResult(html);

        var cacheItem = new OutputCacheItem(LightSpeedDataCompression.OptimizeForCache(result, useCompression: true, minBytes: 5_000));

        True(IsCompressed(cacheItem.Data));

        var firstRead = cacheItem.Data;
        var secondRead = cacheItem.Data;

        Equal(html, firstRead.Html);
        Equal(html, secondRead.Html);
        Same(firstRead, secondRead);
        False(ReferenceEquals(firstRead.Html, secondRead.Html));
    }

    private static RenderResult CreateRenderResult(string html) => new()
    {
        Html = html,
        CanCache = true,
        AppId = 3,
        ModuleId = 4,
        IsPartial = false,
    };

    private static bool IsCompressed(IRenderResult result)
        => (result as IOptimizeMemory)?.UseCompression == true;

    private static TimeSpan Measure<T>(Func<T> action, int iterations)
    {
        action();

        var stopwatch = Stopwatch.StartNew();
        for (var index = 0; index < iterations; index++)
            _ = action();
        stopwatch.Stop();

        return stopwatch.Elapsed;
    }

    private class TestDependentApp(int appId) : IDependentApp
    {
        public int AppId { get; } = appId;
        public bool IsSitePrimaryApp { get; } = false;
        public bool IsEnabled { get; } = true;
        public List<string> CacheKeys { get; } = [$"app-{appId}"];
        public ICollection<string> PathsToMonitor { get; } = [];
    }
}
