using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToCommonTests : LinkHelperTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ToConflictingValuesProvidedTest()
        {
            Link.TestTo(pageId: 27, api: "api");
        }
    }
}