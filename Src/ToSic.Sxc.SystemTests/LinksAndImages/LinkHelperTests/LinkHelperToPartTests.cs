using ToSic.Sxc.Services;
using static ToSic.Sxc.Services.Internal.LinkServiceUnknown;
using static ToSic.Sxc.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperToPartTests(ILinkService link) : LinkHelperToTestBase(link)
{

    [Fact]
    public void NoPage()
    {
        TestToPageParts(null, null, NiceCurrentUrl, NiceCurrentUrl, NiceCurrentRelative);
    }

    [Fact]
    public void NoPageStringParamWithQuestion()
    {
        var query = "active=true";
        var exp = $"{NiceCurrentUrlRoot}?{query}";
        var rel = $"{NiceCurrentRelative}?{query}";
        TestToPageParts(null, $"?{query}", exp, exp, rel);
    }

    [Fact]
    public void NoPageStringParamWithAmpersand()
    {
        var query = "active=true";
        var exp = $"{NiceCurrentUrlRoot}?{query}";
        var rel = $"{NiceCurrentRelative}?{query}";
        TestToPageParts(null, $"&{query}", exp, exp, rel);
    }

    private void NoPageStringParam(string query)
    {
        var exp = NiceCurrentUrlRoot + "?" + query;
        var rel = $"{NiceCurrentRelative}?{query}";
        TestToPageParts(null, query, exp, exp, rel);
    }

    [Fact] public void NoPageStringParamNoValue() => NoPageStringParam("active");
    [Fact] public void NoPageStringParamNoValue2() => NoPageStringParam("active&passive");
    [Fact] public void NoPageStringParamValue1() => NoPageStringParam("this=active");
    [Fact] public void NoPageStringParamValue2() => NoPageStringParam("this=active&that=passive");
    [Fact] public void NoPageStringParamValueParamNoValue() => NoPageStringParam("this=active&passive");
    [Fact] public void NoPageStringParamNoValueParamValue() => NoPageStringParam("active&that=passive");

    private void NoPageObjectParam(object parameters, string expQuery)
    {
        var fullQuery = (string.IsNullOrEmpty(expQuery) ? "" : "?") + expQuery;
        var exp = NiceCurrentUrlRoot + fullQuery;
        var rel = $"{NiceCurrentRelative}{fullQuery}";
        TestToPageParts(null, parameters, exp, exp, rel);
    }

    [Fact] public void NoPageObjectParamUnsupported() => NoPageObjectParam(new DateTime(), "");

    [Fact]
    public void NoPageObjectParamsEmpty() => NoPageObjectParam(NewParameters(null), "");

    [Fact]
    public void NoPageObjectParamsKeyOnly() =>
        NoPageObjectParam(NewParameters(new() { { "active", null } }), "active");

    [Fact]
    public void NoPageObjectParamsKeyValueEmpty() =>
        NoPageObjectParam(NewParameters(new() { { "active", "" } }), "active");

    [Fact]
    public void NoPageObjectParamsKeyEmpty() =>
        NoPageObjectParam(NewParameters(new() { { "", "" } }), "");

    [Fact]
    public void NoPageObjectParamDicKeyValue() =>
        NoPageObjectParam(NewParameters(new() { { "active", "true" } }), "active=true");

    [Fact]
    public void NoPageObjectParamDicObject() =>
        NoPageObjectParam(NewParameters(new() { { "active", "true" }, {"passive", "false"} }), "active=true&passive=false");


    /// <summary>
    /// This will reconfigure the LinkHelperUnknown to deliver ugly dnn-url like ...?tabId=27
    /// </summary>
    /// <param name="action"></param>
    private void RunWithUglyUrl(Action action)
    {
        SwitchModeToUgly(true);
        try
        {
            action.Invoke();
        }
        finally
        {
            SwitchModeToUgly(false);
        }
    }

    //[Fact]
    //[Ignore]
    //public void NoPageUgly()
    //{
    //    RunWithUglyUrl(() => { }
    //        //TestToPageParts(null, null, UglyCurrentUrl, UglyCurrentUrl, DefProtocol, DefDomain, null, null,
    //        //    UglyCurrentQuery, "", "?" + UglyCurrentQuery)
    //    );
    //}


    [Fact]
    public void Page27Plain()
    {
        var exp = NiceAnyPageUrl.Replace("{0}", "27");
        var rel = NiceAnyRelative.Replace("{0}", "27");
        TestToPageParts(27, null, exp, exp, rel);
    }

    private void Page27StringParam(string query)
    {
        var exp = NiceCurrentUrlRoot + "?" + query;
        var rel = NiceAnyRelative.Replace("{0}", "27") + "?" + query;
        TestToPageParts(27, query, exp, exp, rel);
    }
}