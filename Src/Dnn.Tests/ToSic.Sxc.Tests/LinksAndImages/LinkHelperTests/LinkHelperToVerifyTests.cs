using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToVerifyTests : LinkHelperToTestBase
    {

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void TestTheTestStandard() => TestToPageParts(null, standard: "somethingwrong");
        
    }
}
