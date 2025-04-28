using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.ServicesTests.CacheTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class CacheSpecsTests(ICacheService svc)
{
    private static readonly string MainPrefix = $"{CacheKeyTests.FullDefaultPrefix.Replace("App:0", "App:-1")}Main{Sep}";

    private ICacheSpecs GetForMain(string name = "Main") =>
        svc.CreateSpecsTac(name);

    [Fact]
    public void ShareKeyAcrossApps()
    {
        var expected = $"{CacheKeyTests.FullDefaultPrefix.Replace(Sep + "App:0", "")}Main";
        var specs = svc.CreateSpecsTac("Main", shared: true);
        Equal(expected, specs.Key);
    }

    [Fact]
    public void VaryByCustom1CaseSensitive()
    {
        var expected = MainPrefix + "VaryByKey1=Value1";
        var specs = svc.CreateSpecsTac("Main")
            .VaryByTac("Key1", "Value1", caseSensitive: true);
        Equal(expected, specs.Key);
    }

    [Fact]
    public void VaryByCustom1()
    {
        var expected = MainPrefix + "VaryByKey1=Value1".ToLowerInvariant();
        var specs = GetForMain()
            .VaryByTac("Key1", "Value1");
        Equal(expected, specs.Key);
    }

    [Fact]
    public void VaryByCustom2SameKey()
    {
        var expected = MainPrefix + "VaryByKey1=Value1".ToLowerInvariant();
        var specs = GetForMain()
            .VaryByTac("Key1", "valueWhichWillGoAway")
            .VaryByTac("Key1", "Value1");
        Equal(expected, specs.Key);
    }

    [Fact]
    public void VaryByCustom2DiffKey()
    {
        var expected = MainPrefix + $"VaryByKey1=Val1{Sep}VaryByKey2=Value2".ToLowerInvariant();
        var specs = GetForMain()
            .VaryByTac("Key1", "Val1")
            .VaryByTac("Key2", "Value2");
        Equal(expected, specs.Key);
    }

    // Disabled for now, not sure if this single-value case is needed
    //[Fact]
    //public void VaryByCustom1ValueOnly()
    //{
    //    var expected = MainPrefix + "VaryByThisIsTheValue=".ToLowerInvariant();
    //    var specs = GetForMain()
    //        .VaryBy("ThisIsTheValue");
    //    Equal(expected, specs.Key);
    //}

    [Fact]
    public void VaryByParameters()
    {
        var expected = MainPrefix + "VaryByParameters=".ToLowerInvariant();
        var pars = new Parameters();
        var specs = GetForMain().VaryByParametersTac(pars);
        Equal(expected, specs.Key);
    }

    [Theory]
    [InlineData(null, "no names specified, use all")]
    [InlineData("", "empty names specified, use none", "")]
    [InlineData("A,B,C", "too many names")]
    [InlineData("A", "names with exact casing")]
    [InlineData("a", "names with different casing")]
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
        Equal(expected, specs.Key);//, testName);
    }


    [Fact]
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
        Equal(expected, specs.Key);
    }

    [Fact]
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
        Equal(expected, specs.Key);
    }

    [Fact]
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
        Equal(expected, specsFiltered.Key);
        Equal(specsFiltered.Key, specsAll.Key);
    }

    [Fact]
    public void TestCacheKeys()
    {
        var key = "TestKey";
        var previousKey = new CacheKeySpecs(-1, key).Key;

        var spec = GetForMain(key);
        Equal(previousKey, spec.Key);
    }
}