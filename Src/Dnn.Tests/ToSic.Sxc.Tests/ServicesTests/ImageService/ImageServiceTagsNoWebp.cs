using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTagsNoWebp : ImageServiceTagsBase
    {
        // Start the test with a platform-info that has WebP support
        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformNoLicense>();
        }

        [TestMethod]
        public void SourceTags12() => base.SourceTags12(SrcJpg12);

        [TestMethod]
        public void SourceTagsNone() => base.SourceTagsNone(SrcJpgNone);

        [TestMethod]
        public void PictureTagNoSet() => base.PictureTagNoSet(SrcJpgNone);

        [TestMethod]
        public void PictureTag12() => base.PictureTag12(SrcJpg12);
    }
}
