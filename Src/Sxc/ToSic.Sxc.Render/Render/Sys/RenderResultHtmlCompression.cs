using System.IO.Compression;
using System.Text;

namespace ToSic.Sxc.Render.Sys;

internal static class RenderResultHtmlCompression
{
    private static readonly UTF8Encoding Utf8NoBom = new(false);

    public const int DefaultMinBytes = 5_000;

    public static int GetUtf8ByteCount(string html)
        => Utf8NoBom.GetByteCount(html);

    public static byte[] Compress(string html)
    {
        using var output = new MemoryStream();
        using (var compressionStream = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
        using (var writer = new StreamWriter(compressionStream, Utf8NoBom, bufferSize: 1024, leaveOpen: true))
            writer.Write(html);

        return output.ToArray();
    }

    public static string Decompress(byte[] compressedHtml)
    {
        using var input = new MemoryStream(compressedHtml);
        using var decompressionStream = new GZipStream(input, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(decompressionStream, Utf8NoBom, detectEncodingFromByteOrderMarks: false);
        return reader.ReadToEnd();
    }
}
