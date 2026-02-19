using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Models;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcCoreOnly))]
public class LightSpeedDecoratorTests(DataBuilder dataBuilder)//: TestBaseEavCore
{
    //public LightSpeedDecoratorTests() => _testData = new(dataBuilder);

    private readonly LightSpeedTestData _testData = new(dataBuilder);


    [Fact]
    public void DecoratorWithNullEntity()
    {
        var lsDecorator = (null as IEntity).ToModel<LightSpeedDecorator>(nullHandling: ModelNullHandling.PreferModelForce);
        TestEmptyDecorator(lsDecorator);
        True(lsDecorator.UrlParametersOthersDisableCache);
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
        False(lsDecorator.IsEnabled);
        Equal("", lsDecorator.UrlParameterNames);
        False(lsDecorator.ByUrlParameters);
        False(lsDecorator.UrlParametersCaseSensitive);
        True(lsDecorator.UrlParametersOthersDisableCache);
    }

    [Fact]
    public void DecoratorWithByUrlParameter()
        => True(_testData.Decorator(byUrlParameters: true).ByUrlParameters);

    [Fact]
    public void DecoratorWithCaseSensitive()
        => True(_testData.Decorator(caseSensitive: true).UrlParametersCaseSensitive);

    [Fact]
    public void DecoratorWithNames()
        => Equal("a\nb\nc", _testData.Decorator(names: "a\nb\nc").UrlParameterNames);

    [Fact]
    public void DecoratorWithDisableCache()
        => False(_testData.Decorator(othersDisableCache: false).UrlParametersOthersDisableCache);

}