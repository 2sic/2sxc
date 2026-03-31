using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Sys.PageServiceShared;
using Xunit.Abstractions;

namespace ToSic.Sxc.Web.Sys.PageSpecsTests;

public class PageSpecsComparison(ITestOutputHelper output)
{

    [Theory]
    [InlineData("id", "id", 0)]
    [InlineData("id", "ID", 1)]
    [InlineData("id", "unexpected", 1)]
    [InlineData("id,id2", "id", 0)]
    [InlineData("id,id2", "id2", 0)]
    [InlineData("id,id2", "id,id2", 0)]
    [InlineData("id,id2", "id2,id", 0)]
    [InlineData("id,id2", "id2,unexpected", 1)]
    public void CompareWithParameterKeys(string inSpecs, string inParams, int expected)
    {
        // Prepare Parameters
        IParameters pagePars = new Context.Sys.Parameters();
        pagePars = inParams.Split(',').Aggregate(pagePars, (current, s) => current.Add(s, 42));

        var specs = new PageUrlSpecs();
        specs.Add(inSpecs);

        var unexpected = specs.GetInvalid(pagePars);

        output.WriteLine($"allowed: '{specs.Keys()}'");

        Equal(expected, unexpected.Count());
    }

    [Theory]
    [InlineData("variant", "typed,dyn", "dyn", true)]
    [InlineData("variant", "typed,dyn", "typed", true)]
    [InlineData("variant", "typed,dyn", "TYPED", false)]
    [InlineData("variant", "typed,dyn", "unexpected", false)]
    public void CompareWithParameterValues(string inSpecs, string inParams, string valueToCheck, bool expected)
    {
        // Prepare Parameters
        var pagePars = new Context.Sys.Parameters()
            .Add("tut", "basics-linking")
            .Add(inSpecs, valueToCheck);

        var specs = new PageUrlSpecs();
        specs.Add("tut"); // allow all parameters with name "tut"
        specs.Add(inSpecs, inParams);
        var unexpected = specs.GetInvalid(pagePars);
        output.WriteLine($"allowed: '{specs.Keys()}'");
        if (expected)
            Empty(unexpected);
        else
            NotEmpty(unexpected);
    }
}
