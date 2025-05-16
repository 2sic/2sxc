using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests;


public class CspParametersTests
{
    [Fact]
    public void Empty()
    {
        var cspp = new CspParameters();
        Equal("", cspp.ToString());
    }


    [Fact]
    public void OnePair()
    {
        var cspp = new CspParameters();
        cspp.Add("test", "value");
        Equal("test value;", cspp.ToString());
    }
    [Fact]
    public void OnePairTwoValues()
    {
        var cspp = new CspParameters();
        cspp.Add("test", "value");
        cspp.Add("test", "value2");
        Equal("test value value2;", cspp.ToString());
    }

    [Fact]
    public void TwoPairs()
    {
        var cspp = new CspParameters();
        cspp.Add("test", "value");
        cspp.Add("test2", "value2");
        Equal("test value; test2 value2;", cspp.ToString());
    }
}