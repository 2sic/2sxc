using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using static ToSic.Sxc.DataForImageTests.ResizeRecipesData;

namespace ToSic.Sxc.LinksAndImages.LinkImageTests;


public class MultiResizeTests(LinkImageTestHelper helper)//: LinkImageTestBase
{
    [Fact]
    public void NoFactorMap()
    {
        var l = helper.GetLinker();
        var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000);
        var f1 = l.DimGen.ResizeDimensions(settings, settings.Find(SrcSetType.Img, true, null));
        Equal(1000, f1.Width);

        var f2 = new ResizeSettings(settings, factor: 0.5);
        var dims = l.DimGen.ResizeDimensions(f2, f2.Find(SrcSetType.Img, true, null));
        Equal(500, dims.Width);
    }

    [InlineData(W50, 0.5, CssNone, "0.5 should be changed")]
    [InlineData(700, 0.70, CssNone, "0.70 should not be changed")]
    [InlineData(W75ImgOnly777, 0.75, CssNone, "0.75 should be changed")]
    [InlineData(W75CssUnknown, 0.75, CssUnknown, "0.75 should be changed")]
    [InlineData(W100, 1, CssNone, "1 should be changed too")]
    [InlineData((int)(1000 * .9), 0.9, CssNone, "0.9 should just calculate, because it's not in the factor-list")]
    [Theory]

    public void WithFactorMap(int expected, double factor, string cssFramework, string name) 
        => WithFactorMapInternal(expected, factor, cssFramework, TestRecipeSet(), name);



    [InlineData(W50, 0.5, CssNone, "0.5 should be changed")]
    [InlineData(700, 0.70, CssNone, "0.70 should not be changed")]
    [InlineData(W75ImgOnly777, 0.75, CssNone, "0.75 should be changed")]
    [InlineData(W75CssUnknown, 0.75, CssUnknown, "0.75 should be changed")]
    [InlineData(W100, 1, CssNone, "1 should be changed too")]
    [InlineData((int)(1000 * .9), 0.9, CssNone, "0.9 should just calculate, because it's not in the factor-list")]
    [Theory]

    public void WithFactorMapJson(int expected, double factor, string cssFramework, string name) 
        => WithFactorMapInternal(expected, factor, cssFramework, TestRecipeSetFromJson, name + "-json");


    private void WithFactorMapInternal(int expected, double factor, string cssFramework, AdvancedSettings recipes, string name)
    {
        var l = helper.GetLinker();
        var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000, advanced: recipes);

        // Normally UseFactorMap is false because we set the width explicitly
        // But to run the test, we must set it to true
        settings.UseFactorMap = true;

        settings = new(settings, factor: factor);
        var srcSetSettings = settings.Find(SrcSetType.Img, true, cssFramework);
        var f1 = l.DimGen.ResizeDimensions(settings, srcSetSettings);
        Equal(expected, f1.Width);//, name);
    }
}