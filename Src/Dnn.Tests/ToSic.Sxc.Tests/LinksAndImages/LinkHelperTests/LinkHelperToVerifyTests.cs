using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToVerifyTests : LinkHelperToTestBase
    {

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void TestTheTestStandard() => TestToPageParts(null, standard: "somethingwrong");
        
    }
}
