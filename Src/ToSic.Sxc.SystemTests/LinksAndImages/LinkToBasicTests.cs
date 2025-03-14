using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests;

namespace ToSic.Sxc.Tests.LinksAndImages;

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