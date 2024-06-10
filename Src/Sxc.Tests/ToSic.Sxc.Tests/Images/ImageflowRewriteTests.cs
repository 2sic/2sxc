using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Linq;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Tests.Images
{
    [TestClass()]
    public class ImageflowRewriteTests
    {
        #region shared

        public static readonly string Quality = ImageflowRewrite.Quality;
        public static readonly string PngQuality = ImageflowRewrite.PngQuality;
        public static readonly string WebpQuality = ImageflowRewrite.WebpQuality;

        private static NameValueCollection AddKeyWhenMissing(NameValueCollection queryString, string key, string value) =>
            ImageflowRewrite.AddKeyWhenMissing(queryString, key, value);

        private NameValueCollection QueryStringRewrite(NameValueCollection queryString) => 
            ImageflowRewrite.QueryStringRewrite(queryString);

        private static void AreEquivalentAlsoByValues(NameValueCollection expected, NameValueCollection actual) =>
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));

        #endregion


        #region QueryStringRewrite MAIN TESTING

        [TestMethod()]
        public void QueryStringRewriteWhenQueryStringNull()
        {
            Assert.IsNull(QueryStringRewrite(null));
        }


        [TestMethod()]
        public void QueryStringRewriteWhenQueryStringEmpty()
        {
            var actual = QueryStringRewrite(new());
            var expected = new NameValueCollection ();
            AreEquivalentAlsoByValues(expected, actual);
        }


        [TestMethod()]
        public void QueryStringRewriteWhenQueryStringWithOneItem()
        {
            var actual = QueryStringRewrite(new() { { "1", "1" } });
            var expected = new NameValueCollection
            {
                { "1", "1" }
            };
            AreEquivalentAlsoByValues(expected, actual);
        }


        [TestMethod()]
        public void QueryStringRewriteWhenQueryStringWithManyItems()
        {
            var actual = QueryStringRewrite(
                new()
                {
                    { "1", "1" },
                    { "2", "2" }
                });
            var expected = new NameValueCollection
            {
                { "1", "1" },
                { "2", "2" },
            };
            AreEquivalentAlsoByValues(expected, actual);
        }


        [TestMethod()]
        public void QualityQueryStringRewrite()
        {
            var actual = QueryStringRewrite(new() { { Quality, "value" } });
            var expected = new NameValueCollection
            {
                { Quality, "value" },
                { PngQuality, "value" },
                { WebpQuality, "value" },

            };
            AreEquivalentAlsoByValues(expected, actual);
        }


        [TestMethod()]
        public void QualityQueryStringRewriteWhenManyItems()
        {
            var actual = QueryStringRewrite(
                new()
                {
                    { "1", "1" },
                    { "2", "2" },
                    { Quality, "value" }
                });
            var expected = new NameValueCollection
            {
                { "1", "1" },
                { "2", "2" },
                { Quality, "value" },
                { PngQuality, "value" },
                { WebpQuality, "value" },
            };
            AreEquivalentAlsoByValues(expected, actual);
        }


        [TestMethod()]
        public void QualityQueryStringRewriteWhenQueryStringWithManyQualityItems()
        {
            var actual = QueryStringRewrite(new()
            {
                { Quality, "value" },
                { PngQuality, "69" },
                { WebpQuality, "59" }
            });
            var expected = new NameValueCollection
            {
                { Quality, "value" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
            };
            AreEquivalentAlsoByValues(expected, actual);
        }


        #endregion


        #region AddKeyWhenMissing function testing

        [TestMethod()]
        public void AddKeyWhenMissingInQueryStringEmpty()
        {
            var actual = AddKeyWhenMissing(new(), "key", "value");
            var expected = new NameValueCollection { { "key", "value" } };
            AreEquivalentAlsoByValues(expected, actual);
        }

        [TestMethod()]
        public void AddKeyWhenNotMissingInQueryStringOneElement()
        {
            var actual = AddKeyWhenMissing(
                new() { { "key", "value" } },
                "key", "value");
            var expected = new NameValueCollection { { "key", "value" } };
            AreEquivalentAlsoByValues(expected, actual);
        }

        [TestMethod()]
        public void AddKeyWhenMissingInQueryStringOneElement()
        {
            var actual = AddKeyWhenMissing(
                new() { { "1", "1" } },
                "key", "value");
            var expected = new NameValueCollection { { "1", "1" }, { "key", "value" } };
            AreEquivalentAlsoByValues(expected, actual);
        }

        [TestMethod()]
        public void AddKeyWhenNotMissingInQueryStringManyElement()
        {
            var actual = AddKeyWhenMissing(
                new() { { "1", "1" }, { "key", "value" } },
                "key", "value");
            var expected = new NameValueCollection { { "1", "1" }, { "key", "value" } };
            AreEquivalentAlsoByValues(expected, actual);
        }

        [TestMethod()]
        public void AddKeyWhenMissingInQueryStringManyElement()
        {
            var actual = AddKeyWhenMissing(
                new() { { "1", "1" }, { "2", "2" } },
                "key", "value");
            var expected = new NameValueCollection { { "1", "1" }, { "2", "2" } , { "key", "value" } };
            AreEquivalentAlsoByValues(expected, actual);
        }

        #endregion

    }
}