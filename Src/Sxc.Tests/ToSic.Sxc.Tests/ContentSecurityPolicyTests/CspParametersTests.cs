﻿using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests;

[TestClass]
public class CspParametersTests
{
    [TestMethod]
    public void Empty()
    {
        var cspp = new CspParameters();
        AreEqual("", cspp.ToString());
    }


    [TestMethod]
    public void OnePair()
    {
        var cspp = new CspParameters();
        cspp.Add("test", "value");
        AreEqual("test value;", cspp.ToString());
    }
    [TestMethod]
    public void OnePairTwoValues()
    {
        var cspp = new CspParameters();
        cspp.Add("test", "value");
        cspp.Add("test", "value2");
        AreEqual("test value value2;", cspp.ToString());
    }

    [TestMethod]
    public void TwoPairs()
    {
        var cspp = new CspParameters();
        cspp.Add("test", "value");
        cspp.Add("test2", "value2");
        AreEqual("test value; test2 value2;", cspp.ToString());
    }
}