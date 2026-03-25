using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Sys.PageServiceShared;
using Xunit.Abstractions;

namespace ToSic.Sxc.Web.Sys.PageSpecsTests;

public class PageSpecsComparison(ITestOutputHelper output)
{

    [Theory]
    [InlineData("id", "id", 0)]
    [InlineData("id", "unexpected", 1)]
    [InlineData("id,id2", "id", 0)]
    [InlineData("id,id2", "id2", 0)]
    [InlineData("id,id2", "id,id2", 0)]
    [InlineData("id,id2", "id2,id", 0)]
    [InlineData("id,id2", "id2,unexpected", 1)]
    public void CompareWithParameters(string inSpecs, string inParams, int expected)
    {
        IParameters pagePars = new Context.Sys.Parameters();
        pagePars = inParams.Split(',').Aggregate(pagePars, (current, s) => current.Add(s, 42));

        var specs = new PageSpecs();
        specs.AddCsv(PageSpecs.AllowedUrlParameters, inSpecs);

        var unexpected = specs.GetUnexpected(PageSpecs.AllowedUrlParameters, pagePars);

        output.WriteLine("allowed: '" + specs.Get(PageSpecs.AllowedUrlParameters) + "'");

        Equal(expected, unexpected.Count());
    }
}
