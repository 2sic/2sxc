using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static ToSic.Sxc.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperToPagePartUnknownTests (ILinkService Link)
{
    [Fact]
    public void ToNoPageIdOrParamsTest()
    {
        Equal(LinkServiceUnknown.NiceCurrentUrl, Link.TestTo());
    }

    [Fact]
    public void ToPageCommonsTest()
    {
        //Equal($"{LinkHelperUnknown.MockHost}/page", Link.TestTo(pageId: null));
        Equal($"{LinkServiceUnknown.DefRoot}/page{27}", Link.TestTo(pageId: 27));
    }

    [Fact]
    public void ToPageParametersTest()
    {
        Equal($"{LinkServiceUnknown.DefRoot}/page27", Link.TestTo(pageId: 27));
        Equal($"{LinkServiceUnknown.DefRoot}/page27", Link.TestTo(pageId: 27, parameters: null));
        Equal($"{LinkServiceUnknown.DefRoot}/page27?a=1&b=2#fragment", Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment"));
        Equal($"{LinkServiceUnknown.DefRoot}/page27?a=1&b=2&c=3", Link.TestTo(pageId: 27, parameters: NewParameters(new()
        {
            { "a", "1" },
            { "b", "2" },
            { "c", "3" }
        })));
    }
}