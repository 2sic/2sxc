using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Tests.WebUrlTests;


public class UrlValueFilterTests
{
    private static UrlValueFilterNames TestFilter(bool defaultSerialize, IEnumerable<string> opposite) =>
        new(defaultSerialize, opposite);

    [Fact]
    public void NoFilterKeepAll()
    {
        var filter = TestFilter(true, new List<string>());
        var result = filter.Process(new("something", "value"));
        NotNull(result);
    }

    [Fact]
    public void NoFilterKeepNone()
    {
        var filter = TestFilter(false, new List<string>());
        var result = filter.Process(new("something", "value"));
        Null(result);
    }

    [Fact]
    public void FilterSomeKeepRest()
    {
        var filter = TestFilter(true, ["drop"]);
        NotNull(filter.Process(new("something", "value")));
        NotNull(filter.Process(new("something2", "value")));
        NotNull(filter.Process(new("drop2", "value")));
        Null(filter.Process(new("drop", "value"))); //, "this is the only one it should drop");
        Null(filter.Process(new("Drop", "value"))); //, "this should also fail, case insensitive");
    }

    [Fact]
    public void FilterSomeDropRest()
    {
        var filter = TestFilter(false, ["keep"]);
        Null(filter.Process(new("something", "value")));
        Null(filter.Process(new("something2", "value")));
        Null(filter.Process(new("Drop", "value")));
        Null(filter.Process(new("drop2", "value")));
        NotNull(filter.Process(new("keep", "value"))); //, "this is the only one it should keep");
    }
}