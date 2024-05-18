using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToSic.Sxc.Services.Cache;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Tests.ServicesTests.CacheTests;

[TestClass]
public class CacheKeyTests
{
    private const string FullDefaultPrefix = DefaultPrefix + Sep + SegmentPrefix + DefaultSegment + Sep;

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

}