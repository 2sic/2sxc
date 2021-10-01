using ToSic.Sxc.Web;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    public class LinkHelperResolver
    {
        public static ILinkHelper LinkHelper()
        {
            var linkHelper = EavTestBase.Resolve<ILinkHelper>();
            return linkHelper;
        }
    }
}