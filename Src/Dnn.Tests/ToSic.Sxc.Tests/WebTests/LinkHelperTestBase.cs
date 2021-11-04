using ToSic.Sxc.Tests.WebTests.LinkHelperTests;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests
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


        // @STV - don't use statics in tests - results in object-reuse, but we want to always run clean
        internal /*static*/ void ToUrlAreEqual(string testUrl, string part = null) 
            => AreEqual(testUrl, Link.TestTo(part: part));
    }
}
