using System.Collections.Specialized;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests;


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
        Equal(expCount, result.Count);
        var reStringed = NewParameters(result).ToString();
        Equal(expected, reStringed);
    }

    [Fact] public void Null() => TestPQS("", 0, null);
    [Fact] public void Empty() => TestPQS("", 0, string.Empty);
    [Fact] public void Spaces() => TestPQS("", 0, "   ");

    [Fact] public void BasicPair() => TestPQS("2sxc=cool", 1, UseExpected);
    [Fact] public void BasicPairWithSpaces() => TestPQS("2sxc=cool", 1, " 2sxc=cool  ");

    // Special prefixes and suffixes
    [Fact] public void BasicPairWithQuestionPrefix() => TestPQS("2sxc=cool", 1, "?2sxc=cool");
    [Fact] public void BasicPairWithAndPrefix() => TestPQS("2sxc=cool", 1, "&2sxc=cool");
    [Fact] public void BasicPairWithAndSuffix() => TestPQS("2sxc=cool", 1, "&2sxc=cool&");


    [Fact] public void TwoPairs() => TestPQS("2sxc=cool&eav=power", 2, UseExpected);
    [Fact] public void SingleValue() => TestPQS("2sxc", 1, UseExpected);
    [Fact] public void SingleValues() => TestPQS("2sxc&eav&test", 3, UseExpected);
    [Fact] public void PairWithSingleValue() => TestPQS("2sxc=cool&activate", 2, UseExpected);
    [Fact] public void PairsWithSingleValues() => TestPQS("2sxc=cool&activate&disable&eav=power", 4, UseExpected);

    [Fact] public void MultipleIdenticalKeys() => TestPQS("id=27&id=48&id=72", 1, UseExpected);

}