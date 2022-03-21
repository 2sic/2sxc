using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sxc.Tests.DataForImageTests;
using ToSic.Testing.Shared.Platforms;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Testing.Shared.TestHelpers;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTagsImg : ImageServiceTagsBase
    {
        // Start the test with a platform-info that has WebP support
        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformNotPatron>();
        }

        protected override bool TestModeImg => true;

        [DataRow(ImgTagJpgNone, SrcSetNone, null, "No Src Set")]
        [DataRow(ImgTagJpg12, SrcSet12, null, "With Src Set 1,2")]
        [DataRow(ImgTagJpgNoneF05, SrcSetNone, 0.5, "No Src Set, factor 0.5")]
        [DataTestMethod]
        public void ImageTagMultiTest(string expected, string variants, object factor, string testName)
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

        [DataRow(Img120x24x + ",\n" + Img240x48x, SrcSet12, "With Src Set 1,2")]
        [DataRow("", SrcSetNone, "No Src Set")]
        [DataTestMethod]
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
