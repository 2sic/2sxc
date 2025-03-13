﻿using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Tests.ServicesTests.CacheTests;

[TestClass]
public class CacheSpecsTests: TestBaseSxcDb
{
    private static readonly string MainPrefix = $"{CacheKeyTests.FullDefaultPrefix.Replace("App:0", "App:-1")}Main{Sep}";

    private ICacheSpecs GetForMain(string name = "Main") =>
        GetService<ICacheService>().CreateSpecsTac(name);

    [TestMethod]
    public void ShareKeyAcrossApps()
    {
        var expected = $"{CacheKeyTests.FullDefaultPrefix.Replace(Sep + "App:0", "")}Main";
        var svc = GetService<ICacheService>();
        var specs = svc.CreateSpecsTac("Main", shared: true);
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom1CaseSensitive()
    {
        var expected = MainPrefix + "VaryByKey1=Value1";
        var svc = GetService<ICacheService>();
        var specs = svc.CreateSpecsTac("Main")
            .VaryByTac("Key1", "Value1", caseSensitive: true);
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom1()
    {
        var expected = MainPrefix + "VaryByKey1=Value1".ToLowerInvariant();
        var specs = GetForMain()
            .VaryByTac("Key1", "Value1");
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom2SameKey()
    {
        var expected = MainPrefix + "VaryByKey1=Value1".ToLowerInvariant();
        var specs = GetForMain()
            .VaryByTac("Key1", "valueWhichWillGoAway")
            .VaryByTac("Key1", "Value1");
        AreEqual(expected, specs.Key);
    }

    [TestMethod]
    public void VaryByCustom2DiffKey()
    {
        var expected = MainPrefix + $"VaryByKey1=Val1{Sep}VaryByKey2=Value2".ToLowerInvariant();
        var specs = GetForMain()
            .VaryByTac("Key1", "Val1")
            .VaryByTac("Key2", "Value2");
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
        var specs = GetForMain().VaryByParametersTac(pars);
        AreEqual(expected, specs.Key);
    }

    [DataRow(null, "no names specified, use all")]
    [DataRow("", "empty names specified, use none", "")]
    [DataRow("A,B,C", "too many names")]
    [DataRow("A", "names with exact casing")]
    [DataRow("a", "names with different casing")]
    [TestMethod]
    public void VaryByParametersOneNamed(string names, string testName, string specialExpected = default)
    {
        var expected = MainPrefix + ("VaryByParameters=" + (specialExpected ?? "A=AVal")).ToLowerInvariant();
        var pars = new Parameters
        {
            Nvc = new()
            {
                { "A", "AVal" }
            }
        };
        var specs = GetForMain().VaryByParametersTac(pars, names: names);
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
        var specs = GetForMain().VaryByParametersTac(pars, names: "A,B,c");
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
        var specs = GetForMain().VaryByParametersTac(pars, names: "A,c");
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
        var specsFiltered = GetForMain().VaryByParametersTac(pars, names: "A,c");
        var specsAll = GetForMain().VaryByParametersTac(pars);
        AreEqual(expected, specsFiltered.Key);
        AreEqual(specsFiltered.Key, specsAll.Key);
    }

    [TestMethod]
    public void TestCacheKeys()
    {
        var key = "TestKey";
        var previousKey = new CacheKeySpecs(-1, key).Key;

        var spec = GetForMain(key);
        AreEqual(previousKey, spec.Key);
    }
}