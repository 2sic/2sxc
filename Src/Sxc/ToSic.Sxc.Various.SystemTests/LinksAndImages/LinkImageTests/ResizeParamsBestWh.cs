using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sys.GetByName;

#pragma warning disable xUnit1026

namespace ToSic.Sxc.LinksAndImages.LinkImageTests;


public class ResizeParamsBestWh(LinkImageTestHelper helper)//: LinkImageTestBase
{


    const int s = 1440; // standard value, divisible by 1/2/3/4/6/12

    [Fact]
    public void FigureOutBestWidthAndHeight()
    {
        // All Zero and Nulls
        True(TestBestWH(0, 0), "Null everything");
        True(TestBestWH(0, 0, 0), "Null everything");
        True(TestBestWH(0, 0, 0, 0), "Null everything");
        True(TestBestWH(0, 0, 0, 0, 0), "Null everything");
        True(TestBestWH(0, 0, 0, 0, 0, 0), "Null everything");

        // Single Aspect is set
        True(TestBestWH(s, 0, width: s), "W only");
        True(TestBestWH(0, s, height: s), "H only");
        True(TestBestWH(0, 0, factor: 7), "factor only");
        True(TestBestWH(0, 0, ar: 7), "ar only");

        // H / W
        True(TestBestWH(s, s, width: s, height: s), "W & H only");
    }


    // W / Factor - factor should apply now 13.03
    [InlineData(s, 0, s, 1, "W/F only")]
    [InlineData(s * .5, 0, s, .5, "W/F only")]
    [InlineData(s * 2, 0, s, 2, "W/F only")]
    [InlineData(s * 1.5, 0, s, 1.5, "W/F only")]
    [Theory]
    public void FigureOutBestWidthAndHeight_Width(double expW, int expH, int w, double f, string name) 
        => True(TestBestWH((int)expW, expH, width: w, factor: f), name);

    // H / W / Factor - factor should  apply now 13.03
    [InlineData(s, s, 1, "W/H/F")]
    [InlineData(s * 0.5, s, .5, "W/H/F")]
    [InlineData(s * 2, s, 2, "W/H/F")]
    [InlineData(s * 1.5, s, 1.5, "W/H/F")]
    [Theory]
    public void FigureOutBestWidthAndHeight_Square(double exp, int x, double f, string name) 
        => True(TestBestWH((int)exp, (int)exp, width: x, height: x, factor: f), name);


    // W / Factor - factor should apply now 13.03
    [InlineData(s, 0, s, 1, "W/F only")]
    [InlineData(s * .5, 0, s, .5, "W/F only")]
    [InlineData(s * 2, 0, s, 2, "W/F only")]
    [InlineData(s * 1.5, 0, s, 1.5, "W/F only")]
    [Theory]
    public void FigureOutBestWidthAndHeight_Height(double expH, int expW, int h, double f, string name) 
        => True(TestBestWH(expW, (int)expH, height: h, factor: f), "H/F only");

    [Fact]
    public void FigureOutBestWidthAndHeight_SettingsWH()
    {
        True(TestBestWH(s, 0, settings: helper.ToDyn(new { Width = s })), "W only");
        True(TestBestWH(0, s, settings: helper.ToDyn(new { Height = s })), "H only");
        True(TestBestWH(0, 0, settings: helper.ToDyn(new { })), "No params");
        True(TestBestWH(s, s, settings: helper.ToDyn(new { Width = s, Height = s })), "W/H params");
    }


    [Fact]
    public void FigureOutBestWH_SettingsWithNeutralizer()
    {
        True(TestBestWH(0, 0, 0, null, null, null, helper.ToDyn(new { Width = 700 })));
    }

    [InlineData(0, 1, "Factor 0, result 1")]
    [InlineData(1, 1, "Factor 1, result 1")]
    [InlineData(2, 2, "Factor 2, result 2")]
    [InlineData(0.5, 0.5, "Factor / result 0.5")]
    [Theory]
    public void FigureOutBestWidthAndHeight_SettingsWHF(double factor, double fResult, string name)
    {
        Console.WriteLine("Run with Factor: " + factor + $"; start-value is {s}");
        // Factor 1
        True(TestBestWH((int)(fResult * s), 0, factor: factor, settings: helper.ToDyn(new { Width = s })), $"Calc W with f:{factor}");
        True(TestBestWH(0, (int)(fResult * s), factor: factor, settings: helper.ToDyn(new { Height = s })), $"Calc H with f:{factor}");
        True(TestBestWH(0, 0, factor: factor, settings: helper.ToDyn(new { })), $"No params - f shouldn't have an effect f:{factor}");
        True(TestBestWH((int)(fResult * s), (int)(fResult * s), factor: factor, settings: helper.ToDyn(new { Width = s, Height = s })), $"Calc W and H in settings with f:{factor}");
    }


    internal static bool TestBestWH(int expW, int expH, object width = null, object height = null, object factor = null, object ar = null, ICanGetByName settings = null)
    {
        // Get a new linker for each test run
        var paramMerger = new ResizeParamMerger(null);
        var dimGen = new ResizeDimensionGenerator();

        var resizeSettings = paramMerger.BuildCoreSettings(new(null), width, height, factor, ar, null, settings);
        var t1 = dimGen.ResizeDimensions(resizeSettings, resizeSettings.Find(SrcSetType.Img, true, null));
        var okW = expW.Equals(t1.Width);
        var okH = expH.Equals(t1.Height);
        var ok = okW && okH;
        Console.WriteLine((ok ? "ok" : "error") + "; W:" + t1.Width + (okW ? "==": "!=") + expW + "; H:" + t1.Width + (okH ? "==" : "!=") + expH);
        return ok;
    }
}