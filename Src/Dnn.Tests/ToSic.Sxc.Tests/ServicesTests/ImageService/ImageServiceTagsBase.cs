using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public abstract class ImageServiceTagsBase: TestBaseSxcDb
    {
        protected ImageServiceTagsBase(): base()
        {
            // this will run after the base has done it's work
        }

        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            // Reset some important static stuff

            // Just call the normal chain
            return base.SetupServices(services);
        }


        protected const string ImgBase = "/abc/def/test.jpg";
        protected const string SrcSet12 = "1,2";
        protected static string SrcWebP12 = $"<source type='image/webp' srcset='{ImgBase}?w=120&amp;h=24&amp;format=webp 1x,{ImgBase}?w=240&amp;h=48&amp;format=webp 2x'>";
        protected static string SrcJpg12 = $"<source type='image/jpeg' srcset='{ImgBase}?w=120&amp;h=24 1x,{ImgBase}?w=240&amp;h=48 2x'>";

        protected static string SrcSetNone = null;
        protected static string SrcWebPNone = $"<source type='image/webp' srcset='{ImgBase}?w=120&amp;h=24&amp;format=webp'>";
        protected static string SrcJpgNone = $"<source type='image/jpeg' srcset='{ImgBase}?w=120&amp;h=24'>";


        protected void SourceTags12(string expected)
        {
            var svc = Build<IImageService>();
            var settings = svc.ResizeSettings(width: 120, height: 24);
            var sources = svc.Picture(ImgBase, settings: settings, srcSet: SrcSet12).SourceTags;

            Assert.AreEqual(expected, sources.ToString());
        }

        protected void SourceTagsNone(string expected)
        {
            var svc = Build<IImageService>();
            var settings = svc.ResizeSettings(width: 120, height: 24);
            var sources = svc.Picture(ImgBase, settings: settings, srcSet: SrcSetNone).SourceTags;

            Assert.AreEqual(expected, sources.ToString());
        }

        protected void PictureTagNoSet(string expectedParts)
        {
            var svc = Build<IImageService>();
            var settings = svc.ResizeSettings(width: 120, height: 24);
            var pic = svc.Picture(ImgBase, settings: settings);

            var expected = $"<picture>{expectedParts}<img src='{ImgBase}?w=120&amp;h=24'></picture>";
            Assert.AreEqual(expected, pic.ToString());
        }

        protected void PictureTag12(string expectedParts)
        {
            var svc = Build<IImageService>();
            var settings = svc.ResizeSettings(width: 120, height: 24);
            var pic = svc.Picture(ImgBase, settings: settings, srcSet: SrcSet12);

            var expected = $"<picture>{expectedParts}<img src='{ImgBase}?w=120&amp;h=24'></picture>";
            Assert.AreEqual(expected, pic.ToString());
        }
    }
}
