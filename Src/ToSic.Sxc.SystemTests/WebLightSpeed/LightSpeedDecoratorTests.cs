using ToSic.Eav.Data.Build;
using ToSic.Sxc.Web.Internal.LightSpeed;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcCoreOnly))]
public class LightSpeedDecoratorTests(DataBuilder dataBuilder)//: TestBaseEavCore
{
    //public LightSpeedDecoratorTests() => _testData = new(dataBuilder);

    private readonly LightSpeedTestData _testData = new(dataBuilder);


    [Fact]
    public void DecoratorWithNullEntity()
    {
        var lsDecorator = new LightSpeedDecorator(null);
        TestEmptyDecorator(lsDecorator);
        Equal(true, lsDecorator.UrlParametersOthersDisableCache);
    }

    [Fact]
    public void DecoratorWithEntity()
    {
        var lsDecorator = _testData.Decorator();
        Equal(LightSpeedTestData.DefTitle, lsDecorator.Title);
        TestEmptyDecorator(lsDecorator);
    }

    private static void TestEmptyDecorator(LightSpeedDecorator lsDecorator)
    {
        Equal(false, lsDecorator.IsEnabled);
        Equal("", lsDecorator.UrlParameterNames);
        Equal(false, lsDecorator.ByUrlParameters);
        Equal(false, lsDecorator.UrlParametersCaseSensitive);
        Equal(true, lsDecorator.UrlParametersOthersDisableCache);
    }

    [Fact]
    public void DecoratorWithByUrlParameter()
        => Equal(true, _testData.Decorator(byUrlParameters: true).ByUrlParameters);

    [Fact]
    public void DecoratorWithCaseSensitive()
        => Equal(true, _testData.Decorator(caseSensitive: true).UrlParametersCaseSensitive);

    [Fact]
    public void DecoratorWithNames()
        => Equal("a\nb\nc", _testData.Decorator(names: "a\nb\nc").UrlParameterNames);

    [Fact]
    public void DecoratorWithDisableCache()
        => Equal(false, _testData.Decorator(othersDisableCache: false).UrlParametersOthersDisableCache);

}