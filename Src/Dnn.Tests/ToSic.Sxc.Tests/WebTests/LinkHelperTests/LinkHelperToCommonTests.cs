using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToCommonTests : LinkHelperTestBase
    {
        [TestMethod]
        public void ToConflictingValuesProvidedTest()
        {
            ThrowsException<System.ArgumentException>(() =>
            {
                Link.TestTo(pageId: 27, api: "api");
            });
        }
    }
}