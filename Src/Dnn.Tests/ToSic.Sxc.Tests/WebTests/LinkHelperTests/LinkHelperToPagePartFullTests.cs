using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    [Ignore("part is not implemented for now")]
    public class LinkHelperToPagePartFullTests: LinkHelperTestBase
    {
        //[TestMethod]
        //public void ToNoPageIdOrParamsTest()
        //{
        //    AreEqual($"{LinkHelperUnknown.MockHost}/page", Link.TestTo(part: "full"));
        //}

        [TestMethod]
        public void ToPageCommonsTest()
        {
            //AreEqual($"{LinkHelperUnknown.MockHost}/page", Link.TestTo(pageId: null, part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page{27}", Link.TestTo(pageId: 27, part: "full"));
        }

        [TestMethod]
        public void ToPageParametersTest()
        {
            AreEqual($"{LinkHelperUnknown.DefRoot}/page27", Link.TestTo(pageId: 27, part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page27", Link.TestTo(pageId: 27, parameters: null, part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page27?a=1&b=2#fragment", Link.TestTo(pageId: 27, parameters: "a=1&b=2#fragment", part: "full"));
            AreEqual($"{LinkHelperUnknown.DefRoot}/page27?a=1&b=2&c=3", Link.TestTo(pageId: 27, parameters: new Parameters(new NameValueCollection
            {
                { "a", "1" },
                { "b", "2" },
                { "c", "3" }
            }), part: "full"));
        }
    }
}