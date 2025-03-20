using ToSic.Sxc.Services;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperImagePartUndefinedTests(ILinkService Link)
{
    private void ImageVerifyUrlEqual(string testUrl)
    {
        Equal(testUrl, Link.TestImage(url: testUrl));
    }

    [Fact]
    public void ImageNoUrlOrParamsTest()
    {
        ImageVerifyUrlEqual("");
    }

    [Fact]
    public void ImageCommonUrlsTest()
    {
        ImageVerifyUrlEqual("/");
        ImageVerifyUrlEqual("/?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/page");
        ImageVerifyUrlEqual("/page?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/file.ext");
        ImageVerifyUrlEqual("/file.ext?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/folder/");
        ImageVerifyUrlEqual("/folder/?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/folder/page");
        ImageVerifyUrlEqual("/folder/page?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/folder/file.ext");
        ImageVerifyUrlEqual("/folder/file.ext?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/folder/subfolder/");
        ImageVerifyUrlEqual("/folder/subfolder/?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/folder/subfolder/page");
        ImageVerifyUrlEqual("/folder/subfolder/page?a=1&b=2#fragment");
        ImageVerifyUrlEqual("/folder/subfolder/file.ext");
        ImageVerifyUrlEqual("/folder/subfolder/file.ext?a=1&b=2#fragment");
    }

    [Fact]
    public void ImageUrlPathIsMissingTest()
    {
        ImageVerifyUrlEqual("?c=3");
        ImageVerifyUrlEqual("?param=c");
        ImageVerifyUrlEqual("?param=b&b=3&c=3");
        ImageVerifyUrlEqual("?#fragmentB");
        ImageVerifyUrlEqual("?param=c#fragmentB");
        ImageVerifyUrlEqual("#fragmentC");
    }

    [Fact]
    public void ImageWithoutProtocolTest() // current behavior, potentially we can improve like in part "full"
    {
        Equal("//unknown.2sxc.org/test", Link.TestImage(url: "//unknown.2sxc.org/test"));
    }

    [Fact]
    public void ImageUrlWithTildeTest() // current behavior, potentially we can improve like in part "full"
    {
        Equal("~", Link.TestImage(url: "~"));
        Equal("~/", Link.TestImage(url: "~/"));
        Equal("~/page", Link.TestImage(url: "~/page"));
        Equal("~/file.ext", Link.TestImage(url: "~/file.ext"));
        Equal("~/folder/", Link.TestImage(url: "~/folder/"));
    }

    [Fact]
    public void ImageWithAbsoluteUrlTest()
    {
        ImageVerifyUrlEqual("https://unknown2.2sxc.org/");
        ImageVerifyUrlEqual("https://unknown2.2sxc.org/page");
        ImageVerifyUrlEqual("https://unknown2.2sxc.org/file.ext");
        ImageVerifyUrlEqual("https://unknown2.2sxc.org/folder/");
    }

    [Fact]
    public void ImageWithInvalidUrlTest()
    {
        ImageVerifyUrlEqual("hello:there");
        ImageVerifyUrlEqual("file:593902");
        ImageVerifyUrlEqual("../file.ext");
        ImageVerifyUrlEqual("/sibling1/../sibling2/image.jpg");
    }

}