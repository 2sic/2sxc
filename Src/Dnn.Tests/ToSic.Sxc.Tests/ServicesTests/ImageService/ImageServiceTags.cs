using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTags: ImageServiceTagsBase
    {
        // Start the test with a platform-info that has WebP support
        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformWithLicense>();
        }

        [TestMethod]
        public void SourceTags12() => base.SourceTags12(SrcWebP12 + SrcJpg12);

        [TestMethod]
        public void SourceTagsNone() => base.SourceTagsNone(SrcWebPNone + SrcJpgNone);

        [TestMethod]
        public void PictureTagNoSet() => base.PictureTagNoSet(SrcWebPNone + SrcJpgNone);

        [TestMethod]
        public void PictureTag12() => base.PictureTag12(SrcWebP12 + SrcJpg12);
    }
}
