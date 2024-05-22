using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ToSic.Sxc.Services.Cache;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Tests.ServicesTests.CacheTests;

[TestClass]
public class CacheKeyTests
{
    internal const string FullDefaultPrefix = $"{DefaultPrefix}{Sep}App:0{Sep}{SegmentPrefix}{DefaultSegment}{Sep}";

    [DataRow(null)]
    [DataRow("")]
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MainKeyBad(string main)
        => AreEqual(FullDefaultPrefix + main, new CacheKeySpecs(0, main).Key);

    [DataRow(Sep + "App:0", 0, "zero")]
    [DataRow(Sep + "App:27", 27, "27")]
    [DataRow(Sep + "App:42", 42, "42")]
    [DataRow("", CacheKeySpecs.NoApp, "no app")]
    [TestMethod]
    public void MainKeyAppIds(string expectedReplace, int appId, string message)
        => AreEqual(FullDefaultPrefix.Replace(Sep + "App:0", expectedReplace) + "Test", new CacheKeySpecs(appId, "Test").Key, message);



    [DataRow("TestKey")]
    [DataRow("A")]
    [DataRow("A\nB")]
    [TestMethod]
    public void MainKeyOnly(string main)
        => AreEqual( FullDefaultPrefix + main, new CacheKeySpecs(0, main).Key);

    private const string FullSegmentPrefix = $"{DefaultPrefix}{Sep}App:0{Sep}{SegmentPrefix}";

    [DataRow($"{DefaultSegment}{Sep}Main", "Main", null)]
    [DataRow($"{DefaultSegment}{Sep}Main", "Main", "")]
    [DataRow($"MySegment{Sep}Main", "Main", "MySegment")]
    [TestMethod]
    public void MainAndSegment(string expected, string main, string segment)
        => AreEqual( FullSegmentPrefix + expected, new CacheKeySpecs(0, main, segment).Key);


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