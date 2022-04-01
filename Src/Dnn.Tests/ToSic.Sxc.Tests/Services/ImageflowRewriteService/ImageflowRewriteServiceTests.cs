using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Linq;

namespace ToSic.Sxc.Services.Tests
{
    [TestClass()]
    public class ImageflowRewriteServiceTests
    {
        public static readonly string JpgQualityDefault = ImageflowRewriteService.JpgQualityDefault;
        public static readonly string PngQualityDefault = ImageflowRewriteService.PngQualityDefault;
        public static readonly string WebpQualityDefault = ImageflowRewriteService.WebpQualityDefault;
        public static readonly string Quality = ImageflowRewriteService.Quality;
        public static readonly string JpgQuality = ImageflowRewriteService.JpgQuality;
        public static readonly string PngQuality = ImageflowRewriteService.PngQuality;
        public static readonly string WebpQuality = ImageflowRewriteService.WebpQuality;
        public static readonly string Format = ImageflowRewriteService.Format;

        private static NameValueCollection AddKeyWhenMissing(NameValueCollection queryString, string key, string value) =>
            ImageflowRewriteService.AddKeyWhenMissing(queryString, key, value);

        private static NameValueCollection AddQualityIfMissing(NameValueCollection queryString) =>
            ImageflowRewriteService.AddQualityIfMissing(queryString);

        private static NameValueCollection RewriteQuality(NameValueCollection queryString) =>
            ImageflowRewriteService.RewriteQuality(queryString);

