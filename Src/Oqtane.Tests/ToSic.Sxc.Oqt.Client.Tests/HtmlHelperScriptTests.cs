using Oqtane.Models;
using ToSic.Sxc.Oqt.Shared.Helpers;

namespace ToSic.Sxc.Oqt.Client.Tests
{
    [TestClass]
    public class HtmlHelperScriptTests
    {
        private static string AddScript(string html, string src, Alias alias) => HtmlHelper.ManageScripts(html, new() { SxcScripts = [ src ] }, alias);

        [TestMethod]
        public void AddScript_WithValidHtmlAndSrc_AddsScript()
        {
            // Arrange
            var html = "<!-- just a comment -->";
            var src = "script.js";
            var alias = new Alias { BaseUrl = "https://example.com/" };

            // Act
            var result = AddScript(html, src, alias);

            // Assert
            Assert.IsTrue(result.Contains("<script src=\"https://example.com/script.js\"></script>"));
        }

        [TestMethod]
        public void AddScript_WithExistingScript_DoesNotAddScript()
        {
            // Arrange
            var html = "<!-- just a comment --><script src=\"https://example.com/script.js\"></script><!-- just a comment -->";
            var src = "script.js";
            var alias = new Alias { BaseUrl = "https://example.com/" };

            // Act
            var result = AddScript(html, src, alias);

            // Assert
            var scriptCount = result.Split(new[] { "<script src=\"https://example.com/script.js\"></script>" }, StringSplitOptions.None).Length - 1;
            Assert.AreEqual(1, scriptCount);
        }

        [TestMethod]
        public void AddScript_WithEmptyHtml_AddsScript()
        {
            // Arrange
            var html = string.Empty;
            var src = "script.js";
            var alias = new Alias { BaseUrl = "https://example.com/" };

            // Act
            var result = AddScript(html, src, alias);

            // Assert
            Assert.IsTrue(result.Contains("<script src=\"https://example.com/script.js\"></script>"));
        }

        [TestMethod]
        public void AddScript_WithNullSrc_ReturnsOriginalHtml()
        {
            // Arrange
            var html = "<!-- just a comment -->";
            var alias = new Alias { BaseUrl = "https://example.com/" };

            // Act
#pragma warning disable CS8600
            string src = null;
#pragma warning disable CS8604
            var result = AddScript(html, src, alias);
#pragma warning restore CS8604
#pragma warning restore CS8600

            // Assert
            Assert.AreEqual(html, result);
        }

        [TestMethod]
        public void AddScript_WithEmptySrc_ReturnsOriginalHtml()
        {
            // Arrange
            var html = "<!-- just a comment -->";
            var src = string.Empty;
            var alias = new Alias { BaseUrl = "https://example.com/" };

            // Act
            var result = AddScript(html, src, alias);

            // Assert
            Assert.AreEqual(html, result);
        }

        [TestMethod]
        public void AddScript_WithFullUrlSrc_AddsScriptWithFullUrl()
        {
            // Arrange
            var html = "<!-- just a comment -->";
            var src = "https://cdn.example.com/script.js";
            var alias = new Alias { BaseUrl = "https://example.com/" };

            // Act
            var result = AddScript(html, src, alias);

            // Assert
            Assert.IsTrue(result.Contains("<script src=\"https://cdn.example.com/script.js\"></script>"));
        }
    }
}
