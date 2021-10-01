using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToApiPartUndefinedTests: LinkHelperTestBase
    {
        private void ToApiPartUndefinedVerifyUrlAreEqual(string testUrl)
        {
            AreEqual(testUrl, Link.To(api: testUrl));
        }

        [TestMethod]
        public void ToNoApiOrParamsTest()
        {
            ToApiPartUndefinedVerifyUrlAreEqual("");
        }

        [TestMethod]
        public void ToApiCommonUrlsTest()
        {
            AreEqual($"/", Link.To(api: "/"));
            AreEqual($"/?a=1&b=2#fragment", Link.To(api: "/", parameters: "a=1&b=2#fragment"));
            AreEqual($"/api", Link.To(api: "/api"));
            AreEqual($"/api?a=1&b=2#fragment", Link.To(api: "/api", parameters: "a=1&b=2#fragment"));
            AreEqual($"/app/", Link.To(api: "/app/"));
            AreEqual($"/app/?a=1&b=2#fragment", Link.To(api: "/app/", parameters: "a=1&b=2#fragment"));
            AreEqual($"/app/api", Link.To(api: "/app/api"));
            AreEqual($"/app/api?a=1&b=2#fragment", Link.To(api: "/app/api", parameters: "a=1&b=2#fragment"));
        }

        [TestMethod]
        public void ToApiParametersTest()
        {
            AreEqual($"/app/api", Link.To(api: "/app/api"));
            AreEqual($"/app/api", Link.To(api: "/app/api", parameters: null));
            AreEqual($"/app/api?a=1&b=2#fragment", Link.To(api: "/app/api", parameters: "a=1&b=2#fragment"));
            AreEqual($"/app/api?a=1&b=2&c=3", Link.To(api: "/app/api", parameters: new Parameters(new NameValueCollection
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            })));
        }

        [TestMethod]
        public void ToApiPathIsMissingTest()
        {
            AreEqual($"?param=b&b=3&c=3", Link.To(api: "", parameters: "param=b&b=3&c=3"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest() // current behavior, potentially we can improve like in part "full"
        {
            AreEqual($"//unknown.2sxc.org/api?param=b&b=3&c=3", Link.To(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3"));
        }

        [TestMethod]
        public void ToApiWithTildeTest() // current behavior, potentially we can improve like in part "full"
        {
            AreEqual($"~/api?p=1&r=2", Link.To(api: "~/api", parameters: "p=1&r=2"));
            AreEqual($"~/app/", Link.To(api: "~/app/"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            ToApiPartUndefinedVerifyUrlAreEqual("https://unknown2.2sxc.org/");
            ToApiPartUndefinedVerifyUrlAreEqual("https://unknown2.2sxc.org/api");
            AreEqual("https://unknown2.2sxc.org/app/api?a=1", Link.To(api: "https://unknown2.2sxc.org/app/api", parameters: "a=1"));
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