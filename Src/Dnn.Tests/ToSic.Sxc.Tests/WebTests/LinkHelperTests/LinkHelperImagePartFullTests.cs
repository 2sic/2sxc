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
            AreEqual(testUrl, Link.TestImage(url: testUrl, part: "full"));
        }

        [TestMethod]
        public void ImageNoUrlOrParamsTest()
        {
            // todo: this looks wrong - if the image is empty, it shouldn't link the page
            // in that case it should at most add the url to empty
            AreEqual(LinkHelperUnknown.DefRoot /*$"{LinkHelperUnknown.MockHost}/folder/subfolder/page?param=a#fragment"*/, Link.TestImage(url: "", part: "full"));
        }

        [TestMethod]
        public void ImageCommonUrlsTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/", Link.TestImage(url: "/", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/?a=1&b=2#fragment", Link.TestImage(url: "/?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page", Link.TestImage(url: "/page", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page?a=1&b=2#fragment", Link.TestImage(url: "/page?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/file.ext", Link.TestImage(url: "/file.ext", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/file.ext?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/", Link.TestImage(url: "/folder/", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/page", Link.TestImage(url: "/folder/page", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/page?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/file.ext", Link.TestImage(url: "/folder/file.ext", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/file.ext?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/", Link.TestImage(url: "/folder/subfolder/", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page", Link.TestImage(url: "/folder/subfolder/page", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/page?a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/file.ext", Link.TestImage(url: "/folder/subfolder/file.ext", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/file.ext?a=1&b=2#fragment", part: "full"));
        }

        [TestMethod]
        public void ImageUrlPathIsMissingTest()
        {
            // TODO: this is all wrong - the Image should never point to the current page
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=a&c=3#fragment", Link.TestImage(url: "?c=3", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=c#fragment", Link.TestImage(url: "?param=c", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=b&b=3&c=3#fragment", Link.TestImage(url: "?param=b&b=3&c=3", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=a#fragmentB", Link.TestImage(url: "?#fragmentB", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=c#fragmentB", Link.TestImage(url: "?param=c#fragmentB", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/subfolder/page?param=a#fragmentC", Link.TestImage(url: "#fragmentC", part: "full"));
        }

        [TestMethod]
        public void ImageWithoutProtocolTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/test", Link.TestImage(url: "//unknown.2sxc.org/test", part: "full"));
        }

        [TestMethod]
        public void ImageUrlWithTildeTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/", Link.TestImage(url: "~", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/", Link.TestImage(url: "~/", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page", Link.TestImage(url: "~/page", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/file.ext", Link.TestImage(url: "~/file.ext", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/folder/", Link.TestImage(url: "~/folder/", part: "full"));
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
        public void ImageWithInvalidUrlTest()
        {
            ImagePartFullVerifyUrlAreEqual("hello:there");
            ImagePartFullVerifyUrlAreEqual("file:593902");
            ImagePartFullVerifyUrlAreEqual("../file.ext");
            ImagePartFullVerifyUrlAreEqual("/sibling1/../sibling2/image.jpg");
        }

    }
}