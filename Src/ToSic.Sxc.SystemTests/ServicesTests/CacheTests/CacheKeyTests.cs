using ToSic.Sxc.Services.Cache;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.ServicesTests.CacheTests;

public class CacheKeyTests
{
    internal const string FullDefaultPrefix = $"{DefaultPrefix}{Sep}App:0{Sep}{SegmentPrefix}{DefaultSegment}{Sep}";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    //[ExpectedException(typeof(ArgumentException))]
    public void MainKeyBad(string main)
        => Throws<ArgumentException>(() => Equal(FullDefaultPrefix + main, new CacheKeySpecs(0, main).Key));

    [Theory]
    [InlineData(Sep + "App:0", 0, "zero")]
    [InlineData(Sep + "App:27", 27, "27")]
    [InlineData(Sep + "App:42", 42, "42")]
    [InlineData("", CacheKeySpecs.NoApp, "no app")]
    public void MainKeyAppIds(string expectedReplace, int appId, string message)
        => Equal(FullDefaultPrefix.Replace(Sep + "App:0", expectedReplace) + "Test", new CacheKeySpecs(appId, "Test").Key);//, message);



    [InlineData("TestKey")]
    [InlineData("A")]
    [InlineData("A\nB")]
    [Theory]
    public void MainKeyOnly(string main)
        => Equal( FullDefaultPrefix + main, new CacheKeySpecs(0, main).Key);

    private const string FullSegmentPrefix = $"{DefaultPrefix}{Sep}App:0{Sep}{SegmentPrefix}";

    [InlineData($"{DefaultSegment}{Sep}Main", "Main", null)]
    [InlineData($"{DefaultSegment}{Sep}Main", "Main", "")]
    [InlineData($"MySegment{Sep}Main", "Main", "MySegment")]
    [Theory]
    public void MainAndSegment(string expected, string main, string segment)
        => Equal( FullSegmentPrefix + expected, new CacheKeySpecs(0, main, segment).Key);


    [Fact]
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
        Equal(expected, resultDic1);

        var dic2 = new Dictionary<string, string>
        {
            { "C", "CVal" },
            { "A", "AVal" },
            { "B", "BVal" }
        };
        var resultDic2 = CacheKeySpecs.GetVaryByOfDic(dic2);
        Equal(expected, resultDic2);
        Equal(resultDic1, resultDic2);

    }
}