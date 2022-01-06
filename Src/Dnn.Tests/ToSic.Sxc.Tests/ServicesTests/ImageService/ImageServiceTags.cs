using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Razor.Html5;
using ToSic.Sxc.Services.Image;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTags: TestBaseSxc
    {
        private const string ImgBase = "/abc/def/test.jpg";
        private const string SrcSet12 = "1,2";
        private static string SrcWebP12 = $"<source type='image/webp' srcset='{ImgBase}?w=120&amp;h=24&amp;format=webp 1x,{ImgBase}?w=240&amp;h=48&amp;format=webp 2x'>";
        private static string SrcJpg12 = $"<source type='image/jpeg' srcset='{ImgBase}?w=120&amp;h=24 1x,{ImgBase}?w=240&amp;h=48 2x'>";

        private static string SrcSetNone = null;
        private static string SrcWebPNone = $"<source type='image/webp' srcset='{ImgBase}?w=120&amp;h=24&amp;format=webp'>";
        private static string SrcJpgNone = $"<source type='image/jpeg' srcset='{ImgBase}?w=120&amp;h=24'>";


        [TestMethod]
        public void SourceTags12()
        {
            var svc = Build<IImageService>();
            var settings = svc.GetResizeSettings(width: 120, height: 24);
            var sources = svc.SourceTags(ImgBase, settings, srcSet: SrcSet12);

            var expected = SrcWebP12 + SrcJpg12;
            Assert.AreEqual(expected, sources.ToString());
        }

        [TestMethod]
        public void SourceTagsNone()
        {
            var svc = Build<IImageService>();
            var settings = svc.GetResizeSettings(width: 120, height: 24);
            var sources = svc.SourceTags(ImgBase, settings, srcSet: SrcSetNone);

            var expected = SrcWebPNone + SrcJpgNone;
            Assert.AreEqual(expected, sources.ToString());
        }

        [TestMethod]
        public void PictureTagNoSet()
        {
            var svc = Build<IImageService>();
            var settings = svc.GetResizeSettings(width: 120, height: 24);
            var pic = svc.PictureTag(ImgBase, settings);

            var expected = $"<picture>{SrcWebPNone}{SrcJpgNone}<img src='{ImgBase}?w=120&amp;h=24'></picture>";
            Assert.AreEqual(expected, pic.ToString());
        }

        [TestMethod]
        public void PictureTag12()
        {
            var svc = Build<IImageService>();
            var settings = svc.GetResizeSettings(width: 120, height: 24);
            var pic = svc.PictureTag(ImgBase, settings, srcSet: SrcSet12);

            var expected = $"<picture>{SrcWebP12}{SrcJpg12}<img src='{ImgBase}?w=120&amp;h=24'></picture>";
            Assert.AreEqual(expected, pic.ToString());
        }
    }
}
