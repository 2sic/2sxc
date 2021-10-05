using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
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
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual(Https, Link.Image(url: "", part: "protocol"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(Https, Link.Image(url: "/", part: "protocol"));
            AreEqual(Https, Link.Image(url: "/?a=1&b=2#fragment", part: "protocol"));
            AreEqual(Https, Link.Image(url: "/page", part: "protocol"));
            AreEqual(Https, Link.Image(url: "/page?a=1&b=2#fragment", part: "protocol"));
            AreEqual(Https, Link.Image(url: "~/", part: "protocol"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual(Https, Link.Image(url: "?c=3", part: "protocol"));
            AreEqual(Https, Link.Image(url: "?#fragmentB", part: "protocol"));
            AreEqual(Https, Link.Image(url: "?param=c#fragmentB", part: "protocol"));
            AreEqual(Https, Link.Image(url: "#fragmentC", part: "protocol"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(Https, Link.Image(url: "//unknown.2sxc.org/test", part: "protocol"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(Http, Link.Image(url: $"{Http}://unknown2.2sxc.org/", part: "protocol"));
            AreEqual(Https, Link.Image(url: $"{Https}://unknown2.2sxc.org/page", part: "protocol"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.Image(url: "hello:there", part: "protocol"));
            AreEqual("file:593902", Link.Image(url: "file:593902", part: "protocol"));

            AreEqual("../file.ext", Link.Image(url: "../file.ext", part: "protocol"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.Image(url: "/sibling1/../sibling2/image.jpg", part: "protocol"));
        }
    }
}