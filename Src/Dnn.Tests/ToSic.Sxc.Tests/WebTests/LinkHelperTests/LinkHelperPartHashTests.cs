using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperPartHashTests : LinkHelperTestBase
    {
        private const string Fragment = "fragment";
        private const string FragmentB = "fragmentB";

        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual(string.Empty, Link.TestTo(part: "hash"));
        }

        [TestMethod]
        public void ToPageTest()
        {
            AreEqual(string.Empty, Link.TestTo(pageId: 27, part: "hash"));
            AreEqual(Fragment, Link.TestTo(pageId: 27, parameters: $"a=1&b=2#{Fragment}", part: "hash"));
            AreEqual(Fragment, Link.TestTo(pageId: 27, parameters: $"#{Fragment}", part: "hash"));
        }

        [TestMethod]
        public void ToNoApiUrlOrParamsTest()
        {
            AreEqual("", Link.TestTo(api: "", part: "hash"));
            AreEqual(Fragment, Link.TestTo(api: "#" + Fragment, part: "hash"));
        }

        [TestMethod]
        public void ToApiTest()
        {
            AreEqual(string.Empty, Link.TestTo(api: "/", part: "hash"));
            AreEqual(Fragment, Link.TestTo(api: "/", parameters: $"a=1&b=2#{Fragment}", part: "hash"));
            AreEqual(Fragment, Link.TestTo(api: "~/api", parameters: $"#{Fragment}", part: "hash"));
        }

        [TestMethod]
        public void ToApiWithoutProtocolTest()
        {
            AreEqual(Fragment, Link.TestTo(api: "//unknown.2sxc.org/api", parameters: $"param=b&b=3&c=3#{Fragment}", part: "hash"));
        }

        [TestMethod]
        public void ToApiWithAbsoluteUrlTest()
        {
            AreEqual(string.Empty, Link.TestTo(api: "http://unknown2.2sxc.org/", part: "hash"));
            AreEqual(Fragment, Link.TestTo(api: "http://unknown2.2sxc.org/app/api", parameters: $"#{Fragment}", part: "hash"));
        }

        [TestMethod]
        public void ToApiWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello3:there", Link.TestTo(api: "hello3:there", parameters: $"#{Fragment}", part: "hash"));
            AreEqual("file:456", Link.TestTo(api: "file:456", parameters: $"a=1#{Fragment}", part: "hash"));

            // Invalid URI: The format of the URI could not be determined.
            AreEqual("../file.ext", Link.TestTo(api: "../file.ext", parameters: $"#{Fragment}", part: "hash"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestTo(api: "/sibling1/../sibling2/image.jpg", parameters: $"b=2#{Fragment}", part: "hash"));
        }

        [TestMethod]
        public void ImageNoUrlOrParamsTest()
        {
            AreEqual(Fragment, Link.TestImage(url: "", part: "hash"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual(string.Empty, Link.TestImage(url: "/", part: "hash"));
            AreEqual(Fragment, Link.TestImage(url: $"/?a=1&b=2#{Fragment}", part: "hash"));
            AreEqual(string.Empty, Link.TestImage(url: "/page", part: "hash"));
            AreEqual(Fragment, Link.TestImage(url: $"/page?a=1&b=2#{Fragment}", part: "hash"));
            AreEqual(string.Empty, Link.TestImage(url: "~/", part: "hash"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual(Fragment, Link.TestImage(url: "?c=3", part: "hash"));
            AreEqual(FragmentB, Link.TestImage(url: $"?#{FragmentB}", part: "hash"));
            AreEqual(FragmentB, Link.TestImage(url: $"?param=c#{FragmentB}", part: "hash"));
            AreEqual(FragmentB, Link.TestImage(url: $"#{FragmentB}", part: "hash"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual(Fragment, Link.TestImage(url: $"//unknown.2sxc.org/test#{Fragment}", part: "hash"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            AreEqual(Fragment, Link.TestImage(url: $"http://unknown2.2sxc.org/#{Fragment}", part: "hash"));
            AreEqual(FragmentB, Link.TestImage(url: $"https://unknown2.2sxc.org/page#{FragmentB}", part: "hash"));
        }

        [TestMethod]
        public void ImageWithInvalidUrlTest()
        {
            Assert.Inconclusive("TODO: Need to define behavior in this case.");

            AreEqual("hello:there", Link.TestImage(url: "hello:there", part: "hash"));
            AreEqual("file:593902", Link.TestImage(url: "file:593902", part: "hash"));

            AreEqual("../file.ext", Link.TestImage(url: "../file.ext", part: "hash"));
            AreEqual("/sibling1/../sibling2/image.jpg", Link.TestImage(url: "/sibling1/../sibling2/image.jpg", part: "hash"));
        }
    }
}