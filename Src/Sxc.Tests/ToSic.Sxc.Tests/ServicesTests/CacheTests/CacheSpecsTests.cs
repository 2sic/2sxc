using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Tests.ServicesTests.CacheTests;

[TestClass]
public class CacheSpecsTests: TestBaseSxcDb
{
    private static readonly string MainPrefix = $"{CacheKeyTests.FullDefaultPrefix.Replace("App:0", "App:-1")}Main{Sep}";

    private ICacheSpecs GetForMain(string name = "Main") => GetService<ICacheService>().CreateSpecs(name);

    [TestMethod]
    public void ShareKeyAcrossApps()
    {
        var expected = $"{CacheKeyTests.FullDefaultPrefix.Replace(Sep + "App:0", "")}Main";
        var svc = GetService<ICacheService>();
        var specs = svc.CreateSpecs("Main", shared: true);
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom1CaseSensitive()
    {
        var expected = MainPrefix + "VaryByKey1=Value1";
        var svc = GetService<ICacheService>();
        var specs = svc.CreateSpecs("Main")
            .VaryBy("Key1", "Value1", caseSensitive: true);
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom1()
    {
        var expected = MainPrefix + "VaryByKey1=Value1".ToLowerInvariant();
        var specs = GetForMain()
            .VaryBy("Key1", "Value1");
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom2SameKey()
    {
        var expected = MainPrefix + "VaryByKey1=Value1".ToLowerInvariant();
        var specs = GetForMain()
            .VaryBy("Key1", "valueWhichWillGoAway")
            .VaryBy("Key1", "Value1");
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom2DiffKey()
    {
        var expected = MainPrefix + $"VaryByKey1=Val1{Sep}VaryByKey2=Value2".ToLowerInvariant();
        var specs = GetForMain()
            .VaryBy("Key1", "Val1")
            .VaryBy("Key2", "Value2");
        AreEqual(expected, specs.Key);
    }

    // Disabled for now, not sure if this single-value case is needed
    //[TestMethod]
    //public void VaryByCustom1ValueOnly()
    //{
    //    var expected = MainPrefix + "VaryByThisIsTheValue=".ToLowerInvariant();
    //    var specs = GetForMain()
    //        .VaryBy("ThisIsTheValue");
    //    AreEqual(expected, specs.Key);
    //}

    [TestMethod]
    public void VaryByParameters()
    {
        var expected = MainPrefix + "VaryByParameters=".ToLowerInvariant();
        var pars = new Parameters();
        var specs = GetForMain().VaryByParameters(pars);
        AreEqual(expected, specs.Key);
    }

    [DataRow(null, "no names specified")]
    [DataRow("", "empty names specified")]
    [DataRow("A,B,C", "too many names")]
    [DataRow("A", "names with exact casing")]
    [DataRow("a", "names with different casing")]
    [TestMethod]
    public void VaryByParametersOneNamed(string names, string testName)
    {
        var expected = MainPrefix + "VaryByParameters=A=AVal".ToLowerInvariant();
        var pars = new Parameters
        {
            Nvc = new()
            {
                { "A", "AVal" }
            }
        };
        var specs = GetForMain().VaryByParameters(pars, names: names);
        AreEqual(expected, specs.Key, testName);
    }


    [TestMethod]
    public void VaryByParametersWithNamesAll()
    {
        var expected = MainPrefix + "VaryByParameters=A=AVal&B=BVal&C=CVal".ToLowerInvariant();
        var pars = new Parameters
        {
            Nvc = new()
            {
                { "A", "AVal" },
                { "B", "BVal" },
                { "C", "CVal" }
            }
        };
        var specs = GetForMain().VaryByParameters(pars, names: "A,B,c");
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByParametersWithNamesSome()
    {
        var expected = MainPrefix + "VaryByParameters=A=AVal&C=CVal".ToLowerInvariant();
        var pars = new Parameters
        {
            Nvc = new()
            {
                { "A", "AVal" },
                { "B", "BVal" },
                { "C", "CVal" }
            }
        };
        var specs = GetForMain().VaryByParameters(pars, names: "A,c");
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByParametersBlankSameAsNone()
    {
        var expected = MainPrefix + "VaryByParameters=A=AVal&C=CVal".ToLowerInvariant();
        var pars = new Parameters
        {
            Nvc = new()
            {
                { "A", "AVal" },
                { "B", "" },
                { "C", "CVal" }
            }
        };
        var specsFiltered = GetForMain().VaryByParameters(pars, names: "A,c");
        var specsAll = GetForMain().VaryByParameters(pars);
        AreEqual(expected, specsFiltered.Key);
        AreEqual(specsFiltered.Key, specsAll.Key);
    }

}