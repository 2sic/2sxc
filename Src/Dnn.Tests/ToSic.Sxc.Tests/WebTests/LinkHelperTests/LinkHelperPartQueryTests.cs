using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperQueryHashTests : LinkHelperTestBase
    {
        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(string.Empty, Link.TestTo(part: "query"));
        }

        [TestMethod]
        public void ToPageTest()
        {
            AreEqual(string.Empty, Link.TestTo(pageId: 27, part: "query"));
            AreEqual("a=1&b=2", Link.TestTo(pageId: 27, parameters: "a=1&b=2", part: "query"));
            AreEqual("a=1&b=2", Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment", part: "query"));
            AreEqual(string.Empty, Link.TestTo(pageId: 27, parameters: "#fragment", part: "query"));
        }

        [TestMethod]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual("param=a", Link.TestTo(api: "", part: "query"));
        }

        [TestMethod]
        public void ToApiTest()
        {
            AreEqual(string.Empty, Link.TestTo(api: "/", part: "query"));
            AreEqual("a=1&b=2", Link.TestTo(api: "/", parameters: "a=1&b=2#fragment", part: "query"));
            AreEqual(string.Empty, Link.TestTo(api: "~/api", parameters: "#fragment", part: "query"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual("param=b&b=3&c=3", Link.TestTo(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3#fragment", part: "query"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual(string.Empty, Link.TestTo(api: "http://unknown2.2sxc.org/", part: "query"));
            AreEqual("a=1", Link.TestTo(api: "http://unknown2.2sxc.org/app/api", parameters: "a=1#fragment", part: "query"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello3:there", Link.TestTo(api: "hello3:there", parameters: "#fragment", part: "query"));
            AreEqual("file:456", Link.TestTo(api: "file:456", parameters: "a=1#fragment", part: "query"));

            // Invalid URI: The format of the URI could not be determined.
            AreEqual("../file.ext", Link.TestTo(api: "../file.ext", parameters: "#fragment", part: "query"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestTo(api: "/sibling1/../sibling2/image.jpg", parameters: "b=2#fragment", part: "query"));
        }

        [TestMethod]
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual("param=a", Link.Image(url: "", part: "query"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(string.Empty, Link.Image(url: "/", part: "query"));
            AreEqual("a=1&b=2", Link.Image(url: "/?a=1&b=2#fragment", part: "query"));
            AreEqual(string.Empty, Link.Image(url: "/page#fragment", part: "query"));
            AreEqual("a=1&b=2", Link.Image(url: "/page?a=1&b=2#fragment", part: "query"));
            AreEqual(string.Empty, Link.Image(url: "~/", part: "query"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual("param=a&c=3", Link.Image(url: "?c=3", part: "query"));
            AreEqual("param=a", Link.Image(url: "?#fragmentB", part: "query"));
            AreEqual("param=c", Link.Image(url: "?param=c#fragmentB", part: "query"));
            AreEqual("param=a", Link.Image(url: "#fragmentB", part: "query"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(string.Empty, Link.Image(url: "//unknown.2sxc.org/test#fragment", part: "query"));
            AreEqual("a=1", Link.Image(url: "//unknown.2sxc.org/test?a=1#fragment", part: "query"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(string.Empty, Link.Image(url: "http://unknown2.2sxc.org/#fragment", part: "query"));
            AreEqual("a=1", Link.Image(url: "https://unknown2.2sxc.org/page?a=1#fragmentB", part: "query"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.Image(url: "hello:there", part: "query"));
            AreEqual("file:593902", Link.Image(url: "file:593902", part: "query"));

            AreEqual("../file.ext", Link.Image(url: "../file.ext", part: "query"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.Image(url: "/sibling1/../sibling2/image.jpg", part: "query"));
        }
    }
}