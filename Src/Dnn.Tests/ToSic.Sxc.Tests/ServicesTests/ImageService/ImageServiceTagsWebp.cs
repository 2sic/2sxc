using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTagsWebp: ImageServiceTagsBase
    {
        /// <summary>
        /// Start the test with a platform-info that has WebP support
        /// </summary>
        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformWithLicense>();
        }

        protected override bool TestModeImg => false;


        [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, "No Src Set")]
        [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, "With Src Set 1,2")]
        [DataTestMethod]
        public void SourceTagsMultiTests(string expected, string srcset, string name) => SourceTagsMultiTest(expected, srcset, name);

        [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
        [DataRow(SrcWebPNone + SrcJpgNone, SrcSetNone, false, "No Src Set, in-setting")]
        [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, true, "With Src Set 1,2, in-pic")]
        [DataRow(SrcWebP12 + SrcJpg12, SrcSet12, false, "With Src Set 1,2, in-settings")]
        [DataTestMethod]
        public void PictureTags(string expected, string srcset, bool inPicTag, string name)
            => PictureTagInner(expected, srcset, inPicTag, name);

    }
}
