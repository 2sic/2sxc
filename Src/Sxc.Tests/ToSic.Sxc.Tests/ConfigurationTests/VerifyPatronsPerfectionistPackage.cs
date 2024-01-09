using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Licenses;
using ToSic.Eav.Run;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ConfigurationTests
{
    [TestClass]
    public class VerifyPatronsPerfectionistPackage: TestBaseSxcDb
    {
        // Start the test with a platform-info that has a patron
        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();
        }

        [TestMethod]
        public void VerifyPackageOk()
        {
            var licenses = GetService<ILicenseService>();
            var result = licenses.Enabled;

            // Our current test enables 6 packages, so the service should report so many active licenses
            const int testLicCount = 13;
            Assert.AreEqual(testLicCount, result.Count, $"{testLicCount} license package should be enabled. If the number changes, this test may need update.");
        }

        [TestMethod]
        public void VerifyPatronPerfectionistsActive()
        {
            var licenses = GetService<ILicenseService>();
            var result = licenses.IsEnabled(BuiltInLicenses.PatronPerfectionist);

            // Our current test enables 6 packages, so the service should report so many active licenses
            Assert.IsTrue(result, "Patron Perfectionist should be enabled");
        }

        [TestMethod]
        public void VerifyImageFormats()
        {
            var features = GetService<IEavFeaturesService>();
            var result = features.IsEnabled(SxcFeatures.ImageServiceMultiFormat);

            // Our current test enables 6 packages, so the service should report so many active licenses
            Assert.IsTrue(result, "Patron Perfectionist should be enabled");
        }

    }
}
