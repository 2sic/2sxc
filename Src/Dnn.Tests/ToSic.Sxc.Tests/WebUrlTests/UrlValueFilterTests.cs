using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ToSic.Sxc.Web.Url;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebUrlTests
{
    [TestClass]
    public class UrlValueFilterTests
    {
        private UrlValueFilter TestFilter(bool defaultSerialize, IEnumerable<string> opposite)
            => new UrlValueFilter(defaultSerialize, opposite);

        [TestMethod]
        public void NoFilterKeepAll()
        {
            var filter = TestFilter(true, new List<string>());
            var result = filter.FilterValues(new NameObjectSet("something", "value"));
            IsTrue(result.Keep);
        }

        [TestMethod]
        public void NoFilterKeepNone()
        {
            var filter = TestFilter(false, new List<string>());
            var result = filter.FilterValues(new NameObjectSet("something", "value"));
            IsFalse(result.Keep);
        }

        [TestMethod]
        public void FilterSomeKeepRest()
        {
            var filter = TestFilter(true, new[] { "drop" });
            IsTrue(filter.FilterValues(new NameObjectSet("something", "value")).Keep);
            IsTrue(filter.FilterValues(new NameObjectSet("something2", "value")).Keep);
            IsTrue(filter.FilterValues(new NameObjectSet("drop2", "value")).Keep);
            IsFalse(filter.FilterValues(new NameObjectSet("drop", "value")).Keep, "this is the only one it should drop");
            IsFalse(filter.FilterValues(new NameObjectSet("Drop", "value")).Keep, "this should also fail, case insensitive");
        }

        [TestMethod]
        public void FilterSomeDropRest()
        {
            var filter = TestFilter(false, new[] { "keep" });
            IsFalse(filter.FilterValues(new NameObjectSet("something", "value")).Keep);
            IsFalse(filter.FilterValues(new NameObjectSet("something2", "value")).Keep);
            IsFalse(filter.FilterValues(new NameObjectSet("Drop", "value")).Keep);
            IsFalse(filter.FilterValues(new NameObjectSet("drop2", "value")).Keep);
            IsTrue(filter.FilterValues(new NameObjectSet("keep", "value")).Keep, "this is th only one it should keep");
        }
    }
}
