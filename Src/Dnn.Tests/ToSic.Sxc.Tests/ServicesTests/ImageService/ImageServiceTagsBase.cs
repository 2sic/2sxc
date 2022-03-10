using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public abstract class ImageServiceTagsBase: TestBaseSxcDb
    {
        protected const string ImgUrl = "/abc/def/test.jpg";
        protected const string SrcSet12 = "1,2";
        protected const string SrcSet12ResultWebP = "srcset='" + ImgUrl + "?w=120&amp;h=24&amp;format=webp 1x," + ImgUrl + "?w=240&amp;h=48&amp;format=webp 2x'";
        protected const string SrcSet12ResultJpg = "srcset='" + ImgUrl + "?w=120&amp;h=24 1x," + ImgUrl + "?w=240&amp;h=48 2x'";
        protected const string SrcWebP12 = "<source type='image/webp' " + SrcSet12ResultWebP + ">";
        protected const string SrcJpg12 = "<source type='image/jpeg' " + SrcSet12ResultJpg + ">";
        protected const string ImgTagJpg = "<img src='" + ImgUrl + "?w=120&amp;h=24' " + SrcSet12ResultJpg + ">";

        protected const string SrcSetNone = null;
        protected const string SrcWebPNone = "<source type='image/webp' srcset='" + ImgUrl + "?w=120&amp;h=24&amp;format=webp'>";
        protected const string SrcJpgNone = "<source type='image/jpeg' srcset='" + ImgUrl + "?w=120&amp;h=24'>";


        protected void ImageTagMultiTest(string expected, string srcSet, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcSet);
            foreach (var test in testSet)
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcSet: test.Set.SrcSet);
                var img = svc.Img(ImgUrl, settings: settings, srcSet: test.Pic.SrcSet);

                Assert.AreEqual(expected, img.ToString(), $"Failed: {test.Name}");
            }

        }


        protected void PictureTagInner(string expectedParts, string srcSet, bool inPicTag, string testName)
        {
            var svc = Build<IImageService>();
            var settings = svc.ResizeSettings(width: 120, height: 24, srcSet: inPicTag ? null : srcSet);
            var pic = svc.Picture(ImgUrl, settings: settings, srcSet: inPicTag ? srcSet : null);

            var expected = $"<picture>{expectedParts}<img src='{ImgUrl}?w=120&amp;h=24'></picture>";
            Assert.AreEqual(expected, pic.ToString(), $"Test failed: {testName}");
        }


        /// <summary>
        /// Run a batch of tests on the source tags, with permutations of where the settings are given
        /// </summary>
        protected void SourceTagsMultiTest(string expected, string srcSet, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcSet);
            foreach (var test in testSet)
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcSet: test.Set.SrcSet);
                var sources = svc.Picture(ImgUrl, settings: settings, srcSet: test.Pic.SrcSet).SourceTags;

                Assert.AreEqual(expected, sources.ToString(), $"Failed: {test.Name}");
            }
        }

    }
}
