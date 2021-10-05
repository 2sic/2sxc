using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperSuffixHashTests : LinkHelperTestBase
    {
        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(string.Empty, Link.TestTo(part: "suffix"));
        }

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
            AreEqual("?param=a#fragment", Link.Image(url: "", part: "suffix"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(string.Empty, Link.Image(url: "/", part: "suffix"));
            AreEqual("?a=1&b=2#fragment", Link.Image(url: "/?a=1&b=2#fragment", part: "suffix"));
            AreEqual("#fragment", Link.Image(url: "/page#fragment", part: "suffix"));
            AreEqual("?a=1&b=2#fragment", Link.Image(url: "/page?a=1&b=2#fragment", part: "suffix"));
            AreEqual(string.Empty, Link.Image(url: "~/", part: "suffix"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual("?param=a&c=3#fragment", Link.Image(url: "?c=3", part: "suffix"));
            AreEqual("?param=a#fragmentB", Link.Image(url: "?#fragmentB", part: "suffix"));
            AreEqual("?param=c#fragmentB", Link.Image(url: "?param=c#fragmentB", part: "suffix"));
            AreEqual("?param=a#fragmentB", Link.Image(url: "#fragmentB", part: "suffix"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual("#fragment", Link.Image(url: "//unknown.2sxc.org/test#fragment", part: "suffix"));
            AreEqual("?a=1#fragment", Link.Image(url: "//unknown.2sxc.org/test?a=1#fragment", part: "suffix"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual("#fragment", Link.Image(url: "http://unknown2.2sxc.org/#fragment", part: "suffix"));
            AreEqual("?a=1#fragmentB", Link.Image(url: "https://unknown2.2sxc.org/page?a=1#fragmentB", part: "suffix"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.Image(url: "hello:there", part: "suffix"));
            AreEqual("file:593902", Link.Image(url: "file:593902", part: "suffix"));

            AreEqual("../file.ext", Link.Image(url: "../file.ext", part: "suffix"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.Image(url: "/sibling1/../sibling2/image.jpg", part: "suffix"));
        }
    }
}