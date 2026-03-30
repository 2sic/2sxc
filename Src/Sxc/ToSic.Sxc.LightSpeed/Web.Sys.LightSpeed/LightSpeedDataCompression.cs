using ToSic.Sxc.Render.Sys;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

internal static class LightSpeedDataCompression
{
    internal static IRenderResult OptimizeForCache(IRenderResult data, bool useCompression, int minBytes = RenderResultHtmlCompression.DefaultMinBytes)
    {
        if (!useCompression || data is not RenderResult renderResult)
            return data;

        if (renderResult.Html is not { Length: > 0 } html)
            return data;

        // Use UTF-8 byte count because compression operates on the encoded payload, not the .NET string length.
        var originalHtmlUtf8Bytes = RenderResultHtmlCompression.GetUtf8ByteCount(html);
        if (originalHtmlUtf8Bytes < minBytes)
            return data;

        var compressedHtml = RenderResultHtmlCompression.Compress(html);
        if (compressedHtml.Length >= originalHtmlUtf8Bytes)
            return data;

        return renderResult with
        {
            Html = null,
            CompressedHtml = compressedHtml,
            OriginalHtmlUtf8Bytes = originalHtmlUtf8Bytes,
        };
    }
}
