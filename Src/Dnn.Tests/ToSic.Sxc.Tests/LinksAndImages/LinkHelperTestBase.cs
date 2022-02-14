using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.LinksAndImages
{
    public class LinkHelperTestBase: TestBaseSxc
    {
        /// <summary>
        /// 
        /// </summary>
        public LinkHelperTestBase()
        {
            // @STV - don't use statics in tests - can cause unexpected results across tests
            // Every test should run by itself

            Link = Build<ILinkHelper>();
        }

        internal ILinkHelper Link;
    }
}
