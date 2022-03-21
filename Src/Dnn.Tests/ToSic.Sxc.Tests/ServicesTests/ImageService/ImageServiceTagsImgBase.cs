using System.Linq;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Testing.Shared.TestHelpers;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public class ImageServiceTagsImgBase: ImageServiceTagsBase
    {

        protected void ImageTagMultiTest(string expected, string variants, object factor, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
            var svc = Build<IImageService>();
            TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
            {
                // Factor set on the Img call
                var settingsWithoutFactor = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height,
                    recipe: new Recipe(variants: test.Set.Variants));
                var imgSetNoFactor = svc.Img(url: ImgUrl, settings: settingsWithoutFactor, factor: factor,
                    recipe: test.Pic.Variants);
                 Is(expected, imgSetNoFactor.ToString(), $"Failed (factor on Img): {test.Name}");

                // Factor specified on settings
                var settingsWithFactor = svc.ResizeSettings(factor: factor, width: test.Set.Width,
                    height: test.Set.Height,
                    recipe: new Recipe(variants: test.Set.Variants));
                var imgSetFactor = svc.Img(url: ImgUrl, settings: settingsWithFactor, recipe: test.Pic.Variants);
                Is(expected, imgSetFactor.ToString(), $"Failed (factor on settings): {test.Name}");

                // Factor on both - should not equal, because the factor is only applied 1x
                if (factor == null) return; // Skip if the factor has no effect
                var imgBothFactors = svc.Img(url: ImgUrl, settings: settingsWithFactor, factor: factor,
                    recipe: test.Pic.Variants);
                Is(expected, imgBothFactors.ToString(), $"Failed (factor on both): {test.Name}");
            });
        }

        public void ImageSrcSetMultiTest(string expected, string variants, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
            TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, recipe: test.Set.SrcSetRule);
                var img = svc.Img(url: ImgUrl, settings: settings, recipe: test.Pic.SrcSetRule);
                AreEqual(expected.Replace("&amp;", "&"), img.Srcset, $"Failed: {test.Name}");
            });
        }

    }
}
