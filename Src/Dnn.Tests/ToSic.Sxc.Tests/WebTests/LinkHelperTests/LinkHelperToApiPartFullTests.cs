using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToApiPartFullTests: LinkHelperTestBase
    {
        private void ToApiPartFullVerifyUrlAreEqual(string testUrl)
        {
            AreEqual(testUrl, Link.TestTo(api: testUrl, part: "full"));
        }

        [TestMethod]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=a#fragment", Link.TestTo(api: "", part: "full"));
        }

        [TestMethod]
        public void ToApiCommonUrlsTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/", Link.TestTo(api: "/", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/?a=1&b=2#fragment", Link.TestTo(api: "/", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/api", Link.TestTo(api: "/api", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/api?a=1&b=2#fragment", Link.TestTo(api: "/api", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/", Link.TestTo(api: "/app/", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/?a=1&b=2#fragment", Link.TestTo(api: "/app/", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/api", Link.TestTo(api: "/app/api", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/api?a=1&b=2#fragment", Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment", part: "full"));
        }

        [TestMethod]
        public void ToApiParametersTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/api", Link.TestTo(api: "/app/api", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/api", Link.TestTo(api: "/app/api", parameters: null, part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/api?a=1&b=2#fragment", Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/api?a=1&b=2&c=3", Link.TestTo(api: "/app/api", parameters: new Parameters(new NameValueCollection
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            }), part: "full"));
        }

        [TestMethod]
        public void ToApiPathIsMissingTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=b&b=3&c=3#fragment", Link.TestTo(api: "", parameters: "param=b&b=3&c=3", part: "full"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/api?param=b&b=3&c=3", Link.TestTo(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3", part: "full"));
        }

        [TestMethod]
        public void ToApiWithTildeTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/api?p=1&r=2", Link.TestTo(api: "~/api", parameters: "p=1&r=2", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/app/", Link.TestTo(api: "~/app/", part: "full"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual("https://unknown2.2sxc.org/", Link.TestTo(api: "https://unknown2.2sxc.org/", part: "full"));
            AreEqual("https://unknown2.2sxc.org/api", Link.TestTo(api: "https://unknown2.2sxc.org/api", part: "full"));
            AreEqual("https://unknown2.2sxc.org/app/api?a=1", Link.TestTo(api: "https://unknown2.2sxc.org/app/api", parameters: "a=1", part: "full"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            ToApiPartFullVerifyUrlAreEqual("hello2:there");
            ToApiPartFullVerifyUrlAreEqual("file:123");
            ToApiPartFullVerifyUrlAreEqual("../api");
            ToApiPartFullVerifyUrlAreEqual("/sibling1/../sibling2/api");
        }
    }
}