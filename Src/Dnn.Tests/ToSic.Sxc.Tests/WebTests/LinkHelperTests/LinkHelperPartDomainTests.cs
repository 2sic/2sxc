using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperPartDomainTests : LinkHelperTestBase
    {
        private const string Domain = "unknown.2sxc.org";
        private const string Domain2 = "unknown2.2sxc.org";

        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(Domain, Link.To(part: "domain"));
        }

        [TestMethod]
        public void ToPageTest()
        {
            AreEqual(Domain, Link.To(pageId: 27, part: "domain"));
            AreEqual(Domain, Link.To(pageId: 27, parameters: "a=1&b=2#fragment", part: "domain"));
        }

        [TestMethod]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual(Domain, Link.To(api: "", part: "domain"));
        }

        [TestMethod]
        public void ToApiTest()
        {
            AreEqual(Domain, Link.To(api: "/", part: "domain"));
            AreEqual(Domain, Link.To(api: "/", parameters: "a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.To(api: "/app/api", part: "domain"));
            AreEqual(Domain, Link.To(api: "/app/api", parameters: "a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.To(api: "~/api", parameters: "p=1&r=2", part: "domain"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual(Domain, Link.To(api: $"//{Domain}/api", parameters: "param=b&b=3&c=3", part: "domain"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual(Domain2, Link.To(api: $"http://{Domain2}/", part: "domain"));
            AreEqual(Domain2, Link.To(api: $"http://{Domain2}/api", part: "domain"));
            AreEqual(Domain2, Link.To(api: $"http://{Domain2}/app/api", parameters: "a=1", part: "domain"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello3:there", Link.To(api: "hello3:there", part: "domain"));
            AreEqual("file:456", Link.To(api: "file:456", part: "domain"));

            // Invalid URI: The format of the URI could not be determined.
            AreEqual("../file.ext", Link.To(api: "../file.ext", part: "domain"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.To(api: "/sibling1/../sibling2/image.jpg", part: "domain"));
        }

        [TestMethod]
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual(Domain, Link.Image(url: "", part: "domain"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(Domain, Link.Image(url: "/", part: "domain"));
            AreEqual(Domain, Link.Image(url: "/?a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.Image(url: "/page", part: "domain"));
            AreEqual(Domain, Link.Image(url: "/page?a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.Image(url: "~/", part: "domain"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual(Domain, Link.Image(url: "?c=3", part: "domain"));
            AreEqual(Domain, Link.Image(url: "?#fragmentB", part: "domain"));
            AreEqual(Domain, Link.Image(url: "?param=c#fragmentB", part: "domain"));
            AreEqual(Domain, Link.Image(url: "#fragmentC", part: "domain"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(Domain, Link.Image(url: $"//{Domain}/test", part: "domain"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(Domain2, Link.Image(url: $"http://{Domain2}/", part: "domain"));
            AreEqual(Domain2, Link.Image(url: $"https://{Domain2}/page", part: "domain"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.Image(url: "hello:there", part: "domain"));
            AreEqual("file:593902", Link.Image(url: "file:593902", part: "domain"));

            AreEqual("../file.ext", Link.Image(url: "../file.ext", part: "domain"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.Image(url: "/sibling1/../sibling2/image.jpg", part: "domain"));
        }
    }
}