using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToApiPartUndefinedTests: LinkHelperTestBase
    {
        private void ToApiPartUndefinedVerifyUrlAreEqual(string testUrl)
        {
            AreEqual(testUrl, Link.TestTo(api: testUrl));
        }

        [TestMethod]
        public void ToNoApiOrParamsTest()
        {
            ToApiPartUndefinedVerifyUrlAreEqual("");
        }

        [TestMethod]
        public void ToApiCommonUrlsTest()
        {
            AreEqual($"/", Link.TestTo(api: "/"));
            AreEqual($"/?a=1&b=2#fragment", Link.TestTo(api: "/", parameters: "a=1&b=2#fragment"));
            AreEqual($"/api", Link.TestTo(api: "/api"));
            AreEqual($"/api?a=1&b=2#fragment", Link.TestTo(api: "/api", parameters: "a=1&b=2#fragment"));
            AreEqual($"/app/", Link.TestTo(api: "/app/"));
            AreEqual($"/app/?a=1&b=2#fragment", Link.TestTo(api: "/app/", parameters: "a=1&b=2#fragment"));
            AreEqual($"/app/api", Link.TestTo(api: "/app/api"));
            AreEqual($"/app/api?a=1&b=2#fragment", Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment"));
        }

        [TestMethod]
        public void ToApiParametersTest()
        {
            AreEqual($"/app/api", Link.TestTo(api: "/app/api"));
            AreEqual($"/app/api", Link.TestTo(api: "/app/api", parameters: null));
            AreEqual($"/app/api?a=1&b=2#fragment", Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment"));
            AreEqual($"/app/api?a=1&b=2&c=3", Link.TestTo(api: "/app/api", parameters: NewParameters(new()
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            })));
        }

        [TestMethod]
        public void ToApiPathIsMissingTest()
        {
            AreEqual($"?param=b&b=3&c=3", Link.TestTo(api: "", parameters: "param=b&b=3&c=3"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest() // current behavior, potentially we can improve like in part "full"
        {
            AreEqual($"//unknown.2sxc.org/api?param=b&b=3&c=3", Link.TestTo(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3"));
        }

        [TestMethod]
        public void ToApiWithTildeTest() // current behavior, potentially we can improve like in part "full"
        {
            AreEqual($"~/api?p=1&r=2", Link.TestTo(api: "~/api", parameters: "p=1&r=2"));
            AreEqual($"~/app/", Link.TestTo(api: "~/app/"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            ToApiPartUndefinedVerifyUrlAreEqual("https://unknown2.2sxc.org/");
            ToApiPartUndefinedVerifyUrlAreEqual("https://unknown2.2sxc.org/api");
            AreEqual("https://unknown2.2sxc.org/app/api?a=1", Link.TestTo(api: "https://unknown2.2sxc.org/app/api", parameters: "a=1"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            ToApiPartUndefinedVerifyUrlAreEqual("hello2:there");
            ToApiPartUndefinedVerifyUrlAreEqual("file:123");
            ToApiPartUndefinedVerifyUrlAreEqual("../api");
            ToApiPartUndefinedVerifyUrlAreEqual("/sibling1/../sibling2/api");
        }
    }
}