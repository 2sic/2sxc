using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ToSic.Eav.Caching;
using ToSic.Sxc.Services.Cache;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Tests.ServicesTests.CacheTests;

[TestClass]
public class CacheKeyTests
{
    internal const string FullDefaultPrefix = DefaultPrefix + Sep + SegmentPrefix + DefaultSegment + Sep;

    [DataRow(null)]
    [DataRow("")]
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MainKeyBad(string main)
        => AreEqual(FullDefaultPrefix + main, new CacheKeySpecs(main).Key);

    [DataRow("TestKey")]
    [DataRow("A")]
    [DataRow("A\nB")]
    [TestMethod]
    public void MainKeyOnly(string main)
        => AreEqual( FullDefaultPrefix + main, new CacheKeySpecs(main).Key);

    private const string FullSegmentPrefix = DefaultPrefix + Sep + SegmentPrefix;

    [DataRow($"{DefaultSegment}{Sep}Main", "Main", null)]
    [DataRow($"{DefaultSegment}{Sep}Main", "Main", "")]
    [DataRow($"MySegment{Sep}Main", "Main", "MySegment")]
    [TestMethod]
    public void MainAndSegment(string expected, string main, string segment)
        => AreEqual( FullSegmentPrefix + expected, new CacheKeySpecs(main, segment).Key);


    [TestMethod]
    public void EnsureDicIsSorted()
    {
        var expected = $"{Sep}A=AVal{Sep}B=BVal{Sep}C=CVal";
        var dic = new Dictionary<string, string>
        {
            { "B", "BVal" },
            { "A", "AVal" },
            { "C", "CVal" }
        };
        var resultDic1 = CacheKeySpecs.GetVaryByOfDic(dic);
        AreEqual(expected, resultDic1);

        var dic2 = new Dictionary<string, string>
        {
            { "C", "CVal" },
            { "A", "AVal" },
            { "B", "BVal" }
        };
        var resultDic2 = CacheKeySpecs.GetVaryByOfDic(dic2);
        AreEqual(expected, resultDic2);
        AreEqual(resultDic1, resultDic2);

    }
}