using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests
{
    [TestClass]
    public class MergeNameValueCollectionTests
    {
        /// <summary>
        /// Test accessor
        /// </summary>
        /// <returns></returns>
        private NameValueCollection ImportTest(NameValueCollection first, NameValueCollection second, bool replace = false) =>
            first.Merge(second, replace);

        private const string Same = "!same!";

        private void Test(string exp, string expReplace, string first, string second)
        {
            var nvc1 = UrlHelpers.ParseQueryString(first);
            var itemsIn1 = nvc1.Count;
            var nvc2 = UrlHelpers.ParseQueryString(second);
            var itemsIn2 = nvc2.Count;
            var merged = ImportTest(nvc1, nvc2);
            Assert.AreEqual(itemsIn1, nvc1.Count, "Import shouldn't change first"); 
            Assert.AreEqual(itemsIn2, nvc2.Count, "Import shouldn't change second");
            Assert.AreEqual(exp, UrlHelpers.NvcToString(merged));

            merged = ImportTest(nvc1, nvc2, true);
            if (expReplace == Same) expReplace = exp;
            Assert.AreEqual(itemsIn1, nvc1.Count, "Import shouldn't change first"); 
            Assert.AreEqual(itemsIn2, nvc2.Count, "Import shouldn't change second");
            Assert.AreEqual(expReplace, UrlHelpers.NvcToString(merged));

        }

        [TestMethod] public void BasicMerge() => Test("first=1&second=2", Same, "first=1", "second=2");
        [TestMethod] public void LongerMerge() => Test("first=1&a=b&second=2&x=z", Same, "first=1&a=b", "second=2&x=z");
        [TestMethod] public void EmptyBoth() => Test("", Same, "", "");
        [TestMethod] public void EmptyFirst() => Test("second=2&x=z",Same, "", "second=2&x=z");
        [TestMethod] public void EmptySecond() => Test("first=1&a=b", Same, "first=1&a=b", "");

        [TestMethod] public void FirstJustKey() => Test("first&second=2&x=z", Same, "first", "second=2&x=z");

        [TestMethod] public void IdenticalKeys() => Test(
            "first=a&identical=a&identical=b&second=2&x=z", "first=a&identical=b&second=2&x=z",
            "first=a&identical=a", "identical=b&second=2&x=z");
    }
}
