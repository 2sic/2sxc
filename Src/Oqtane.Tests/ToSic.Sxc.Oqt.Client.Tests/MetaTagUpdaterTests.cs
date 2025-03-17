using ToSic.Sxc.Oqt.Shared.Helpers;

namespace ToSic.Sxc.Oqt.Client.Tests;

[TestClass]
public class MetaTagUpdaterTests
{
    private const string MetaFruitDoubleQuote = """<meta name="keywords" content="orange, grape, strawberry">""";
    private const string MetaFruitSingleQuote = """<meta name="keywords" content='orange, grape, strawberry'>""";

    private static string AddOrUpdateMetaTagContent(string html, string name, string content) => HtmlHelper.AddOrUpdateMetaTagContent(html, name, content);

    [TestMethod]
    public void Test_UpdateExistingMetaTag()
    {
        var headContent = """<meta name="keywords" content="apple, banana, cherry">""";
        var result = AddOrUpdateMetaTagContent(headContent, "keywords", "orange, grape, strawberry");
        Assert.IsTrue(result.Contains(MetaFruitDoubleQuote));
    }

    [TestMethod]
    public void Test_AppendNewMetaTag()
    {
        var headContent = """<meta name="description" content="This is a description.">""";
        var result = AddOrUpdateMetaTagContent(headContent, "keywords", "orange, grape, strawberry");
        Assert.IsTrue(result.Contains(MetaFruitSingleQuote));
    }

    [TestMethod]
    public void Test_EmptyHeadContent_AppendNewMetaTag()
    {
        var headContent = @"";
        var result = AddOrUpdateMetaTagContent(headContent, "keywords", "orange, grape, strawberry");
        Assert.IsTrue(result.Contains(MetaFruitSingleQuote));
    }

    [TestMethod]
    public void Test_NullHeadContent_ReturnsOriginal()
    {
        var result = AddOrUpdateMetaTagContent(null!, "keywords", "orange, grape, strawberry");
        Assert.IsTrue(result.Contains(MetaFruitSingleQuote));
    }

    [TestMethod]
    public void Test_NullMetaTagName_ReturnsOriginal()
    {
        var headContent = """<meta name="keywords" content='apple, banana, cherry'>""";
        var result = AddOrUpdateMetaTagContent(headContent, null!, "orange, grape, strawberry");
        Assert.AreEqual(headContent, result);
    }

    [TestMethod]
    public void Test_EmptyMetaTagName_ReturnsOriginal()
    {
        var headContent = """<meta name="keywords" content="apple, banana, cherry">""";
        var result = AddOrUpdateMetaTagContent(headContent, "", "orange, grape, strawberry");
        Assert.AreEqual(headContent, result);
    }

    [TestMethod]
    public void Test_NullContentValue_UpdatesToEmpty()
    {
        var headContent = """<meta name="keywords" content="apple, banana, cherry">""";
#pragma warning disable CS8625
        var result = AddOrUpdateMetaTagContent(headContent, "keywords", null);
#pragma warning restore CS8625
        Assert.IsTrue(result.Contains("""<meta name="keywords" content="">"""));
    }

    [TestMethod]
    public void Test_EmptyContentValue_UpdatesToEmpty()
    {
        var headContent = """<meta name="keywords" content="apple, banana, cherry">""";
        var result = AddOrUpdateMetaTagContent(headContent, "keywords", "");
        Assert.IsTrue(result.Contains("""<meta name="keywords" content="">"""));
    }
}