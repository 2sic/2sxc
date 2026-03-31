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
        var html = new StringBuilder(approxChars + 1024);
        var random = new Random(approxChars);
        html.AppendLine("<main class=\"page page--cache-test\" data-cache=\"lightspeed\">");

        var index = 1;
        while (html.Length < approxChars)
        {
            html.AppendLine(BuildFragment(index, random));
            index++;
        }

        html.AppendLine("</main>");
        return html.ToString();
    }

    private static string BuildFragment(int index, Random random)
    {
        var title = $"{Pick(Titles, random)} {Pick(Topics, random)}";
        var author = Pick(Authors, random);
        var city = Pick(Cities, random);
        var category = Pick(Categories, random);
        var blockKind = Pick(BlockKinds, random);
        var stateClass = index % 3 == 0 ? "is-featured" : "is-standard";
        var slug = ToSlug(title, index, random.Next(1000, 9999));
        var isoDate = new DateTime(2025, 1, 1).AddDays((index * 17) % 320).ToString("yyyy-MM-dd");
        var uniqueKey = BuildUniqueKey(index, random);
        var tags = BuildTags(index, random);
        var metrics = $"{{&quot;views&quot;:{random.Next(20, 9000)},&quot;score&quot;:{random.Next(10, 99)},&quot;v&quot;:&quot;{uniqueKey}&quot;}}";
        var imageWidth = 320 + index % 5 * 40;
        var imageHeight = 180 + index % 4 * 20;
        var imageQuality = 60 + index % 30;

        return (index % 4) switch
        {
            0 => $@"<article class=""card card--{blockKind} {stateClass}"" data-id=""{index}"" data-category=""{category}"" data-state=""{metrics}"">
  <header class=""card__header"">
    <h2 class=""card__title"">{title}</h2>
    <p class=""card__meta"">
      <span class=""author"">{author}</span>
      <span class=""city"">{city}</span>
      <time datetime=""{isoDate}"">{isoDate}</time>
    </p>
  </header>
  <div class=""card__body"">
    <p>{Pick(Sentences, random)} {Pick(Sentences, random)}</p>
    <ul class=""tag-list"">{tags}</ul>
    <a class=""btn btn--primary"" href=""/news/{slug}?cat={category}&amp;ref={random.Next(100, 999)}"">Read more</a>
  </div>
</article>",
            1 => $@"<section class=""teaser teaser--split teaser--{blockKind}"" data-ref=""{uniqueKey}"">
  <div class=""teaser__content"">
    <h3>{title}</h3>
    <p>{Pick(Sentences, random)}</p>
    <p>{Pick(Sentences, random)}</p>
  </div>
  <aside class=""teaser__aside"">
    <img src=""/img/{slug}.jpg?width={imageWidth}&amp;quality={imageQuality}"" width=""{imageWidth}"" height=""{imageHeight}"" alt=""{title}"">
  </aside>
</section>",
            2 => $@"<li class=""search-result search-result--{category}"" data-score=""{random.NextDouble():F3}"" data-key=""{uniqueKey}"">
  <a href=""/docs/{slug}"">
    <strong>{title}</strong>
    <span class=""breadcrumb"">/{category}/{city}/{index}</span>
    <span class=""excerpt"">{Pick(Sentences, random)} {Pick(Sentences, random)}</span>
  </a>
</li>",
            _ => $@"<div class=""content-block content-block--{blockKind}"" id=""block-{index}"" data-json=""{metrics}"">
  <h4>{title}</h4>
  <blockquote>{Pick(Quotes, random)}</blockquote>
  <pre class=""snippet"">var cfg = {{ ""id"": {index}, ""key"": ""{uniqueKey}"", ""slug"": ""{slug}"", ""city"": ""{city}"" }};</pre>
</div>",
        };
    }

    private static string BuildTags(int index, Random random)
    {
        var tagCount = 2 + index % 4;
        var tags = new StringBuilder(tagCount * 32);
        for (var tagIndex = 0; tagIndex < tagCount; tagIndex++)
            tags.Append($"<li class=\"tag tag--{tagIndex}\">{Pick(Tags, random)}</li>");

        return tags.ToString();
    }

    private static string Pick(IReadOnlyList<string> values, Random random)
        => values[random.Next(values.Count)];

    private static string BuildUniqueKey(int index, Random random)
      => $"{index:x4}{random.Next(0, 1 << 16):x4}{random.Next(0, 1 << 16):x4}";

    private static string ToSlug(string value, int index, int suffix)
      => string.Join("-", value.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) + $"-{index}-{suffix}";

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

    private static readonly string[] Titles =
    [
      "Latest",
      "Breaking",
      "Weekly",
      "Community",
      "Editorial",
      "Release",
      "Migration",
      "Preview",
    ];

    private static readonly string[] Topics =
    [
      "Caching",
      "Search",
      "Content",
      "Images",
      "Templates",
      "Security",
      "Routing",
      "Insights",
    ];

    private static readonly string[] Authors =
    [
      "2sxc Team",
      "Daniel Mettler",
      "Benjamin Freitag",
      "Roman Opalko",
      "Dominik Graf",
      "Raoul Bossi",
    ];

    private static readonly string[] Cities =
    [
      "Buchs",
      "Zurich",
      "Vienna",
      "Berlin",
      "Ljubljana",
      "Oslo",
      "Milan",
    ];

    private static readonly string[] Categories =
    [
      "guides",
      "blog",
      "docs",
      "news",
      "release-notes",
      "announcements",
    ];

    private static readonly string[] BlockKinds =
    [
      "teaser",
      "feature",
      "summary",
      "compact",
      "quote",
      "code",
    ];

    private static readonly string[] Tags =
    [
      "cache",
      "lightspeed",
      "html",
      "compression",
      "razor",
      "rendering",
      "cms",
      "performance",
    ];

    private static readonly string[] Sentences =
    [
      "This fragment simulates CMS output with varying metadata and nested markup.",
      "The benchmark should include repeated patterns, but not to the point of synthetic compression ratios.",
      "Rendered pages often contain unique slugs, ids, dates, image urls and query strings.",
      "Some blocks include short text while others contain denser paragraphs and attribute-heavy markup.",
      "Mixed page output usually combines cards, lists, snippets, images and editorial text.",
    ];

    private static readonly string[] Quotes =
    [
      "Cache correctness matters more than micro-bench optimism.",
      "Compression only helps if the memory trade is real.",
      "Synthetic markup should be representative, not flattering.",
    ];
}