using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass()]
    public class LinkHelperToApiPartFullTests: LinkHelperTestBase
    {
        private void ToApiPartFullVerifyUrlAreEqual(string testUrl)
        {
            AreEqual(testUrl, Link.To(api: testUrl, part: "full"));
        }

        [TestMethod()]
        public void ToNoUrlOrParamsTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/folder/subfolder/page?param=a#fragment", Link.To(api: "", part: "full"));
        }

        [TestMethod()]
        public void ToCommonUrlsTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/", Link.To(api: "/", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/?a=1&b=2#fragment", Link.To(api: "/", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/api", Link.To(api: "/api", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/api?a=1&b=2#fragment", Link.To(api: "/api", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/", Link.To(api: "/app/", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/?a=1&b=2#fragment", Link.To(api: "/app/", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/api", Link.To(api: "/app/api", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/api?a=1&b=2#fragment", Link.To(api: "/app/api", parameters: "a=1&b=2#fragment", part: "full"));
        }

        [TestMethod()]
        public void ToApiParametersTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/app/api", Link.To(api: "/app/api", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/api", Link.To(api: "/app/api", parameters: null, part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/api?a=1&b=2#fragment", Link.To(api: "/app/api", parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/api?a=1&b=2&c=3", Link.To(api: "/app/api", parameters: new Parameters(new NameValueCollection
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            }), part: "full"));
        }

        [TestMethod()]
        public void ToUrlPathIsMissingTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/folder/subfolder/page?param=b&b=3&c=3#fragment", Link.To(api: "", parameters: "param=b&b=3&c=3", part: "full"));
        }

        [TestMethod()]
        public void ToWithoutProtocolTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/api?param=b&b=3&c=3", Link.To(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3", part: "full"));
        }

        [TestMethod()]
        public void ToUrlWithTildeTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/api?p=1&r=2", Link.To(api: "~/api", parameters: "p=1&r=2", part: "full"));
            AreEqual($"{LinkHelperUnknown.MockHost}/app/", Link.To(api: "~/app/", part: "full"));
        }

        [TestMethod()]
        public void ToWithAbsoluteUrlTest()
        {
            AreEqual("https://unknown2.2sxc.org/", Link.To(api: "https://unknown2.2sxc.org/", part: "full"));
            AreEqual("https://unknown2.2sxc.org/api", Link.To(api: "https://unknown2.2sxc.org/api", part: "full"));
            AreEqual("https://unknown2.2sxc.org/app/api?a=1", Link.To(api: "https://unknown2.2sxc.org/app/api", parameters: "a=1", part: "full"));
        }

        [TestMethod()]
        public void ToWithInvalidUrlTest()
        {
            ToApiPartFullVerifyUrlAreEqual("hello2:there");
            ToApiPartFullVerifyUrlAreEqual("file:123");
            ToApiPartFullVerifyUrlAreEqual("../api");
            ToApiPartFullVerifyUrlAreEqual("/sibling1/../sibling2/api");
        }
    }
}