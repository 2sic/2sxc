namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

// 2022-03-15 2dm disabled - factor maps don't work like this, will probably stick to json format
//[TestClass]
//public class FactorMapTests
//{
//    [DataRow(null, "null")]
//    [DataRow("", "empty string")]
//    [DataRow("   ", "spaces")]
//    [DataRow("   \n  ", "spaces and new lines")]
//    [DataRow("   \r\n  ", "spaces and new lines")]
//    [DataRow("0.5", "factor, no value")]
//    [DataRow("=500", "value, no factor")]
//    [DataRow("0.5:500", "bad separator")]
//    [DataRow("x=500", "invalid factor")]
//    [DataRow("1=2=600", "multi :")]
//    [DataTestMethod]

//    public void EmptyAndInvalidMaps(string original, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.IsNotNull(fm);
//        Assert.AreEqual(0, fm.Length, name);
//    }


//    [DataRow("0.5=600", "double")]
//    [DataRow("1/2=600", "/")]
//    [DataRow("1:2=600", ":")]
//    [DataRow(" 1 / 2=600", "/ with spaces")]
//    [DataTestMethod]
//    public void Simple(string original, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.AreEqual(1, fm.Length, name);
//        AssertFandW(0.5, 600, fm[0], name);
//    }

//    [DataRow("x\n0.5=600", "double")]
//    [DataRow("0.5=600\nx=500", "double")]
//    [DataRow("1/2=600\n=500", "/")]
//    [DataRow(" 1 / 2=600\n0.5", "/ with spaces")]
//    [DataRow("x\n0.5\n1\n:500\n0.5=600\n22", "double")]
//    [DataTestMethod]
//    public void OneGoodRestBad(string original, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.AreEqual(1, fm.Length, name);
//        AssertFandW(0.5, 600, fm[0], name);
//    }

//    private static void AssertFandW(double expectedF, int expectedW, FactorRule fm, string name)
//    {
//        Assert.AreEqual(expectedF, fm.Factor, name);
//        Assert.AreEqual(expectedW, fm.Width, name);
//    }


//    [DataRow("0.5=600;1x,2x", "1x,2x", "double")]
//    [DataRow("1/2=600", "", "/")]
//    [DataRow("1:2=600", "", ":")]
//    [DataRow(" 1 / 2=600", "", "/ with spaces")]
//    [DataTestMethod]
//    public void WithSrcMap(string original, string srcSet, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.AreEqual(1, fm.Length, name);
//        AssertFandW(0.5, 600, fm[0], name);
//        Assert.AreEqual(srcSet, fm[0].SrcSet, name + " srcSet");
//    }
//}