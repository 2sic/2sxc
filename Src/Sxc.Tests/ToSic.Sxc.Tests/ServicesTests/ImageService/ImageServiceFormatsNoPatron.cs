using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceFormatsNoPatron: ImageServiceFormats
    {
        // Start the test with a platform-info that has no patron
        protected override IServiceCollection SetupServices(IServiceCollection services)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformNotPatron>();
        }


        protected override int ExpectedPngFormats => 0;
    }
}
