using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperImagePartFullTests: LinkHelperTestBase
    {
        private void ImagePartFullVerifyUrlAreEqual(string testUrl)
        {
            AreEqual(testUrl, Link.TestImage(url: testUrl, type: "full"));
        }

        [TestMethod]
        public void ImageNoUrlOrParamsTest()
        {
            // todo: this looks wrong - if the image is empty, it shouldn't link the page
            // in that case it should at most add the url to empty
            AreEqual(LinkHelperUnknown.DefRoot /*$"{LinkHelperUnknown.MockHost}/folder/subfolder/page?param=a#fragment"*/, Link.TestImage(url: "", type: "full"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/", Link.TestImage(url: "/", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/?a=1&b=2#fragment", Link.TestImage(url: "/?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page", Link.TestImage(url: "/page", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page?a=1&b=2#fragment", Link.TestImage(url: "/page?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/file.ext", Link.TestImage(url: "/file.ext", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/file.ext?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/", Link.TestImage(url: "/folder/", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/page", Link.TestImage(url: "/folder/page", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/page?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/file.ext", Link.TestImage(url: "/folder/file.ext", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/file.ext?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/", Link.TestImage(url: "/folder/subfolder/", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page", Link.TestImage(url: "/folder/subfolder/page", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/page?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/file.ext", Link.TestImage(url: "/folder/subfolder/file.ext", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/file.ext?a=1&b=2#fragment", type: "full"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            // TODO: this is all wrong - the Image should never point to the current page
            AreEqual($"{LinkHelperUnknown.DefRoot}?c=3", Link.TestImage(url: "?c=3", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}?param=c", Link.TestImage(url: "?param=c", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}?param=b&b=3&c=3", Link.TestImage(url: "?param=b&b=3&c=3", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}#fragmentB", Link.TestImage(url: "#fragmentB", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}#fragmentB", Link.TestImage(url: "?#fragmentB", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}?param=c#fragmentB", Link.TestImage(url: "?param=c#fragmentB", type: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}#fragmentC", Link.TestImage(url: "#fragmentC", type: "full"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual($"//{LinkHelperUnknown.DefDomain}/test", Link.TestImage(url: "//unknown.2sxc.org/test", type: "full"));
        }

        [TestMethod]
        public void ImageWithAbsoluteUrlTest()
        {
            ImagePartFullVerifyUrlAreEqual("https://unknown2.2sxc.org/");
            ImagePartFullVerifyUrlAreEqual("https://unknown2.2sxc.org/page");
            ImagePartFullVerifyUrlAreEqual("https://unknown2.2sxc.org/file.ext");
            ImagePartFullVerifyUrlAreEqual("https://unknown2.2sxc.org/folder/");
        }

        [TestMethod]
        [Ignore("ATM these tests fail, but it's actually not quite clear what should happen")]
        public void ImageWithInvalidUrlTest()
        {
            // ImagePartFullVerifyUrlAreEqual("hello:there");
            ImagePartFullVerifyUrlAreEqual("file:593902");
            ImagePartFullVerifyUrlAreEqual("../file.ext");
            ImagePartFullVerifyUrlAreEqual("/sibling1/../sibling2/image.jpg");
        }

    }
}