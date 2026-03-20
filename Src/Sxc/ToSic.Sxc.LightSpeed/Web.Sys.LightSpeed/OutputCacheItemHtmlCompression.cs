using System.IO.Compression;
using System.Text;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

internal static class OutputCacheItemHtmlCompression
{
    public const int DefaultMinBytes = 5_000;

    public static int GetUtf8ByteCount(string html)
        => Encoding.UTF8.GetByteCount(html);

    public static byte[] Compress(string html)
    {
        var input = Encoding.UTF8.GetBytes(html);

        using var output = new MemoryStream(input.Length);
        using (var compressionStream = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
            compressionStream.Write(input, 0, input.Length);

        return output.ToArray();
    }

    public static string Decompress(byte[] compressedHtml)
    {
        using var input = new MemoryStream(compressedHtml);
        using var decompressionStream = new GZipStream(input, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(decompressionStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
        return reader.ReadToEnd();
    }
}