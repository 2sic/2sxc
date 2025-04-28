using ToSic.Sxc.Services;
using static ToSic.Sxc.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperToApiPartUndefinedTests(ILinkService Link)
{
    private void ToApiPartUndefinedVerifyUrlEqual(string testUrl)
    {
        Equal(testUrl, Link.TestTo(api: testUrl));
    }

    [Fact]
    public void ToNoApiOrParamsTest()
    {
        ToApiPartUndefinedVerifyUrlEqual("");
    }

    [Fact]
    public void ToApiCommonUrlsTest()
    {
        Equal($"/", Link.TestTo(api: "/"));
        Equal($"/?a=1&b=2#fragment", Link.TestTo(api: "/", parameters: "a=1&b=2#fragment"));
        Equal($"/api", Link.TestTo(api: "/api"));
        Equal($"/api?a=1&b=2#fragment", Link.TestTo(api: "/api", parameters: "a=1&b=2#fragment"));
        Equal($"/app/", Link.TestTo(api: "/app/"));
        Equal($"/app/?a=1&b=2#fragment", Link.TestTo(api: "/app/", parameters: "a=1&b=2#fragment"));
        Equal($"/app/api", Link.TestTo(api: "/app/api"));
        Equal($"/app/api?a=1&b=2#fragment", Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment"));
    }

    [Fact]
    public void ToApiParametersTest()
    {
        Equal($"/app/api", Link.TestTo(api: "/app/api"));
        Equal($"/app/api", Link.TestTo(api: "/app/api", parameters: null));
        Equal($"/app/api?a=1&b=2#fragment", Link.TestTo(api: "/app/api", parameters: "a=1&b=2#fragment"));
        Equal($"/app/api?a=1&b=2&c=3", Link.TestTo(api: "/app/api", parameters: NewParameters(new()
        {
            { "a", "1" },
            { "b", "2" },
            { "c", "3" }
        })));
    }

    [Fact]
    public void ToApiPathIsMissingTest()
    {
        Equal($"?param=b&b=3&c=3", Link.TestTo(api: "", parameters: "param=b&b=3&c=3"));
    }

    [Fact]
    public void ToApiWithoutProtocolTest() // current behavior, potentially we can improve like in part "full"
    {
        Equal($"//unknown.2sxc.org/api?param=b&b=3&c=3", Link.TestTo(api: "//unknown.2sxc.org/api", parameters: "param=b&b=3&c=3"));
    }

    [Fact]
    public void ToApiWithTildeTest() // current behavior, potentially we can improve like in part "full"
    {
        Equal($"~/api?p=1&r=2", Link.TestTo(api: "~/api", parameters: "p=1&r=2"));
        Equal($"~/app/", Link.TestTo(api: "~/app/"));
    }

    [Fact]
    public void ToApiWithAbsoluteUrlTest()
    {
        ToApiPartUndefinedVerifyUrlEqual("https://unknown2.2sxc.org/");
        ToApiPartUndefinedVerifyUrlEqual("https://unknown2.2sxc.org/api");
        Equal("https://unknown2.2sxc.org/app/api?a=1", Link.TestTo(api: "https://unknown2.2sxc.org/app/api", parameters: "a=1"));
    }

    [Fact]
    public void ToApiWithInvalidUrlTest()
    {
        ToApiPartUndefinedVerifyUrlEqual("hello2:there");
        ToApiPartUndefinedVerifyUrlEqual("file:123");
        ToApiPartUndefinedVerifyUrlEqual("../api");
        ToApiPartUndefinedVerifyUrlEqual("/sibling1/../sibling2/api");
    }
}