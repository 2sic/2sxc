using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Tests.WebTests.LinkHelperTests;
using ToSic.Sxc.Web;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests
{
    [TestClass]
    public class LinkToBasicTests: LinkHelperTestBase
    {
        [TestMethod]
        public void NormalPage()
        {
            Assert.AreEqual($"{LinkHelperUnknown.DefRoot}/page0", Link.TestTo(pageId: 0));
            Assert.AreEqual($"{LinkHelperUnknown.DefRoot}/page27", Link.TestTo(pageId: 27));
        }
    }
}
