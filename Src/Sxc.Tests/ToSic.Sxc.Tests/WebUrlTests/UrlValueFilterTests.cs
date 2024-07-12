using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ToSic.Sxc.Web.Internal.Url;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebUrlTests
{
    [TestClass]
    public class UrlValueFilterTests
    {
        private UrlValueFilterNames TestFilter(bool defaultSerialize, IEnumerable<string> opposite) =>
            new(defaultSerialize, opposite);

        [TestMethod]
        public void NoFilterKeepAll()
        {
            var filter = TestFilter(true, new List<string>());
            var result = filter.Process(new("something", "value"));
            IsTrue(result.Keep);
        }

        [TestMethod]
        public void NoFilterKeepNone()
        {
            var filter = TestFilter(false, new List<string>());
            var result = filter.Process(new("something", "value"));
            IsFalse(result.Keep);
        }

        [TestMethod]
        public void FilterSomeKeepRest()
        {
            var filter = TestFilter(true, new[] { "drop" });
            IsTrue(filter.Process(new("something", "value")).Keep);
            IsTrue(filter.Process(new("something2", "value")).Keep);
            IsTrue(filter.Process(new("drop2", "value")).Keep);
            IsFalse(filter.Process(new("drop", "value")).Keep, "this is the only one it should drop");
            IsFalse(filter.Process(new("Drop", "value")).Keep, "this should also fail, case insensitive");
        }

        [TestMethod]
        public void FilterSomeDropRest()
        {
            var filter = TestFilter(false, new[] { "keep" });
            IsFalse(filter.Process(new("something", "value")).Keep);
            IsFalse(filter.Process(new("something2", "value")).Keep);
            IsFalse(filter.Process(new("Drop", "value")).Keep);
            IsFalse(filter.Process(new("drop2", "value")).Keep);
            IsTrue(filter.Process(new("keep", "value")).Keep, "this is th only one it should keep");
        }
    }
}
