using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass()]
    public class LinkHelperToCommonTests : EavTestBase
    {
        public static ILinkHelper Link = LinkHelper();

        private static ILinkHelper LinkHelper()
        {
            var linkHelper = Resolve<ILinkHelper>();
            return linkHelper;
        }

        [TestMethod()]
        public void ToConflictingValuesProvidedTest()
        {
            ThrowsException<System.ArgumentException>(() =>
            {
                Link.To(pageId: 27, api: "api");
            });
        }
    }
}