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
            AreEqual("param=a", Link.TestImage(url: "", part: "query"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(string.Empty, Link.TestImage(url: "/", part: "query"));
            AreEqual("a=1&b=2", Link.TestImage(url: "/?a=1&b=2#fragment", part: "query"));
            AreEqual(string.Empty, Link.TestImage(url: "/page#fragment", part: "query"));
            AreEqual("a=1&b=2", Link.TestImage(url: "/page?a=1&b=2#fragment", part: "query"));
            AreEqual(string.Empty, Link.TestImage(url: "~/", part: "query"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual("param=a&c=3", Link.TestImage(url: "?c=3", part: "query"));
            AreEqual("param=a", Link.TestImage(url: "?#fragmentB", part: "query"));
            AreEqual("param=c", Link.TestImage(url: "?param=c#fragmentB", part: "query"));
            AreEqual("param=a", Link.TestImage(url: "#fragmentB", part: "query"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(string.Empty, Link.TestImage(url: "//unknown.2sxc.org/test#fragment", part: "query"));
            AreEqual("a=1", Link.TestImage(url: "//unknown.2sxc.org/test?a=1#fragment", part: "query"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(string.Empty, Link.TestImage(url: "http://unknown2.2sxc.org/#fragment", part: "query"));
            AreEqual("a=1", Link.TestImage(url: "https://unknown2.2sxc.org/page?a=1#fragmentB", part: "query"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.TestImage(url: "hello:there", part: "query"));
            AreEqual("file:593902", Link.TestImage(url: "file:593902", part: "query"));

            AreEqual("../file.ext", Link.TestImage(url: "../file.ext", part: "query"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestImage(url: "/sibling1/../sibling2/image.jpg", part: "query"));
        }
    }
}