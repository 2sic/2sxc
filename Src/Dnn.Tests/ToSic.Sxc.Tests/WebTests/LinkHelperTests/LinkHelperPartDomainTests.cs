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
        [Ignore("part is not implemented for now")]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(Domain, Link.TestTo(part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ToPageTest()
        {
            AreEqual(Domain, Link.TestTo(pageId: 27, part: "domain"));
            AreEqual(Domain, Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual(Domain, Link.TestTo(api: "", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ToApiTest()
        {
            AreEqual(Domain, Link.TestTo(api: "/", part: "domain"));
            AreEqual(Domain, Link.TestTo(api: "/", parameters: "a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.TestTo(api: "/app/api", part: "domain"));
            AreEqual(Domain, Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.TestTo(api: "~/api", parameters: "p=1&r=2", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual(Domain, Link.TestTo(api: $"//{Domain}/api", parameters: "param=b&b=3&c=3", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual(Domain2, Link.TestTo(api: $"http://{Domain2}/", part: "domain"));
            AreEqual(Domain2, Link.TestTo(api: $"http://{Domain2}/api", part: "domain"));
            AreEqual(Domain2, Link.TestTo(api: $"http://{Domain2}/app/api", parameters: "a=1", part: "domain"));
        }

        [TestMethod]
        [Ignore("TODO: behavior not clear yen")]
        public void ToApiWithInvalidUrlTest()
        {
            // Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello3:there", Link.TestTo(api: "hello3:there", part: "domain"));
            AreEqual("file:456", Link.TestTo(api: "file:456", part: "domain"));

            // Invalid URI: The format of the URI could not be determined.
            AreEqual("../file.ext", Link.TestTo(api: "../file.ext", part: "domain"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestTo(api: "/sibling1/../sibling2/image.jpg", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual(Domain, Link.TestImage(url: "", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageCommonUrlsTest()
        {
            AreEqual(Domain, Link.TestImage(url: "/", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "/?a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "/page", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "/page?a=1&b=2#fragment", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "~/", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual(Domain, Link.TestImage(url: "?c=3", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "?#fragmentB", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "?param=c#fragmentB", part: "domain"));
            AreEqual(Domain, Link.TestImage(url: "#fragmentC", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(Domain, Link.TestImage(url: $"//{Domain}/test", part: "domain"));
        }

        [TestMethod]
        [Ignore("part is not implemented for now")]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(Domain2, Link.TestImage(url: $"http://{Domain2}/", part: "domain"));
            AreEqual(Domain2, Link.TestImage(url: $"https://{Domain2}/page", part: "domain"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.TestImage(url: "hello:there", part: "domain"));
            AreEqual("file:593902", Link.TestImage(url: "file:593902", part: "domain"));

            AreEqual("../file.ext", Link.TestImage(url: "../file.ext", part: "domain"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestImage(url: "/sibling1/../sibling2/image.jpg", part: "domain"));
        }
    }
}