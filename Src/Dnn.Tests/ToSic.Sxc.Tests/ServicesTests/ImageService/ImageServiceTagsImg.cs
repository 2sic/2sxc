using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Sxc.Services;
using ToSic.Testing.Shared.Platforms;

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

        [DataRow(ImgTagJpgNone, SrcSetNone, "No Src Set")]
        [DataRow(ImgTagJpg12, SrcSet12, "With Src Set 1,2")]
        [TestMethod]
        public void ImageTagMultiTest(string expected, string srcset, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            foreach (var test in testSet)
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcset: test.Set.Srcset);
                var img = svc.Img(ImgUrl, settings: settings, srcset: test.Pic.Srcset);

                Assert.AreEqual(expected, img.ToString(), $"Failed: {test.Name}");
            }
        }

        [DataRow("", SrcSetNone, "No Src Set")]
        [DataRow(ImgTagJpg12, SrcSet12, "With Src Set 1,2")]
        [TestMethod]
        public void ImageSrcSetMultiTest(string expected, string srcset, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            foreach (var test in testSet)
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcset: test.Set.Srcset);
                var img = svc.Img(ImgUrl, settings: settings, srcset: test.Pic.Srcset);

                Assert.AreEqual(expected, img.Srcset, $"Failed: {test.Name}");
            }
        }
    }
}
