using ToSic.Sxc.LinksAndImages.LinkHelperTests;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Link.Sys;

namespace ToSic.Sxc.LinksAndImages;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkToBasicTests(ILinkService Link)
{
    [Fact]
    public void NormalPage()
    {
        Equal($"{LinkServiceUnknown.DefRoot}/page0", Link.TestTo(pageId: 0));
        Equal($"{LinkServiceUnknown.DefRoot}/page27", Link.TestTo(pageId: 27));
    }
}