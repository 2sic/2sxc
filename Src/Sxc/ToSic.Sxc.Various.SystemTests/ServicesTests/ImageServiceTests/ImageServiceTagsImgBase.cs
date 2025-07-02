using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

public abstract class ImageServiceTagsImgBase(IImageService svc, ITestOutputHelper output, TestScenario? testScenario = default) : ImageServiceTagsBase(svc, output, testScenario)
{
    // These are necessary, as otherwise the test will automatically look up the "Content" settings for image resizing
    // which would result in a different result.
    // TODO: ALSO create a test which uses null, and expects the proper resized with default settings
    object fakeEmptySettings = new object();

    public virtual void ImageTagMultiTest(string expected, string variants, object? factor, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            // Factor set on the Img call
            var settingsWithoutFactor = ImgSvc.SettingsTac(
                settings: fakeEmptySettings,
                width: test.Set.Width,
                height: test.Set.Height,
                recipe: new Recipe(variants: test.Set.Variants)
            );
            var imgSetNoFactor = ImgSvc.Img(link: ImgUrl, settings: settingsWithoutFactor, factor: factor,
                recipe: test.Pic.Variants);
            Equal(expected, imgSetNoFactor.ToString());//, $"Failed (factor on Img): {test.Name}");

            // Factor specified on settings
            var settingsWithFactor = ImgSvc.SettingsTac(
                settings: fakeEmptySettings,
                factor: factor,
                width: test.Set.Width,
                height: test.Set.Height,
                recipe: new Recipe(variants: test.Set.Variants)
            );
            var imgSetFactor = ImgSvc.Img( link: ImgUrl, settings: settingsWithFactor, recipe: test.Pic.Variants);
            Equal(expected, imgSetFactor.ToString());//, $"Failed (factor on settings): {test.Name}");

            // Factor on both - should not equal, because the factor is only applied 1x
            if (factor == null)
                return; // Skip if the factor has no effect
            var imgBothFactors = ImgSvc.Img(link: ImgUrl, settings: settingsWithFactor, factor: factor,
                recipe: test.Pic.Variants);
            Equal(expected, imgBothFactors.ToString());//, $"Failed (factor on both): {test.Name}");
        });
    }

    public virtual void ImageSrcSetMultiTest(string expected, string variants, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            var settings = ImgSvc.SettingsTac(
                settings: fakeEmptySettings,
                width: test.Set.Width,
                height: test.Set.Height,
                recipe: test.Set.SrcSetRule
            );
            var img = ImgSvc.Img(link: ImgUrl, settings: settings, recipe: test.Pic.SrcSetRule);
            Equal(expected?.Replace("&amp;", "&"), img.SrcSet);//, $"Failed: {test.Name}");
        });
    }

}