        private NameValueCollection QueryStringRewrite(NameValueCollection queryString) => 
            new ImageflowRewriteService().QueryStringRewrite(queryString);

        
        [TestMethod()]
        public void AddKeyWhenMissingInQueryStringEmpty()
        {
            var actual = AddKeyWhenMissing(new NameValueCollection(), "key", "value");
            var expected = new NameValueCollection { { "key", "value" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddKeyWhenNotMissingInQueryStringOneElement()
        {
            var actual = AddKeyWhenMissing(
                new NameValueCollection { { "key", "value" } },
                "key", "value");
            var expected = new NameValueCollection { { "key", "value" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddKeyWhenMissingInQueryStringOneElement()
        {
            var actual = AddKeyWhenMissing(
                new NameValueCollection { { "1", "1" } },
                "key", "value");
            var expected = new NameValueCollection { { "1", "1" }, { "key", "value" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddKeyWhenNotMissingInQueryStringManyElement()
        {
            var actual = AddKeyWhenMissing(
                new NameValueCollection { { "1", "1" }, { "key", "value" } },
                "key", "value");
            var expected = new NameValueCollection { { "1", "1" }, { "key", "value" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddKeyWhenMissingInQueryStringManyElement()
        {
            var actual = AddKeyWhenMissing(
                new NameValueCollection { { "1", "1" }, { "2", "2" } },
                "key", "value");
            var expected = new NameValueCollection { { "1", "1" }, { "2", "2" } , { "key", "value" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddQualityWhenMissingInQueryStringEmpty()
        {
            var actual = AddQualityIfMissing(new NameValueCollection());
            var expected = new NameValueCollection { { Quality, JpgQualityDefault } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddQualityWhenNotMissingInQueryStringExactItem()
        {
            var actual = AddQualityIfMissing(new NameValueCollection{{ Quality, "99" }});
            var expected = new NameValueCollection { { Quality, "99" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddQualityWhenMissingInQueryStringManyItems()
        {
            var actual = AddQualityIfMissing(new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" }
            });
            var expected = new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
                { Quality, JpgQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddQualityWhenNotMissingInQueryStringManyItems()
        {
            var actual = AddQualityIfMissing(new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
                { Quality, "89" }
            });
            var expected = new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
                { Quality, "89" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddWebpQualityWhenMissingInQueryStringEmpty()
        {
            var actual = AddQualityIfMissing(new NameValueCollection{{ Format, "webp"}});
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, WebpQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddWebpQualityWhenNotMissingInQueryStringExactItem()
        {
            var actual = AddQualityIfMissing(new NameValueCollection
            {
                { Format, "webp" },
                { Quality, "99" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { Quality, "99" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddWebpQualityWhenMissingInQueryStringManyItems()
        {
            var actual = AddQualityIfMissing(new NameValueCollection
            {
                { Format, "webp" },
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, WebpQualityDefault },
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void AddWebpQualityWhenNotMissingInQueryStringManyItems()
        {
            var actual = AddQualityIfMissing(new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "59" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "59" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void RewriteQualityWhenQueryStringEmpty()
        {
            var actual = RewriteQuality(new NameValueCollection());
            var expected = new NameValueCollection ();
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void RewriteQualityWhenNotMissingInQueryStringExactItem()
        {
            var actual = RewriteQuality(new NameValueCollection { { Quality, "99" } });
            var expected = new NameValueCollection { { Quality, "99" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }


        [TestMethod()]
        public void RewriteWebpQualityWhenNotMissingInQueryStringExactItem()
        {
            var actual = RewriteQuality(new NameValueCollection
            {
                { Format, "webp" },
                { Quality, "99" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "99" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void RewriteWebpQualityWhenMissingInQueryStringManyItems()
        {
            var actual = RewriteQuality(new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "99" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "99" },
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void RewriteWebpQualityWhenNotMissingInQueryStringManyItems()
        {
            var actual = RewriteQuality(new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "59" },
                { Quality, "99" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "59" },
                { Quality, "99" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenQueryStringNull()
        {
            var actual = QueryStringRewrite(null);
            var expected = new NameValueCollection { { Quality, JpgQualityDefault } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }


        [TestMethod()]
        public void QueryStringRewriteWhenQueryStringEmpty()
        {
            var actual = QueryStringRewrite(new NameValueCollection());
            var expected = new NameValueCollection { { Quality, JpgQualityDefault } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenNotMissingInQueryStringOneElement()
        {
            var actual = QueryStringRewrite(new NameValueCollection { { Quality, "value" } });
            var expected = new NameValueCollection { { Quality, "value" } };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenMissingInQueryStringOneElement()
        {
            var actual = QueryStringRewrite(new NameValueCollection { { "1", "1" } });
            var expected = new NameValueCollection
            {
                { "1", "1" }, 
                { Quality, JpgQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenNotMissingInQueryStringManyElement()
        {
            var actual = QueryStringRewrite(
                new NameValueCollection
                {
                    { "1", "1" }, 
                    { Quality, JpgQualityDefault }
                });
            var expected = new NameValueCollection
            {
                { "1", "1" }, 
                { Quality, JpgQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenMissingInQueryStringManyElement()
        {
            var actual = QueryStringRewrite(
                new NameValueCollection
                {
                    { "1", "1" }, 
                    { "2", "2" }
                });
            var expected = new NameValueCollection
            {
                { "1", "1" }, 
                { "2", "2" },
                { Quality, JpgQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenMissingInQueryStringManyItems()
        {
            var actual = QueryStringRewrite(new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" }
            });
            var expected = new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
                { Quality, JpgQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWhenNotMissingInQueryStringManyItems()
        {
            var actual = QueryStringRewrite(new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
                { Quality, "89" }
            });
            var expected = new NameValueCollection
            {
                { JpgQuality, "79" },
                { PngQuality, "69" },
                { WebpQuality, "59" },
                { Quality, "89" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWebpWhenMissingInQueryStringEmpty()
        {
            var actual = QueryStringRewrite(new NameValueCollection { { Format, "webp" } });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, WebpQualityDefault }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWebpQuality()
        {
            var actual = QueryStringRewrite(new NameValueCollection
            {
                { Format, "webp" },
                { Quality, "99" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "99" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWebpQualityWhenNotMissingInQueryString()
        {
            var actual = QueryStringRewrite(new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "99" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { WebpQuality, "99" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }

        [TestMethod()]
        public void QueryStringRewriteWebpQualityWhenNotMissingInQueryString2()
        {
            var actual = QueryStringRewrite(new NameValueCollection
            {
                { Format, "webp" },
                { Quality, "99" },
                { WebpQuality, "59" }
            });
            var expected = new NameValueCollection
            {
                { Format, "webp" },
                { Quality, "99" },
                { WebpQuality, "59" }
            };
            CollectionAssert.AreEquivalent(
                expected.AllKeys.ToDictionary(k => k, k => expected[k]),
                actual.AllKeys.ToDictionary(k => k, k => actual[k]));
        }



    }
}