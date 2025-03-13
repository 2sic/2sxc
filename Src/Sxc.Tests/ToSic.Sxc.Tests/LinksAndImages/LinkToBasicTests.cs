using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages;

[TestClass]
public class LinkToBasicTests: LinkHelperTestBase
{
    [TestMethod]
    public void NormalPage()
    {
        AreEqual($"{LinkServiceUnknown.DefRoot}/page0", Link.TestTo(pageId: 0));
        AreEqual($"{LinkServiceUnknown.DefRoot}/page27", Link.TestTo(pageId: 27));
    }
}