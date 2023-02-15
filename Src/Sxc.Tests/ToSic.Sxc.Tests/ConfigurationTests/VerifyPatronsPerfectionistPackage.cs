using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Configuration;
using ToSic.Eav.Configuration.Licenses;
using ToSic.Eav.Run;
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
            var licenses = Build<ILicenseService>();
            var result = licenses.Enabled;

            // Our current test enables 6 packages, so the service should report so many active licenses
            Assert.AreEqual(6, result.Count, "X license package should be enabled. If the number changes, this test may need update.");
        }

        [TestMethod]
        public void VerifyPatronPerfectionistsActive()
        {
            var licenses = Build<ILicenseService>();
            var result = licenses.IsEnabled(BuiltInLicenses.PatronPerfectionist);

            // Our current test enables 6 packages, so the service should report so many active licenses
            Assert.IsTrue(result, "Patron Perfectionist should be enabled");
        }

        [TestMethod]
        public void VerifyImageFormats()
        {
            var features = Build<IFeaturesInternal>();
            var result = features.IsEnabled(Configuration.Features.BuiltInFeatures.ImageServiceMultiFormat);

            // Our current test enables 6 packages, so the service should report so many active licenses
            Assert.IsTrue(result, "Patron Perfectionist should be enabled");
        }

    }
}
