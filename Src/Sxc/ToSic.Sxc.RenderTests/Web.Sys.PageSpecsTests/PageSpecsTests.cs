using ToSic.Sxc.Web.Sys.PageServiceShared;

namespace ToSic.Sxc.Web.Sys.PageSpecsTests;

public class PageSpecsTests
{
    private const string Id = "id";

    [Fact]
    public void InitiallyEmpty()
    {
        var c = new PageUrlSpecs();
        Empty(c.Specs);
        False(c.ContainsKey(Id));
        Null(c.ValuesCsv(Id));
    }

    [Fact]
    public void SetNull()
    {
        var c = new PageUrlSpecs();
        c.Set(Id);
        True(c.ContainsKey(Id));
        Null(c.ValuesCsv(Id));
    }

    [Fact]
    public void SetValue()
    {
        var c = new PageUrlSpecs();
        c.Set(Id, "42");
        True(c.ContainsKey(Id));
        Equal("42", c.ValuesCsv(Id));
    }

    [Fact]
    public void SetValues()
    {
        var c = new PageUrlSpecs();
        c.Set(Id, "param1,param2");
        True(c.ContainsKey(Id));
        Equal("param1,param2", c.ValuesCsv(Id));
    }

    [Theory]
    [InlineData("42", "42", "")]
    [InlineData("42", "42", null)]
    [InlineData("42,43", "42", "43")]
    [InlineData("42,43,44", "42", "43,44")]
    [InlineData("42,47,43,44", "42,47", "43,44")]
    public void SetAddExtends(string expected, string first, string add)
    {
        var c = new PageUrlSpecs();
        c.Set(Id, first);
        c.Add(Id, add);
        True(c.ContainsKey(Id));
        Equal(expected, c.ValuesCsv(Id));
    }

    [Fact]
    public void SetSetReplaces()
    {
        var c = new PageUrlSpecs();
        c.Set(Id, "42");
        c.Set(Id, "43");
        True(c.ContainsKey(Id));
        Equal("43", c.ValuesCsv(Id));
    }

    [Theory]
    [InlineData("", null, null)]
    [InlineData("", "", "")]
    [InlineData("", "", null)]
    [InlineData("", null, "")]
    [InlineData("param1", "param1", "param1")]
    [InlineData("param1", "param1", "")]
    [InlineData("param1", "", "param1")]
    [InlineData("param1", "param1", null)]
    [InlineData("param1,param2,param3", "param1,param2", "param2,param3")]
    [InlineData("param2,param1,param3", "param2,param1", "param2,param3")]
    public void SetManyKeys(string expected, string keys1, string keys2)
    {
        var c = new PageUrlSpecs();
        c.Set(keys1);
        c.Add(keys2);
        //True(c.ContainsKey(AllowedUrlParameters));
        Equal(expected, c.Keys());
    }
}