using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Web.LinkHelperUnknown;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperSuffixHashTests : LinkHelperTestBase
    {

        [TestMethod]
        public void ToPageTest()
        {
            AreEqual(string.Empty, Link.TestTo(pageId: 27, part: "suffix"));
            AreEqual("?a=1&b=2", Link.TestTo(pageId: 27, parameters: "a=1&b=2", part: "suffix"));
            AreEqual("?a=1&b=2#fragment", Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment", part: "suffix"));
            AreEqual("#fragment", Link.TestTo(pageId: 27, parameters: "#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual("?param=a#fragment", Link.TestTo(api: "", part: "suffix"));
        }

        [TestMethod]
        public void ToApiTest()
        {
            AreEqual(string.Empty, Link.TestTo(api: "/", part: "suffix"));
            AreEqual("?a=1&b=2#fragment", Link.TestTo(api: "/", parameters: "a=1&b=2#fragment", part: "suffix"));
            AreEqual("#fragment", Link.TestTo(api: "~/api", parameters: "#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual("?param=b&b=3&c=3#fragment", Link.TestTo(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual(string.Empty, Link.TestTo(api: "http://unknown2.2sxc.org/", part: "suffix"));
            AreEqual("?a=1#fragment", Link.TestTo(api: "http://unknown2.2sxc.org/app/api", parameters: "a=1#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello3:there", Link.TestTo(api: "hello3:there", parameters: "#fragment", part: "suffix"));
            AreEqual("file:456", Link.TestTo(api: "file:456", parameters: "a=1#fragment", part: "suffix"));

            // Invalid URI: The format of the URI could not be determined.
            AreEqual("../file.ext", Link.TestTo(api: "../file.ext", parameters: "#fragment", part: "suffix"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestTo(api: "/sibling1/../sibling2/image.jpg", parameters: "b=2#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual($"?{CurrentQuery}", Link.TestImage(url: "", part: "suffix"));
            AreEqual($"?{CurrentQuery}#fragment", Link.TestImage(url: "#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(string.Empty, Link.TestImage(url: "/", part: "suffix"));
            AreEqual("?a=1&b=2#fragment", Link.TestImage(url: "/?a=1&b=2#fragment", part: "suffix"));
            AreEqual("#fragment", Link.TestImage(url: "/page#fragment", part: "suffix"));
            AreEqual("?a=1&b=2#fragment", Link.TestImage(url: "/page?a=1&b=2#fragment", part: "suffix"));
            AreEqual(string.Empty, Link.TestImage(url: "~/", part: "suffix"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual($"?{CurrentQuery}&c=3", Link.TestImage(url: "?c=3", part: "suffix"));
            AreEqual($"?{CurrentQuery}&c=3#fragment", Link.TestImage(url: "?c=3#fragment", part: "suffix"));
            AreEqual($"?{CurrentQuery}#fragmentB", Link.TestImage(url: "?#fragmentB", part: "suffix"));
            AreEqual("?param=c#fragmentB", Link.TestImage(url: "?param=c#fragmentB", part: "suffix"));
            AreEqual($"?{CurrentQuery}#fragmentB", Link.TestImage(url: "#fragmentB", part: "suffix"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual("#fragment", Link.TestImage(url: "//unknown.2sxc.org/test#fragment", part: "suffix"));
            AreEqual("?a=1#fragment", Link.TestImage(url: "//unknown.2sxc.org/test?a=1#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual("#fragment", Link.TestImage(url: "http://unknown2.2sxc.org/#fragment", part: "suffix"));
            AreEqual("?a=1#fragmentB", Link.TestImage(url: "https://unknown2.2sxc.org/page?a=1#fragmentB", part: "suffix"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.TestImage(url: "hello:there", part: "suffix"));
            AreEqual("file:593902", Link.TestImage(url: "file:593902", part: "suffix"));

            AreEqual("../file.ext", Link.TestImage(url: "../file.ext", part: "suffix"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestImage(url: "/sibling1/../sibling2/image.jpg", part: "suffix"));
        }
    }
}