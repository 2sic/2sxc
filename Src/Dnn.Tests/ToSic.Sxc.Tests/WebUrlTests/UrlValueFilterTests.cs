using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ToSic.Sxc.Web.Url;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebUrlTests
{
    [TestClass]
    public class UrlValueFilterTests
    {
        [TestMethod]
        public void NoFilterKeepAll()
        {
            var filter = new UrlValueFilter(true, new List<string>());
            var result = filter.FilterValues("something", "value");
            IsTrue(result.Keep);
        }

        [TestMethod]
        public void NoFilterKeepNone()
        {
            var filter = new UrlValueFilter(false, new List<string>());
            var result = filter.FilterValues("something", "value");
            IsFalse(result.Keep);
        }

        [TestMethod]
        public void FilterSomeKeepRest()
        {
            var filter = new UrlValueFilter(true, new string[] { "drop" });
            IsTrue(filter.FilterValues("something", "value").Keep);
            IsTrue(filter.FilterValues("something2", "value").Keep);
            IsTrue(filter.FilterValues("drop2", "value").Keep);
            IsFalse(filter.FilterValues("drop", "value").Keep, "this is the only one it should drop");
            IsFalse(filter.FilterValues("Drop", "value").Keep, "this should also fail, case insensitive");
        }

        [TestMethod]
        public void FilterSomeDropRest()
        {
            var filter = new UrlValueFilter(false, new string[] { "keep" });
            IsFalse(filter.FilterValues("something", "value").Keep);
            IsFalse(filter.FilterValues("something2", "value").Keep);
            IsFalse(filter.FilterValues("Drop", "value").Keep);
            IsFalse(filter.FilterValues("drop2", "value").Keep);
            IsTrue(filter.FilterValues("keep", "value").Keep, "this is th only one it should keep");
        }
    }
}
