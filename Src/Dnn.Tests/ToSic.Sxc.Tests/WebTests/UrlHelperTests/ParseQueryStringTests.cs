using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web.Url;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.UrlHelperTests
{
    [TestClass]
    public class ParseQueryStringTests
    {
        /// <summary>
        /// Test accessor
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private NameValueCollection Parse(string query) => UrlHelpers.ParseQueryString(query);

        private const string UseExpected = "!use-expected!";
        private void TestPQS(string expected, int expCount, string query)
        {
            query = (query == UseExpected) ? expected : query;
            var result = Parse(query);
            AreEqual(expCount, result.Count);
            var reStringed = new Parameters(result).ToString();
            AreEqual(expected, reStringed);
        }

        [TestMethod] public void Null() => TestPQS("", 0, null);
        [TestMethod] public void Empty() => TestPQS("", 0, string.Empty);
        [TestMethod] public void Spaces() => TestPQS("", 0, "   ");

        [TestMethod] public void BasicPair() => TestPQS("2sxc=cool", 1, UseExpected);
        [TestMethod] public void BasicPairWithSpaces() => TestPQS("2sxc=cool", 1, " 2sxc=cool  ");

        // Special prefixes and suffixes
        [TestMethod] public void BasicPairWithQuestionPrefix() => TestPQS("2sxc=cool", 1, "?2sxc=cool");
        [TestMethod] public void BasicPairWithAndPrefix() => TestPQS("2sxc=cool", 1, "&2sxc=cool");
        [TestMethod] public void BasicPairWithAndSuffix() => TestPQS("2sxc=cool", 1, "&2sxc=cool&");


        [TestMethod] public void TwoPairs() => TestPQS("2sxc=cool&eav=power", 2, UseExpected);
        [TestMethod] public void SingleValue() => TestPQS("2sxc", 1, UseExpected);
        [TestMethod] public void SingleValues() => TestPQS("2sxc&eav&test", 3, UseExpected);
        [TestMethod] public void PairWithSingleValue() => TestPQS("2sxc=cool&activate", 2, UseExpected);
        [TestMethod] public void PairsWithSingleValues() => TestPQS("2sxc=cool&activate&eav=power&disable", 4, UseExpected);

        [TestMethod] public void MultipleIdenticalKeys() => TestPQS("id=27&id=48&id=72", 1, UseExpected);

    }
}
