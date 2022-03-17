using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Testing.Shared.Platforms;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTagsImg : ImageServiceTagsBase
    {
        // Start the test with a platform-info that has WebP support
        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformNoLicense>();
        }

        protected override bool TestModeImg => true;

        [DataRow(ImgTagJpgNone, SrcSetNone, null, "No Src Set")]
        [DataRow(ImgTagJpg12, SrcSet12, null, "With Src Set 1,2")]
        [DataRow(ImgTagJpgNoneF05, SrcSetNone, 0.5, "No Src Set, factor 0.5")]
        [DataTestMethod]
        public void ImageTagMultiTest(string expected, string srcset, object factor, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            var svc = Build<IImageService>();
            foreach (var test in testSet)
            {
                // Factor set on the Img call
                var settingsWithoutFactor = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height,
                    rules: new MultiResizeRule() { SrcSet = test.Set.Srcset });
                var imgSetNoFactor = svc.Img(ImgUrl, settings: settingsWithoutFactor, factor: factor, rules: test.Pic.Srcset);
                AreEqual(expected, imgSetNoFactor.ToString(), $"Failed (factor on Img): {test.Name}");

                // Factor specified on settings
                var settingsWithFactor = svc.ResizeSettings(factor: factor, width: test.Set.Width, height: test.Set.Height, rules: new MultiResizeRule() { SrcSet = test.Set.Srcset });
                var imgSetFactor = svc.Img(ImgUrl, settings: settingsWithFactor, rules: test.Pic.Srcset);
                AreEqual(expected, imgSetFactor.ToString(), $"Failed (factor on settings): {test.Name}");

                // Factor on both - should not equal, because the factor is only applied 1x
                if (factor == null) return; // Skip if the factor has no effect
                var imgBothFactors = svc.Img(ImgUrl, settings: settingsWithFactor, factor: factor, rules: test.Pic.Srcset);
                AreEqual(expected, imgBothFactors.ToString(), $"Failed (factor on both): {test.Name}");
            }
        }

        [DataRow(Img120x24x + ",\n" + Img240x48x, SrcSet12, "With Src Set 1,2")]
        [DataRow("", SrcSetNone, "No Src Set")]
        [DataTestMethod]
        public void ImageSrcSetMultiTest(string expected, string srcset, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height,
                    rules: test.Set.SrcSetRule); // test.Set.Srcset == null ? null : new MultiResizeRule { SrcSet = test.Set.Srcset });
                var img = svc.Img(ImgUrl, settings: settings,
                    rules: test.Pic.SrcSetRule);// test.Pic.Srcset == null ? null : new MultiResizeRule { SrcSet = test.Pic.Srcset });
                AreEqual(expected.Replace("&amp;", "&"), img.Srcset, $"Failed: {test.Name}");
            });
        }


    }
}
