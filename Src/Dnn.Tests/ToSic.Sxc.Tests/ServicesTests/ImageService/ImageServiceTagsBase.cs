using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public abstract class ImageServiceTagsBase: TestBaseSxcDb
    {
        protected const string ImgUrl = "/abc/def/test.jpg";
        protected const string Img120x24 = ImgUrl + "?w=120&amp;h=24";
        protected const string Img120x24x = Img120x24 + " 1x";
        protected const string Img240x48 = ImgUrl + "?w=240&amp;h=48";
        protected const string Img240x48x = Img240x48 + " 2x";
        protected const string SrcSet12 = "1,2";
        protected const string SrcSet12ResultWebP = "srcset='" + Img120x24 + "&amp;format=webp 1x," + ImgUrl + "?w=240&amp;h=48&amp;format=webp 2x'";
        protected const string SrcSet12ResultJpg = "srcset='" + Img120x24x + "," + Img240x48x + "'";
        protected const string SrcWebP12 = "<source type='image/webp' " + SrcSet12ResultWebP + ">";
        protected const string SrcJpg12 = "<source type='image/jpeg' " + SrcSet12ResultJpg + ">";
        protected const string ImgTagJpg12 = "<img src='" + Img120x24 + "' " + SrcSet12ResultJpg + ">";

        protected const string SrcSetNone = null;
        protected const string SrcWebPNone = "<source type='image/webp' srcset='" + ImgUrl + "?w=120&amp;h=24&amp;format=webp'>";
        protected const string SrcJpgNone = "<source type='image/jpeg' srcset='" + ImgUrl + "?w=120&amp;h=24'>";
        protected const string ImgTagJpgNone = "<img src='" + ImgUrl + "?w=120&amp;h=24'>";



        protected void PictureTagInner(string expectedParts, string srcset, bool inPicTag, string testName)
        {
            var svc = Build<IImageService>();
            var settings = svc.ResizeSettings(width: 120, height: 24, srcset: inPicTag ? null : srcset);
            var pic = svc.Picture(ImgUrl, settings: settings, srcset: inPicTag ? srcset : null);

            var expected = $"<picture>{expectedParts}<img src='{ImgUrl}?w=120&amp;h=24'></picture>";
            Assert.AreEqual(expected, pic.ToString(), $"Test failed: {testName}");
        }


        /// <summary>
        /// Run a batch of tests on the source tags, with permutations of where the settings are given
        /// </summary>
        protected void SourceTagsMultiTest(string expected, string srcset, string testName)
        {
            var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, srcset);
            foreach (var test in testSet)
            {
                var svc = Build<IImageService>();
                var settings = svc.ResizeSettings(width: test.Set.Width, height: test.Set.Height, srcset: test.Set.Srcset);
                var sources = svc.Picture(ImgUrl, settings: settings, srcset: test.Pic.Srcset).Sources;

                Assert.AreEqual(expected, sources.ToString(), $"Failed: {test.Name}");
            }
        }

    }
}
