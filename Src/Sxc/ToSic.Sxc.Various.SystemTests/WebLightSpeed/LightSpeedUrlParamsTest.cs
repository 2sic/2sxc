using System.Collections.Specialized;
using System.Diagnostics;
using ToSic.Eav.Data.Build;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sxc.Web.Sys.Url;

#pragma warning disable xUnit1026

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcCoreOnly))]
public class LightSpeedUrlParamsTest(DataBuilder dataBuilder)//: TestBaseEavCore
{
    //public LightSpeedUrlParamsTest() => _testData = new(dataBuilder);

    private readonly LightSpeedTestData _testData = new(dataBuilder);

    private static NameValueCollection Parse(string query) => UrlHelpers.ParseQueryString(query);

    internal static (bool CachingAllowed, string Extension) GetUrlParamsTac(LightSpeedDecorator lsConfig,
        string pageParameters, ILog? log = null, bool usePiggyBack = true)
        => GetUrlParamsTac(lsConfig, new Parameters { Nvc = Parse(pageParameters) }, log, usePiggyBack);

    internal static (bool CachingAllowed, string Extension) GetUrlParamsTac(LightSpeedDecorator lsConfig,
        IParameters? pageParameters = null, ILog? log = null, bool usePiggyBack = true)
        => LightSpeedUrlParams.GetUrlParams(lsConfig, pageParameters ?? new Parameters(), log, usePiggyBack);

    [Theory]
    [InlineData(true, null, "no setting")]
    [InlineData(true, true, "true")]
    [InlineData(false, false, "false")]
    public void IsEnabled(bool expected, bool? isEnabled, string message)
    {
        var lsDecorator = _testData.Decorator(isEnabled: isEnabled);
        var result = GetUrlParamsTac(lsDecorator);
        Equal(expected, result.CachingAllowed);//, message);
        Equal("", result.Extension);
    }

    [Theory]
    [InlineData(true, "", null, null, "", "blank, enabled/byUrl not defined")]
    [InlineData(true, "", true, true, "", "blank, enabled/byUrl true")]
    [InlineData(true, "", true, false, "", "blank, enabled, but not byUrl")]
    [InlineData(true, "", true, false, "a=b")]
    [InlineData(true, "a=b", true, true, "a=b")]
    [InlineData(true, "a=b&test=y", true, true, "test=y&a=b")]//, DisplayName = "Ensure parameters are sorted")]
    [InlineData(true, "a=b&test=y&zeta=beta", true, true, "zeta=beta&test=y&a=b")]//, DisplayName = "Ensure parameters are sorted")]
    [InlineData(true, "a=alpha&a=beta", true, true, "a=beta&a=alpha")]//, DisplayName = "Ensure parameter values are sorted")]
    [InlineData(false, "", false, null, "")]
    public void ByUrlParameters(bool expected, string expValue, bool? isEnabled, bool? byUrlParameters, string urlParameters, string? message = default)
    {
        var lsDecorator = _testData.Decorator(isEnabled: isEnabled, byUrlParameters: byUrlParameters, othersDisableCache: false);
        var result = GetUrlParamsTac(lsDecorator, urlParameters);
        Equal(expected, result.CachingAllowed);//, message);//, "caching allowed");
        Equal(expValue, result.Extension);//, "strings match");
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("", "a", "")]
    [InlineData("a=b", "a", "a=b")]
    [InlineData("a=b&b=c", "a,b", "a=b&b=c")]
    [InlineData("a=b", "a,b", "a=b")]//, DisplayName = "more possible, but only some given")]
    [InlineData("a=b", "*", "a=b")]
    [InlineData("a=b&c=d", "*", "a=b&c=d")]
    public void NamesShouldFilterResults(string expValue, string names, string urlParameters, string? message = default)
    {
        var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: names, othersDisableCache: true);
        var result = GetUrlParamsTac(lsDecorator, urlParameters);
        True(result.CachingAllowed);//, message);
        Equal(expValue, result.Extension);
    }

    [Theory]
    [InlineData(true, "", "")]//, DisplayName = "no names, params")]
    [InlineData(false, "", "a=b")]//, DisplayName = "has params but none expected")]
    [InlineData(true, "a", "a=b")]//, DisplayName = "Has params, expected")]
    [InlineData(false, "b", "a=b")]//, DisplayName = "Has params, others expected")]
    [InlineData(true, "a,b", "a=b&b=c")]//, DisplayName = "Has params, expected")]
    [InlineData(false, "a", "a=b&b=c")]//, DisplayName = "Has params, not all expected")]
    [InlineData(true, "*", "a=b")]//, DisplayName = "has params, all expected")]
    [InlineData(true, "*", "a=b&b=c")]//, DisplayName = "has params, all expected")]
    [InlineData(true, "a,b", "a=b")]//, DisplayName = "Has params, more can be expected")]
    [InlineData(true, "a,b", "")]//, DisplayName = "No params, various expected")]
    public void NamesShouldDisable(bool expected, string names, string urlParameters)
    {
        var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: names, othersDisableCache: true);
        var result = GetUrlParamsTac(lsDecorator, urlParameters);
        Equal(expected, result.CachingAllowed);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("", "a", "")]
    [InlineData("a=b", "a // this is because xyz", "a=b")]
    [InlineData("a=b&b=c", "a // ok\nb // also ok", "a=b&b=c")]
    [InlineData("a=b&c=d", "* // whatever", "a=b&c=d")]
    public void NamesCanBeMultilineAndCommented(string expValue, string names, string urlParameters, string? message = default)
    {
        var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: names, othersDisableCache: true);
        var result = GetUrlParamsTac(lsDecorator, urlParameters);
        True(result.CachingAllowed);//, message);
        Equal(expValue, result.Extension);
    }


    [Fact]
    public void LoadTestWithoutCaching()
    {
        const int repeat = 10000;
        var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: "a // nice idea\nb // ok too, but a bit nasty\n\ntest // another one to parse", othersDisableCache: true);
        var parameters = "ZETA=last&d=27&a=b&b=c";

        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < repeat; i++) 
            GetUrlParamsTac(lsDecorator, parameters, usePiggyBack: false);
        stopwatch.Stop();

        Trace.WriteLine($"Ran {repeat} iterations without cache, duration was {stopwatch.ElapsedMilliseconds}ms");

        stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < repeat; i++) 
            GetUrlParamsTac(lsDecorator, parameters);
        stopwatch.Stop();

        Trace.WriteLine($"Ran {repeat} iterations with cache, duration was {stopwatch.ElapsedMilliseconds}ms");
    }
}