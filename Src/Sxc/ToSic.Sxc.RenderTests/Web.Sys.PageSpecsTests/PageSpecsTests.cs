using ToSic.Sxc.Web.Sys.PageServiceShared;

namespace ToSic.Sxc.Web.Sys.PageSpecsTests;

public class PageSpecsTests
{
    [Fact]
    public void InitiallyEmpty()
    {
        var c = new PageSpecs();
        Empty(c.Collected);
        False(c.ContainsKey(PageSpecs.AllowedUrlParameters));
        Null(c.Get(PageSpecs.AllowedUrlParameters));
    }

    [Fact]
    public void CollectAddsKeyValue()
    {
        var c = new PageSpecs();
        c.Set(PageSpecs.AllowedUrlParameters, "param1,param2");
        True(c.ContainsKey(PageSpecs.AllowedUrlParameters));
        Equal("param1,param2", c.Get(PageSpecs.AllowedUrlParameters));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("", "", "")]
    [InlineData("", "", null)]
    [InlineData(null, null, "")]
    [InlineData("param1", "param1", "param1")]
    [InlineData("param1", "param1", "")]
    [InlineData("param1", "", "param1")]
    [InlineData("param1", "param1", null)]
    [InlineData("param1,param2,param3", "param1,param2", "param2,param3")]
    [InlineData("param2,param1,param3", "param2,param1", "param2,param3")]
    public void AddCsvMergesValues(string expected, string set1, string set2)
    {
        var c = new PageSpecs();
        c.Set(PageSpecs.AllowedUrlParameters, set1);
        c.AddCsv(PageSpecs.AllowedUrlParameters, set2);
        //True(c.ContainsKey(AllowedUrlParameters));
        Equal(expected, c.Get(PageSpecs.AllowedUrlParameters));
    }
}