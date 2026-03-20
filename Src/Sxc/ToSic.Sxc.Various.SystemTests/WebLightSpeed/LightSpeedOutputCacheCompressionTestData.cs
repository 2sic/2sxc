using System.IO.Compression;
using System.Text;

namespace ToSic.Sxc.WebLightSpeed;

public enum TestCompressionProfile
{
    GZipFastest,
    GZipOptimal,
}

public record HtmlCompressionCase(string Name, int ApproxChars)
{
    public override string ToString() => Name;
}

public static class LightSpeedOutputCacheCompressionTestData
{
    public static TheoryData<HtmlCompressionCase> HtmlSizes { get; } =
    [
        new("Approx 100 B", 100),
        new("Approx 1 KB", 1_000),
        new("Approx 5 KB", 5_000),
        new("Approx 10 KB", 10_000),
    ];

    public static TheoryData<HtmlCompressionCase, TestCompressionProfile> HtmlSizesAndProfiles { get; } = BuildSizesAndProfiles();

    private static TheoryData<HtmlCompressionCase, TestCompressionProfile> BuildSizesAndProfiles()
    {
        TheoryData<HtmlCompressionCase, TestCompressionProfile> data = [];
        foreach (var htmlSize in HtmlSizes)
        {
            data.Add(htmlSize, TestCompressionProfile.GZipFastest);
            data.Add(htmlSize, TestCompressionProfile.GZipOptimal);
        }

        return data;
    }

    public static string CreateRealisticHtml(int approxChars)
    {
        var html = new StringBuilder(approxChars + 512);
        html.AppendLine("<section class=\"ls-grid\" data-cache=\"lightspeed\">");

        var index = 1;
        while (html.Length < approxChars)
        {
            html.AppendLine($"""
                <article class="card card--teaser" data-id="{index}" data-type="fragment">
                  <header class="card__header">
                    <h2 class="card__title">Latest News {index}</h2>
                    <p class="card__meta">
                      <span class="author">2sxc Team</span>
                      <time datetime="2026-03-19">2026-03-19</time>
                    </p>
                  </header>
                  <div class="card__body">
                    <p>
                      Lightspeed cache should store realistic HTML fragments with repeated
                      classes, nested tags, attributes and content blocks that behave similarly
                      to rendered Razor output in production.
                    </p>
                    <ul class="tag-list">
                      <li class="tag">cache</li>
                      <li class="tag">lightspeed</li>
                      <li class="tag">html</li>
                      <li class="tag">compression</li>
                    </ul>
                    <a class="btn btn--primary" href="/news/{index}?utm_source=cache-test&amp;utm_medium=system-test">
                      Read more
                    </a>
                  </div>
                </article>
                """);
            index++;
        }

        html.AppendLine("</section>");
        return html.ToString();
    }

    public static byte[] Compress(string html, TestCompressionProfile profile)
    {
        var input = Encoding.UTF8.GetBytes(html);

        using var output = new MemoryStream(input.Length);
        using (var compressionStream = new GZipStream(output, ToCompressionLevel(profile), leaveOpen: true))
            compressionStream.Write(input, 0, input.Length);

        return output.ToArray();
    }

    public static string Decompress(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var decompressionStream = new GZipStream(input, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(decompressionStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
        return reader.ReadToEnd();
    }

    public static int GetUtf8ByteCount(string html)
        => Encoding.UTF8.GetByteCount(html);

    private static CompressionLevel ToCompressionLevel(TestCompressionProfile profile)
        => profile switch
        {
            TestCompressionProfile.GZipFastest => CompressionLevel.Fastest,
            TestCompressionProfile.GZipOptimal => CompressionLevel.Optimal,
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null),
        };
}