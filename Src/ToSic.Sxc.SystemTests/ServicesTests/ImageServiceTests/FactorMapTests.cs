namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

// 2022-03-15 2dm disabled - factor maps don't work like this, will probably stick to json format
//[TestClass]
//public class FactorMapTests
//{
//    [InlineData(null, "null")]
//    [InlineData("", "empty string")]
//    [InlineData("   ", "spaces")]
//    [InlineData("   \n  ", "spaces and new lines")]
//    [InlineData("   \r\n  ", "spaces and new lines")]
//    [InlineData("0.5", "factor, no value")]
//    [InlineData("=500", "value, no factor")]
//    [InlineData("0.5:500", "bad separator")]
//    [InlineData("x=500", "invalid factor")]
//    [InlineData("1=2=600", "multi :")]
//    [Theory]

//    public void EmptyAndInvalidMaps(string original, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.NotNull(fm);
//        Assert.Equal(0, fm.Length, name);
//    }


//    [InlineData("0.5=600", "double")]
//    [InlineData("1/2=600", "/")]
//    [InlineData("1:2=600", ":")]
//    [InlineData(" 1 / 2=600", "/ with spaces")]
//    [Theory]
//    public void Simple(string original, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.Equal(1, fm.Length, name);
//        AssertFandW(0.5, 600, fm[0], name);
//    }

//    [InlineData("x\n0.5=600", "double")]
//    [InlineData("0.5=600\nx=500", "double")]
//    [InlineData("1/2=600\n=500", "/")]
//    [InlineData(" 1 / 2=600\n0.5", "/ with spaces")]
//    [InlineData("x\n0.5\n1\n:500\n0.5=600\n22", "double")]
//    [Theory]
//    public void OneGoodRestBad(string original, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.Equal(1, fm.Length, name);
//        AssertFandW(0.5, 600, fm[0], name);
//    }

//    private static void AssertFandW(double expectedF, int expectedW, FactorRule fm, string name)
//    {
//        Assert.Equal(expectedF, fm.Factor, name);
//        Assert.Equal(expectedW, fm.Width, name);
//    }


//    [InlineData("0.5=600;1x,2x", "1x,2x", "double")]
//    [InlineData("1/2=600", "", "/")]
//    [InlineData("1:2=600", "", ":")]
//    [InlineData(" 1 / 2=600", "", "/ with spaces")]
//    [Theory]
//    public void WithSrcMap(string original, string srcSet, string name)
//    {
//        var fm = FactorMapHelper.CreateFromString(original);
//        Assert.Equal(1, fm.Length, name);
//        AssertFandW(0.5, 600, fm[0], name);
//        Assert.Equal(srcSet, fm[0].SrcSet, name + " srcSet");
//    }
//}