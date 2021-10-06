using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    [Ignore("part is not implemented for now")]
    public class LinkHelperPartProtocolTests : LinkHelperTestBase
    {
        private const string Http = "http";
        private const string Https = "https";

        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(Https, Link.TestTo(part: "protocol"));
        }

        [TestMethod]
        public void ToPageTest()
        {
            AreEqual(Https, Link.TestTo(pageId: 27, part: "protocol"));
            AreEqual(Https, Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment", part: "protocol"));
        }

        [TestMethod]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual(Https, Link.TestTo(api: "", part: "protocol"));
        }

        [TestMethod]
        public void ToApiTest()
        {
            AreEqual(Https, Link.TestTo(api: "/", part: "protocol"));
            AreEqual(Https, Link.TestTo(api: "/", parameters: "a=1&b=2#fragment", part: "protocol"));
            AreEqual(Https, Link.TestTo(api: "/app/api", part: "protocol"));
            AreEqual(Https, Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment", part: "protocol"));
            AreEqual(Https, Link.TestTo(api: "~/api", parameters: "p=1&r=2", part: "protocol"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual(Https, Link.TestTo(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3", part: "protocol"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual(Http, Link.TestTo(api: $"{Http}://unknown2.2sxc.org/", part: "protocol"));
            AreEqual(Https, Link.TestTo(api: $"{Https}://unknown2.2sxc.org/api", part: "protocol"));
            AreEqual(Http, Link.TestTo(api: $"{Http}://unknown2.2sxc.org/app/api", parameters: "a=1", part: "protocol"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello3:there", Link.TestTo(api: "hello3:there", part: "protocol"));
            AreEqual("file:456", Link.TestTo(api: "file:456", part: "protocol"));

            // Invalid URI: The format of the URI could not be determined.
            AreEqual("../file.ext", Link.TestTo(api: "../file.ext", part: "protocol"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestTo(api: "/sibling1/../sibling2/image.jpg", part: "protocol"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual(Https, Link.TestImage(url: "", type: "protocol"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageCommonUrlsTest()
        {
            AreEqual(Https, Link.TestImage(url: "/", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "/?a=1&b=2#fragment", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "/page", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "/page?a=1&b=2#fragment", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "~/", type: "protocol"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual(Https, Link.TestImage(url: "?c=3", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "?#fragmentB", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "?param=c#fragmentB", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: "#fragmentC", type: "protocol"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(Https, Link.TestImage(url: "//unknown.2sxc.org/test", type: "protocol"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(Http, Link.TestImage(url: $"{Http}://unknown2.2sxc.org/", type: "protocol"));
            AreEqual(Https, Link.TestImage(url: $"{Https}://unknown2.2sxc.org/page", type: "protocol"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.TestImage(url: "hello:there", type: "protocol"));
            AreEqual("file:593902", Link.TestImage(url: "file:593902", type: "protocol"));

            AreEqual("../file.ext", Link.TestImage(url: "../file.ext", type: "protocol"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestImage(url: "/sibling1/../sibling2/image.jpg", type: "protocol"));
        }
    }
}