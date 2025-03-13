using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests;


public class CspParameterFinalizerTests
{
    private readonly CspParameterFinalizer _finalizer = new();//null);

    [Fact]
    public void NothingHasNoDefault()
    {
        var cspp = new CspParameters();
        cspp = _finalizer.MergedWithAll(cspp);
        Equal("", cspp.ToString());
    }

    [Fact]
    public void AllEmptyDoesNotInitializeDefault()
    {
        var cspp = new CspParameters
        {
            { CspConstants.AllSrcName, "" }
        };
        cspp = _finalizer.MergedWithAll(cspp);
        Equal("", cspp.ToString());
    }

    [Fact]
    public void AllInitializesDefault()
    {
        var cspp = new CspParameters
        {
            { CspConstants.AllSrcName, "'self'" }
        };
        cspp = _finalizer.MergedWithAll(cspp);
        Equal("default-src 'self';", cspp.ToString());
    }

    [Fact]
    public void AllExtendsDefault()
    {
        var cspp = new CspParameters
        {
            { CspConstants.DefaultSrcName, "'none'"},
            { CspConstants.AllSrcName, "'self'" }
        };
        cspp = _finalizer.MergedWithAll(cspp);
        Equal("default-src 'none' 'self';", cspp.ToString());
    }

    [Fact]
    public void FinalizeWithoutDuplicates()
    {
        var cspp = new CspParameters
        {
            { CspConstants.DefaultSrcName, "'none'"},
            { CspConstants.DefaultSrcName, "'self'"},
            { CspConstants.AllSrcName, "'self'" }
        };
        cspp = _finalizer.Finalize(cspp);
        Equal("default-src 'none' 'self';", cspp.ToString());
    }

    [Fact]
    public void DeduplicateEmpty()
    {
        var cspp = new CspParameters();
        cspp = _finalizer.DeduplicateValues(cspp);
        Equal("", cspp.ToString());
    }

    [Fact]
    public void DeduplicateNoDuplicates()
    {
        var cspp = new CspParameters
        {
            { CspConstants.AllSrcName, "test" },
            { CspConstants.AllSrcName, "test2" },
        };
        cspp = _finalizer.DeduplicateValues(cspp);
        Equal("all-src test test2;", cspp.ToString());
    }

    [Fact]
    public void DeduplicateDuplicates()
    {
        var cspp = new CspParameters
        {
            { CspConstants.AllSrcName, "test" },
            { CspConstants.AllSrcName, "test" },
        };
        cspp = _finalizer.DeduplicateValues(cspp);
        Equal("all-src test;", cspp.ToString());
    }


}