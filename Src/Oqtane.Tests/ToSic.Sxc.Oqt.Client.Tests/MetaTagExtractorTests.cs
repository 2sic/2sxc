using ToSic.Sxc.Oqt.Shared.Helpers;

namespace ToSic.Sxc.Oqt.Client.Tests
{

    [TestClass]
    public class MetaTagExtractorTests
    {
        private static string GetMetaTagContent(string html, string name) => HtmlHelper.GetMetaTagContent(html, name);

        [TestMethod]
        public void Test_ValidMetaTag_ReturnsContentValue()
        {
            var html = @"<meta name=""keywords"" content=""apple, banana, cherry"">";
            var result = GetMetaTagContent(html, "keywords");
            Assert.AreEqual("apple, banana, cherry", result);
        }

        [TestMethod]
        public void Test_NoMetaTag_ReturnsNull()
        {
            var html = @"<meta name=""description"" content=""This is a description."">";
            var result = GetMetaTagContent(html, "keywords");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Test_EmptyContentAttribute_ReturnsEmptyString()
        {
            var html = @"<meta name=""keywords"" content="""">";
            var result = GetMetaTagContent(html, "keywords");
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Test_MetaTagNameCaseInsensitive_ReturnsContentValue()
        {
            var html = @"<meta NAME=""KEYWORDS"" content=""apple, banana, cherry"">";
            var result = GetMetaTagContent(html, "keywords");
            Assert.AreEqual("apple, banana, cherry", result);
        }

        [TestMethod]
        public void Test_ContentValueWithQuotes_ReturnsCorrectValue()
        {
            var html = @"<meta name=""keywords"" content=""apple, ""banana"", cherry"">";
            var result = GetMetaTagContent(html, "keywords");
            Assert.AreEqual(@"apple, ""banana"", cherry", result);
        }

        [TestMethod]
        public void Test_NullHtml_ReturnsNull()
        {
#pragma warning disable CS8625
            var result = GetMetaTagContent(null, "keywords");
#pragma warning restore CS8625
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Test_EmptyHtml_ReturnsNull()
        {
            var result = GetMetaTagContent(string.Empty, "keywords");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Test_NullMetaTagName_ReturnsNull()
        {
            var html = @"<meta name=""keywords"" content=""apple, banana, cherry"">";
#pragma warning disable CS8625
            var result = GetMetaTagContent(html, null);
#pragma warning restore CS8625
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Test_EmptyMetaTagName_ReturnsNull()
        {
            var html = @"<meta name=""keywords"" content=""apple, banana, cherry"">";
            var result = GetMetaTagContent(html, string.Empty);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Test_MetaTagWithSpaces_ReturnsContentValue()
        {
            var html = @"<meta    name   =   ""keywords""   content   =   ""apple, banana, cherry""   >";
            var result = GetMetaTagContent(html, "keywords");
            Assert.AreEqual("apple, banana, cherry", result);
        }
    }
}

