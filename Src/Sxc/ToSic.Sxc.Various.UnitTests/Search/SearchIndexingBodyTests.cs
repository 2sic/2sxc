using ToSic.Eav;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Search.Sys;
using Xunit.DependencyInjection;

namespace ToSic.Sxc.Tests.Search;

[Startup(typeof(StartupTestsEavDataBuild))]
public class SearchIndexingBodyTests(IDataFactory dataFactory)
{
    [Fact]
    public void Body_IndexesPlainTextAndNumbers_AndExcludesOtherFieldTypes()
    {
        var entity = dataFactory.Create(new System.Collections.Generic.Dictionary<string, object?>
        {
            ["Text"] = "<p>A &amp; B</p>",
            ["Count"] = 42m,
            ["Enabled"] = true,
        });

        var body = SearchIndexing.Body(entity, "en-us");

        Contains("A & B", body);
        Contains("42", body);
        DoesNotContain("True", body);
        DoesNotContain("<p>", body);
    }
}
