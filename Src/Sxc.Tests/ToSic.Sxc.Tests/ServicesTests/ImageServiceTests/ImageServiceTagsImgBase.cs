using System.Linq;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Testing.Shared.TestHelpers;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

public class ImageServiceTagsImgBase: ImageServiceTagsBase
{

    protected void ImageTagMultiTest(string expected, string variants, object factor, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        var svc = GetService<IImageService>();
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            // Factor set on the Img call
            var settingsWithoutFactor = svc.SettingsTac(width: test.Set.Width, height: test.Set.Height,
                recipe: new Recipe(variants: test.Set.Variants));
            var imgSetNoFactor = svc.Img(link: ImgUrl, settings: settingsWithoutFactor, factor: factor,
                recipe: test.Pic.Variants);
            Is(expected, imgSetNoFactor.ToString(), $"Failed (factor on Img): {test.Name}");

            // Factor specified on settings
            var settingsWithFactor = svc.SettingsTac(factor: factor, width: test.Set.Width,
                height: test.Set.Height,
                recipe: new Recipe(variants: test.Set.Variants));
            var imgSetFactor = svc.Img( link: ImgUrl, settings: settingsWithFactor, recipe: test.Pic.Variants);
            Is(expected, imgSetFactor.ToString(), $"Failed (factor on settings): {test.Name}");

            // Factor on both - should not equal, because the factor is only applied 1x
            if (factor == null) return; // Skip if the factor has no effect
            var imgBothFactors = svc.Img(link: ImgUrl, settings: settingsWithFactor, factor: factor,
                recipe: test.Pic.Variants);
            Is(expected, imgBothFactors.ToString(), $"Failed (factor on both): {test.Name}");
        });
    }

    public void ImageSrcSetMultiTest(string expected, string variants, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            var svc = GetService<IImageService>();
            var settings = svc.SettingsTac(width: test.Set.Width, height: test.Set.Height, recipe: test.Set.SrcSetRule);
            var img = svc.Img(link: ImgUrl, settings: settings, recipe: test.Pic.SrcSetRule);
            AreEqual(expected?.Replace("&amp;", "&"), img.SrcSet, $"Failed: {test.Name}");
        });
    }

}