using static ToSic.Sxc.DataTests.GetAndConvertHelperTestAccessors;

namespace ToSic.Sxc.DataTests;


public class GetAndConvertHelperTests
{
    private static readonly List<string> MockSystemLanguages2 = ["en", "de"];
    private static readonly List<string> MockSystemLanguages4 = ["en-us", "de-ch", "fr-fr"];

    [Fact]
    public void GetFinalLanguagesList_LangsNull()
    {
        var dims = new[] { "en", "de", null };
        var languages = TacGetFinalLanguagesList(null, MockSystemLanguages2, dims);
        //Assert.True(skipAddDef);
        Equal(dims, languages);
    }

    [Fact]
    public void GetFinalLanguagesList_LangsEmpty()
    {
        var dims = new[] { "en", "de", null };
        var languages = TacGetFinalLanguagesList("", MockSystemLanguages2, dims);
        //Assert.True(skipAddDefault);
        Equal(new string[] { null }, languages);
    }

    [Fact]
    public void GetFinalLanguagesList_LangsFirst()
    {
        var dims = new[] { "en", "de", null };
        var languages = TacGetFinalLanguagesList("en", MockSystemLanguages2, dims);
        //Assert.True(skipAddDefault);
        Equal(new[] { "en" }, languages);
    }

    [Fact]
    public void GetFinalLanguagesList_LangsBoth()
    {
        var dims = new[] { "en", "de", null };
        var languages = TacGetFinalLanguagesList("en,de", MockSystemLanguages2, dims);
        //Assert.True(skipAddDefault);
        Equal(new[] { "en", "de" }, languages);
    }

    [Fact]
    public void GetFinalLanguagesList_LangsFirstAndEmpty()
    {
        var dims = new[] { "en", "de", null };
        var languages = TacGetFinalLanguagesList("en,", MockSystemLanguages2, dims);
        //Assert.True(skipAddDefault);
        Equal(new[] { "en", null }, languages);
    }

    [Fact]
    public void GetFinalLanguagesList_LangsFirstShortened()
    {
        var dims = new[] { "en-us", "de-CH", null };
        var languages = TacGetFinalLanguagesList("en", MockSystemLanguages4, dims);
        //Assert.True(skipAddDefault);
        Equal(new[] { "en-us" }, languages);
    }

    [Fact]
    public void GetFinalLanguagesList_LangsNotFound()
    {
        var dims = new[] { "en-us", "de-CH", null };
        var languages = TacGetFinalLanguagesList("qr", MockSystemLanguages4, dims);
        //Assert.True(skipAddDefault);
        Equal(Array.Empty<string>() , languages);
    }

}