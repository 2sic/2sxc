using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Tests.WebUrlTests;


public class UrlValueFilterTests
{
    private UrlValueFilterNames TestFilter(bool defaultSerialize, IEnumerable<string> opposite) =>
        new(defaultSerialize, opposite);

    [Fact]
    public void NoFilterKeepAll()
    {
        var filter = TestFilter(true, new List<string>());
        var result = filter.Process(new("something", "value"));
        True(result.Keep);
    }

    [Fact]
    public void NoFilterKeepNone()
    {
        var filter = TestFilter(false, new List<string>());
        var result = filter.Process(new("something", "value"));
        False(result.Keep);
    }

    [Fact]
    public void FilterSomeKeepRest()
    {
        var filter = TestFilter(true, ["drop"]);
        True(filter.Process(new("something", "value")).Keep);
        True(filter.Process(new("something2", "value")).Keep);
        True(filter.Process(new("drop2", "value")).Keep);
        False(filter.Process(new("drop", "value")).Keep, "this is the only one it should drop");
        False(filter.Process(new("Drop", "value")).Keep, "this should also fail, case insensitive");
    }

    [Fact]
    public void FilterSomeDropRest()
    {
        var filter = TestFilter(false, ["keep"]);
        False(filter.Process(new("something", "value")).Keep);
        False(filter.Process(new("something2", "value")).Keep);
        False(filter.Process(new("Drop", "value")).Keep);
        False(filter.Process(new("drop2", "value")).Keep);
        True(filter.Process(new("keep", "value")).Keep, "this is th only one it should keep");
    }
}