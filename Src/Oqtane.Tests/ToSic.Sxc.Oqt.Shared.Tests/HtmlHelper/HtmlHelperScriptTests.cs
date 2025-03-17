using Oqtane.Models;
using Xunit.Abstractions;

namespace ToSic.Sxc.Oqtane.Shared.HtmlHelper;


public class HtmlHelperScriptTests(ITestOutputHelper output)
{
    /// <summary>
    ///  Add Script Test Accessor
    /// </summary>
    private static string AddScriptTac(string html, string src, Alias alias) =>
        Oqt.Shared.Helpers.HtmlHelper.ManageScripts(
            html,
            new()
            {
                SxcScripts = [src],
                TemplateResources = [],
            },
            alias
        );

    [Theory]
    [InlineData("Add Script", """<!-- just a comment --><script src="https://example.com/script.js"></script>""", "<!-- just a comment -->", "script.js")]
    public void TestVariousCombinationsOfAdd(string name, string expected, string html, string src)
    {
        // Arrange
        //var html = "<!-- just a comment -->";
        //var src = "script.js";
        var alias = new Alias { BaseUrl = "https://example.com/" };

        // Act
        var result = AddScriptTac(html, src, alias);

        // Assert
        output.WriteLine($"Expected: {expected}");
        output.WriteLine($"Result: {result}");
        //Assert.Equal(expected, result);
        Contains(expected, result);

    }

    //[Fact]
    //public void AddScript_WithValidHtmlAndSrc_AddsScript()
    //{
    //    // Arrange
    //    var html = "<!-- just a comment -->";
    //    var src = "script.js";
    //    var alias = new Alias { BaseUrl = "https://example.com/" };

    //    // Act
    //    var result = AddScriptTac(html, src, alias);

    //    // Assert
    //    Assert.Contains("""<script src="https://example.com/script.js"></script>""", result);
    //}

    [Fact]
    public void AddScript_WithExistingScript_DoesNotAddScript()
    {
        // Arrange
        var html = "<!-- just a comment --><script src=\"https://example.com/script.js\"></script><!-- just a comment -->";
        var src = "script.js";
        var alias = new Alias { BaseUrl = "https://example.com/" };

        // Act
        var result = AddScriptTac(html, src, alias);

        // Assert
        var scriptCount = result.Split(["""<script src="https://example.com/script.js"></script>"""], StringSplitOptions.None).Length - 1;
        Equal(1, scriptCount);
    }

    [Fact]
    public void AddScript_WithEmptyHtml_AddsScript()
    {
        // Arrange
        var html = string.Empty;
        var src = "script.js";
        var alias = new Alias { BaseUrl = "https://example.com/" };

        // Act
        var result = AddScriptTac(html, src, alias);

        // Assert
        Contains("""<script src="https://example.com/script.js"></script>""", result);
    }

    [Fact]
    public void AddScript_WithNullSrc_ReturnsOriginalHtml()
    {
        // Arrange
        var html = "<!-- just a comment -->";
        var alias = new Alias { BaseUrl = "https://example.com/" };

        // Act
#pragma warning disable CS8600
        string src = null;
#pragma warning disable CS8604
        var result = AddScriptTac(html, src, alias);
#pragma warning restore CS8604
#pragma warning restore CS8600

        // Assert
        Equal(html, result);
    }

    [Fact]
    public void AddScript_WithEmptySrc_ReturnsOriginalHtml()
    {
        // Arrange
        var html = "<!-- just a comment -->";
        var src = string.Empty;
        var alias = new Alias { BaseUrl = "https://example.com/" };

        // Act
        var result = AddScriptTac(html, src, alias);

        // Assert
        Equal(html, result);
    }

    [Fact]
    public void AddScript_WithFullUrlSrc_AddsScriptWithFullUrl()
    {
        // Arrange
        var html = "<!-- just a comment -->";
        var src = "https://cdn.example.com/script.js";
        var alias = new Alias { BaseUrl = "https://example.com/" };

        // Act
        var result = AddScriptTac(html, src, alias);

        // Assert
        Contains("""<script src="https://cdn.example.com/script.js"></script>""", result);
    }
}