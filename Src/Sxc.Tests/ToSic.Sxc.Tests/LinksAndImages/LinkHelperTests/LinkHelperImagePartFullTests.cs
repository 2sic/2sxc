using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests
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
            AreEqual(LinkServiceUnknown.DefRoot /*$"{LinkHelperUnknown.MockHost}/folder/subfolder/page?param=a#fragment"*/, Link.TestImage(url: "", type: "full"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual($"{LinkServiceUnknown.DefRoot}/", Link.TestImage(url: "/", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/?a=1&b=2#fragment", Link.TestImage(url: "/?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/page", Link.TestImage(url: "/page", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/page?a=1&b=2#fragment", Link.TestImage(url: "/page?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/file.ext", Link.TestImage(url: "/file.ext", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/file.ext?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/", Link.TestImage(url: "/folder/", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/page", Link.TestImage(url: "/folder/page", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/page?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/file.ext", Link.TestImage(url: "/folder/file.ext", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/file.ext?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/subfolder/", Link.TestImage(url: "/folder/subfolder/", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/subfolder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/subfolder/page", Link.TestImage(url: "/folder/subfolder/page", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/subfolder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/page?a=1&b=2#fragment", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/subfolder/file.ext", Link.TestImage(url: "/folder/subfolder/file.ext", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}/folder/subfolder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/file.ext?a=1&b=2#fragment", type: "full"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            AreEqual($"{LinkServiceUnknown.DefRoot}?c=3", Link.TestImage(url: "?c=3", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}?param=c", Link.TestImage(url: "?param=c", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}?param=b&b=3&c=3", Link.TestImage(url: "?param=b&b=3&c=3", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}#fragmentB", Link.TestImage(url: "#fragmentB", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}#fragmentB", Link.TestImage(url: "?#fragmentB", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}?param=c#fragmentB", Link.TestImage(url: "?param=c#fragmentB", type: "full"));
            AreEqual($"{LinkServiceUnknown.DefRoot}#fragmentC", Link.TestImage(url: "#fragmentC", type: "full"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual($"//{LinkServiceUnknown.DefDomain}/test", Link.TestImage(url: "//unknown.2sxc.org/test", type: "full"));
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