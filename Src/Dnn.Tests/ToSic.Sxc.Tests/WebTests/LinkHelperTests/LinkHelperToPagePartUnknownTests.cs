using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToPagePartUnknownTests : LinkHelperTestBase
    {
        [TestMethod]
        public void ToNoPageIdOrParamsTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/page", Link.TestTo());
        }

        [TestMethod]
        public void ToPageCommonsTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/page", Link.TestTo(pageId: null));
            AreEqual($"{LinkHelperUnknown.MockHost}/page{27}", Link.TestTo(pageId: 27));
        }

        [TestMethod]
        public void ToPageParametersTest()
        {
            AreEqual($"{LinkHelperUnknown.MockHost}/page27", Link.TestTo(pageId: 27));
            AreEqual($"{LinkHelperUnknown.MockHost}/page27", Link.TestTo(pageId: 27, parameters: null));
            AreEqual($"{LinkHelperUnknown.MockHost}/page27?a=1&b=2#fragment", Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment"));
            AreEqual($"{LinkHelperUnknown.MockHost}/page27?a=1&b=2&c=3", Link.TestTo(pageId: 27, parameters: new Parameters(new NameValueCollection
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            })));
        }
    }
}