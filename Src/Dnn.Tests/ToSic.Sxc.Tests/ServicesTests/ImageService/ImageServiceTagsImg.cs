using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
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

        [TestMethod]
        public void ImgAlt()
        {
            var svc = Build<IImageService>();
            var img = svc.Img(ImgUrl, imgAlt: "test-alt");
            AreEqual($"<img src='{ImgUrl}' alt='test-alt'>", img.ToString());
        }

        [TestMethod]
        public void ImgClass()
        {
            var svc = Build<IImageService>();
            var img = svc.Img(ImgUrl, imgClass: "class-dummy");
            AreEqual($"<img src='{ImgUrl}' class='class-dummy'>", img.ToString());
        }

        [DataRow(ImgTagJpgNone, SrcSetNone, null, "No Src Set")]
        [DataRow(ImgTagJpg12, SrcSet12, null, "With Src Set 1,2")]
        [DataRow(ImgTagJpgNoneF05, SrcSetNone, 0.5, "No Src Set, factor 0.5")]
        [DataTestMethod]
        public void ImageTagMultiTest(string expected, string srcset, object factor, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            foreach (var test in testSet)
            {
                // Factor set on the Img call
                var svc = Build<IImageService>();
                var settingsWithoutFactor = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcset: test.Set.Srcset);
                var imgSetNoFactor = svc.Img(ImgUrl, settings: settingsWithoutFactor, factor: factor, srcset: test.Pic.Srcset);
                AreEqual(expected, imgSetNoFactor.ToString(), $"Failed (factor on Img): {test.Name}");

                // Factor specified on settings
                var settingsWithFactor = svc.ResizeSettings(factor: factor, width: test.Set.Width, height: test.Set.Height, srcset: test.Set.Srcset);
                var imgSetFactor = svc.Img(ImgUrl, settings: settingsWithFactor, srcset: test.Pic.Srcset);
                AreEqual(expected, imgSetFactor.ToString(), $"Failed (factor on settings): {test.Name}");

                // Factor on both - should not equal, because the factor is applied 2x
                var imgBothFactors = svc.Img(ImgUrl, settings: settingsWithFactor, factor: factor, srcset: test.Pic.Srcset);
                AreNotEqual(expected, imgBothFactors.ToString(), $"Failed (factor on both): {test.Name}");
            }
        }

        [DataRow("", SrcSetNone, "No Src Set")]
        [DataRow(Img120x24x + ",\n" + Img240x48x, SrcSet12, "With Src Set 1,2")]
        [DataTestMethod]
        public void ImageSrcSetMultiTest(string expected, string srcset, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            foreach (var test in testSet)
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcset: test.Set.Srcset);
                var img = svc.Img(ImgUrl, settings: settings, srcset: test.Pic.Srcset);

                AreEqual(expected.Replace("&amp;", "&"), img.Srcset, $"Failed: {test.Name}");
            }
        }
    }
}
