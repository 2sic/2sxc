using ToSic.Sxc.Render.Sys;
using ToSic.Sys.Caching;
using ToSic.Sys.Memory;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class OutputCacheItem : ICanEstimateSize, ITimestamped
{
    private readonly IRenderResult data;
    private readonly byte[]? compressedHtml;
    private string? decompressedHtml;

    public OutputCacheItem(IRenderResult data)
    {
        this.data = data;
    }

    private OutputCacheItem(IRenderResult data, byte[] compressedHtml, int originalHtmlUtf8Bytes)
    {
        this.data = data;
        this.compressedHtml = compressedHtml;
        OriginalHtmlUtf8Bytes = originalHtmlUtf8Bytes;
    }

    public static OutputCacheItem Create(IRenderResult data, bool useCompression, int minBytes = OutputCacheItemHtmlCompression.DefaultMinBytes)
    {
        if (!useCompression)
            return new(data);

        if (data is not RenderResult renderResult || string.IsNullOrEmpty(renderResult.Html))
            return new(data);

        var originalHtmlUtf8Bytes = OutputCacheItemHtmlCompression.GetUtf8ByteCount(renderResult.Html);
        if (originalHtmlUtf8Bytes < minBytes)
            return new(data);

        var compressedHtml = OutputCacheItemHtmlCompression.Compress(renderResult.Html);
        if (compressedHtml.Length >= originalHtmlUtf8Bytes)
            return new(data);

        return new(renderResult with { Html = null }, compressedHtml, originalHtmlUtf8Bytes);
    }

    public IRenderResult Data => !IsCompressed || data is not RenderResult renderResult
        ? data
        : renderResult with { Html = decompressedHtml ??= OutputCacheItemHtmlCompression.Decompress(compressedHtml!) };

    public bool IsCompressed => compressedHtml != null;

    public int? OriginalHtmlUtf8Bytes { get; }

    public int? CompressedHtmlBytes => compressedHtml?.Length;

//#if NETFRAMEWORK
//    /// <summary>
//    /// This is only used in Dnn - might be solved with generics some time, but ATM this is just simpler
//    /// </summary>
//    public bool EnforcePre1025 = true;

//#endif
    public SizeEstimate EstimateSize(ILog? log = default)
                => !IsCompressed
                        ? (data as ICanEstimateSize)?.EstimateSize(log)
                            ?? new SizeEstimate(0, 0, Unknown: true)
                        : ((data as ICanEstimateSize)?.EstimateSize(log)
                             ?? new SizeEstimate(0, 0, Unknown: true))
                            + new SizeEstimate(compressedHtml!.Length, 32, true);

    /// <summary>
    /// Timestamp info to better analyze cache data
    /// </summary>
    long ITimestamped.CacheTimestamp { get; } = DateTime.Now.Ticks;
}