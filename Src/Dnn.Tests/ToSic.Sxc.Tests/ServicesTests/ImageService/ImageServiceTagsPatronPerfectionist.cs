using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sxc.Tests.DataForImageTests;
using ToSic.Testing.Shared.Platforms;
using static ToSic.Testing.Shared.TestHelpers;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTagsPatronPerfectionist: ImageServiceTagsBase
    {
        /// <summary>
        /// Start the test with a platform-info that has WebP support
        /// </summary>
        protected override IServiceCollection SetupServices(IServiceCollection services)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();
        }

        protected override bool TestModeImg => false;


        [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, "No Src Set")]
        [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, "With Src Set 1,2")]
        [DataTestMethod]
        public void SourceTagsMultiTests(string expected, string variants, string name) 
            => SourceTagsMultiTest(expected, variants, name);

        [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
        [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, false, "No Src Set, in-setting")]
        [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, true, "With Src Set 1,2, in-pic")]
        [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, false, "With Src Set 1,2, in-settings")]
        [DataTestMethod]
        public void PictureTags(string expected, string variants, bool inPicTag, string name)
            => PictureTagInner(expected, variants, inPicTag, name);


        [DataRow("<img src='test.jpg?w=777' class='img-fluid' test='value'>", 0.75, "0.75 with attributes")]
        [DataTestMethod]
        public void ImgWhichShouldAutoGetAttributes(string expected, double factor, string name)
        {
            var set = ResizeRecipesData.TestRecipeSet();
            var svc = Build<IImageService>();
            var img = svc.Img(link: "test.jpg", factor: factor, recipe: set);
            Is(expected, img.ToString(), name);
        }

        [TestMethod]
        public void ImgWhichShouldSetWidth()
        {
            var recipe = new Recipe(width: 1000, variants: "1", setWidth: true,
                attributes: new Dictionary<string, object>
                {
                    { "class", "img-fluid" }, 
                    { "sizes", "100vw" }
                });
            var svc = Build<IImageService>();
            var img = svc.Img(link: "test.jpg", factor: 0.5, imgClass: "added", recipe: recipe);
            Is("<img src='test.jpg?w=1000' class='img-fluid added' width='1000' srcset='test.jpg?w=1000 1x' sizes='100vw'>", img.ToString(), "test");

        }

    }
}
