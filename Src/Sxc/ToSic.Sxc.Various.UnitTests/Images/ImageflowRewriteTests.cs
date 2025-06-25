using System.Collections.Specialized;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Tests.Images;


public class ImageflowRewriteTests
{
    #region shared

    public static readonly string Quality = ImageflowRewrite.Quality;
    public static readonly string PngQuality = ImageflowRewrite.PngQuality;
    public static readonly string WebpQuality = ImageflowRewrite.WebpQuality;

    private static NameValueCollection AddKeyWhenMissing(NameValueCollection queryString, string key, string value) =>
        ImageflowRewrite.AddKeyWhenMissing(queryString, key, value);

    private NameValueCollection QueryStringRewrite(NameValueCollection queryString) => 
        ImageflowRewrite.QueryStringRewrite(queryString);

    private static void AreEquivalentAlsoByValues(NameValueCollection expected, NameValueCollection actual) =>
        Equivalent(
            expected.AllKeys.ToDictionary(k => k, k => expected[k]),
            actual.AllKeys.ToDictionary(k => k, k => actual[k]));

    #endregion


    #region QueryStringRewrite MAIN TESTING

    [Fact]
    public void QueryStringRewriteWhenQueryStringNull()
    {
        Null(QueryStringRewrite(null));
    }


    [Fact]
    public void QueryStringRewriteWhenQueryStringEmpty()
    {
        var actual = QueryStringRewrite(new());
        var expected = new NameValueCollection ();
        AreEquivalentAlsoByValues(expected, actual);
    }


    [Fact]
    public void QueryStringRewriteWhenQueryStringWithOneItem()
    {
        var actual = QueryStringRewrite(new() { { "1", "1" } });
        var expected = new NameValueCollection
        {
            { "1", "1" }
        };
        AreEquivalentAlsoByValues(expected, actual);
    }


    [Fact]
    public void QueryStringRewriteWhenQueryStringWithManyItems()
    {
        var actual = QueryStringRewrite(
            new()
            {
                { "1", "1" },
                { "2", "2" }
            });
        var expected = new NameValueCollection
        {
            { "1", "1" },
            { "2", "2" },
        };
        AreEquivalentAlsoByValues(expected, actual);
    }


    [Fact]
    public void QualityQueryStringRewrite()
    {
        var actual = QueryStringRewrite(new() { { Quality, "value" } });
        var expected = new NameValueCollection
        {
            { Quality, "value" },
            { PngQuality, "value" },
            { WebpQuality, "value" },

        };
        AreEquivalentAlsoByValues(expected, actual);
    }


    [Fact]
    public void QualityQueryStringRewriteWhenManyItems()
    {
        var actual = QueryStringRewrite(
            new()
            {
                { "1", "1" },
                { "2", "2" },
                { Quality, "value" }
            });
        var expected = new NameValueCollection
        {
            { "1", "1" },
            { "2", "2" },
            { Quality, "value" },
            { PngQuality, "value" },
            { WebpQuality, "value" },
        };
        AreEquivalentAlsoByValues(expected, actual);
    }


    [Fact]
    public void QualityQueryStringRewriteWhenQueryStringWithManyQualityItems()
    {
        var actual = QueryStringRewrite(new()
        {
            { Quality, "value" },
            { PngQuality, "69" },
            { WebpQuality, "59" }
        });
        var expected = new NameValueCollection
        {
            { Quality, "value" },
            { PngQuality, "69" },
            { WebpQuality, "59" },
        };
        AreEquivalentAlsoByValues(expected, actual);
    }


    #endregion


    #region AddKeyWhenMissing function testing

    [Fact]
    public void AddKeyWhenMissingInQueryStringEmpty()
    {
        var actual = AddKeyWhenMissing(new(), "key", "value");
        var expected = new NameValueCollection { { "key", "value" } };
        AreEquivalentAlsoByValues(expected, actual);
    }

    [Fact]
    public void AddKeyWhenNotMissingInQueryStringOneElement()
    {
        var actual = AddKeyWhenMissing(
            new() { { "key", "value" } },
            "key", "value");
        var expected = new NameValueCollection { { "key", "value" } };
        AreEquivalentAlsoByValues(expected, actual);
    }

    [Fact]
    public void AddKeyWhenMissingInQueryStringOneElement()
    {
        var actual = AddKeyWhenMissing(
            new() { { "1", "1" } },
            "key", "value");
        var expected = new NameValueCollection { { "1", "1" }, { "key", "value" } };
        AreEquivalentAlsoByValues(expected, actual);
    }

    [Fact]
    public void AddKeyWhenNotMissingInQueryStringManyElement()
    {
        var actual = AddKeyWhenMissing(
            new() { { "1", "1" }, { "key", "value" } },
            "key", "value");
        var expected = new NameValueCollection { { "1", "1" }, { "key", "value" } };
        AreEquivalentAlsoByValues(expected, actual);
    }

    [Fact]
    public void AddKeyWhenMissingInQueryStringManyElement()
    {
        var actual = AddKeyWhenMissing(
            new() { { "1", "1" }, { "2", "2" } },
            "key", "value");
        var expected = new NameValueCollection { { "1", "1" }, { "2", "2" } , { "key", "value" } };
        AreEquivalentAlsoByValues(expected, actual);
    }

    #endregion

}