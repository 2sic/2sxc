using System;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests;

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