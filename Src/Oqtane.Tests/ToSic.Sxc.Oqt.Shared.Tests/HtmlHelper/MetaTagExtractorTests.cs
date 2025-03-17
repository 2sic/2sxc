namespace ToSic.Sxc.Oqtane.Shared.HtmlHelper;

public class MetaTagExtractorTests
{
    private static string GetMetaTagContent(string html, string name) =>
        Oqt.Shared.Helpers.HtmlHelper.GetMetaTagContent(html, name);

    [Fact]
    public void Test_ValidMetaTag_ReturnsContentValue()
    {
        var html = """<meta name="keywords" content="apple, banana, cherry">""";
        var result = GetMetaTagContent(html, "keywords");
        Equal("apple, banana, cherry", result);
    }

    [Fact]
    public void Test_NoMetaTag_ReturnsNull()
    {
        var html = """<meta name="description" content="This is a description.">""";
        var result = GetMetaTagContent(html, "keywords");
        Null(result);
    }

    [Fact]
    public void Test_EmptyContentAttribute_ReturnsEmptyString()
    {
        var html = """<meta name="keywords" content="">""";
        var result = GetMetaTagContent(html, "keywords");
        Equal(string.Empty, result);
    }

    [Fact]
    public void Test_MetaTagNameCaseInsensitive_ReturnsContentValue()
    {
        var html = """<meta NAME="KEYWORDS" content="apple, banana, cherry">""";
        var result = GetMetaTagContent(html, "keywords");
        Equal("apple, banana, cherry", result);
    }

    [Fact]
    public void Test_ContentValueWithQuotes_ReturnsCorrectValue()
    {
        var html = """<meta name="keywords" content="apple, "banana", cherry">""";
        var result = GetMetaTagContent(html, "keywords");
        Equal("""apple, "banana", cherry""", result);
    }

    [Fact]
    public void Test_NullHtml_ReturnsNull()
    {
        var result = GetMetaTagContent(null!, "keywords");
        Null(result);
    }

    [Fact]
    public void Test_EmptyHtml_ReturnsNull()
    {
        var result = GetMetaTagContent(string.Empty, "keywords");
        Null(result);
    }

    [Fact]
    public void Test_NullMetaTagName_ReturnsNull()
    {
        var html = """<meta name="keywords" content="apple, banana, cherry">""";
        var result = GetMetaTagContent(html, null!);
        Null(result);
    }

    [Fact]
    public void Test_EmptyMetaTagName_ReturnsNull()
    {
        var html = """<meta name="keywords" content="apple, banana, cherry">""";
        var result = GetMetaTagContent(html, string.Empty);
        Null(result);
    }

    [Fact]
    public void Test_MetaTagWithSpaces_ReturnsContentValue()
    {
        var html = """<meta    name   =   "keywords"   content   =   "apple, banana, cherry"   >""";
        var result = GetMetaTagContent(html, "keywords");
        Equal("apple, banana, cherry", result);
    }
}