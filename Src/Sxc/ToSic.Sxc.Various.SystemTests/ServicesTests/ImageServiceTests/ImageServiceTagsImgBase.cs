using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

public abstract class ImageServiceTagsImgBase(IImageService svc, ITestOutputHelper output, TestScenario? testScenario = default) : ImageServiceTagsBase(svc, output, testScenario)
{

    public virtual void ImageTagMultiTest(string expected, string variants, object factor, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            // Factor set on the Img call
            var settingsWithoutFactor = svc.SettingsTac(width: test.Set.Width, height: test.Set.Height,
                recipe: new Recipe(variants: test.Set.Variants));
            var imgSetNoFactor = svc.Img(link: ImgUrl, settings: settingsWithoutFactor, factor: factor,
                recipe: test.Pic.Variants);
            Equal(expected, imgSetNoFactor.ToString());//, $"Failed (factor on Img): {test.Name}");

            // Factor specified on settings
            var settingsWithFactor = svc.SettingsTac(factor: factor, width: test.Set.Width,
                height: test.Set.Height,
                recipe: new Recipe(variants: test.Set.Variants));
            var imgSetFactor = svc.Img( link: ImgUrl, settings: settingsWithFactor, recipe: test.Pic.Variants);
            Equal(expected, imgSetFactor.ToString());//, $"Failed (factor on settings): {test.Name}");

            // Factor on both - should not equal, because the factor is only applied 1x
            if (factor == null) return; // Skip if the factor has no effect
            var imgBothFactors = svc.Img(link: ImgUrl, settings: settingsWithFactor, factor: factor,
                recipe: test.Pic.Variants);
            Equal(expected, imgBothFactors.ToString());//, $"Failed (factor on both): {test.Name}");
        });
    }

    public virtual void ImageSrcSetMultiTest(string expected, string variants, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            //var svc = GetService<IImageService>();
            var settings = svc.SettingsTac(width: test.Set.Width, height: test.Set.Height, recipe: test.Set.SrcSetRule);
            var img = svc.Img(link: ImgUrl, settings: settings, recipe: test.Pic.SrcSetRule);
            Equal(expected?.Replace("&amp;", "&"), img.SrcSet);//, $"Failed: {test.Name}");
        });
    }

}