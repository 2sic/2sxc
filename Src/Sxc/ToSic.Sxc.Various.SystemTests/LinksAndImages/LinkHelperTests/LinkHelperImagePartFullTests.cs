using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Link.Sys;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperImagePartFullTests(ILinkService Link)
{
    private void ImagePartFullVerifyUrlEqual(string testUrl)
    {
        Equal(testUrl, Link.TestImage(url: testUrl, type: "full"));
    }

    [Fact]
    public void ImageNoUrlOrParamsTest()
    {
        // todo: this looks wrong - if the image is empty, it shouldn't link the page
        // in that case it should at most add the url to empty
        Equal(LinkServiceUnknown.DefRoot /*$"{LinkHelperUnknown.MockHost}/folder/subfolder/page?param=a#fragment"*/, Link.TestImage(url: "", type: "full"));
    }

    [Fact]
    public void ImageCommonUrlsTest()
    {
        Equal($"{LinkServiceUnknown.DefRoot}/", Link.TestImage(url: "/", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/?a=1&b=2#fragment", Link.TestImage(url: "/?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/page", Link.TestImage(url: "/page", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/page?a=1&b=2#fragment", Link.TestImage(url: "/page?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/file.ext", Link.TestImage(url: "/file.ext", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/file.ext?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/", Link.TestImage(url: "/folder/", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/page", Link.TestImage(url: "/folder/page", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/page?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/file.ext", Link.TestImage(url: "/folder/file.ext", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/file.ext?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/subfolder/", Link.TestImage(url: "/folder/subfolder/", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/subfolder/?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/subfolder/page", Link.TestImage(url: "/folder/subfolder/page", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/subfolder/page?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/page?a=1&b=2#fragment", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/subfolder/file.ext", Link.TestImage(url: "/folder/subfolder/file.ext", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}/folder/subfolder/file.ext?a=1&b=2#fragment", Link.TestImage(url: "/folder/subfolder/file.ext?a=1&b=2#fragment", type: "full"));
    }

    [Fact]
    public void ImageUrlPathIsMissingTest()
    {
        Equal($"{LinkServiceUnknown.DefRoot}?c=3", Link.TestImage(url: "?c=3", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}?param=c", Link.TestImage(url: "?param=c", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}?param=b&b=3&c=3", Link.TestImage(url: "?param=b&b=3&c=3", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}#fragmentB", Link.TestImage(url: "#fragmentB", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}#fragmentB", Link.TestImage(url: "?#fragmentB", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}?param=c#fragmentB", Link.TestImage(url: "?param=c#fragmentB", type: "full"));
        Equal($"{LinkServiceUnknown.DefRoot}#fragmentC", Link.TestImage(url: "#fragmentC", type: "full"));
    }

    [Fact]
    public void ImageWithoutProtocolTest()
    {
        Equal($"//{LinkServiceUnknown.DefDomain}/test", Link.TestImage(url: "//unknown.2sxc.org/test", type: "full"));
    }

    [Fact]
    public void ImageWithAbsoluteUrlTest()
    {
        ImagePartFullVerifyUrlEqual("https://unknown2.2sxc.org/");
        ImagePartFullVerifyUrlEqual("https://unknown2.2sxc.org/page");
        ImagePartFullVerifyUrlEqual("https://unknown2.2sxc.org/file.ext");
        ImagePartFullVerifyUrlEqual("https://unknown2.2sxc.org/folder/");
    }

    [Fact(Skip = "ATM these tests fail, but it's actually not quite clear what should happen")]
    public void ImageWithInvalidUrlTest()
    {
        // ImagePartFullVerifyUrlEqual("hello:there");
        ImagePartFullVerifyUrlEqual("file:593902");
        ImagePartFullVerifyUrlEqual("../file.ext");
        ImagePartFullVerifyUrlEqual("/sibling1/../sibling2/image.jpg");
    }

}