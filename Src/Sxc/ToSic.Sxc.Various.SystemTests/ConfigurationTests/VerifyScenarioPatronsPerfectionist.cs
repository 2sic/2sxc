using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.ConfigurationTests;

public class VerifyScenarioPatronsPerfectionist(VerifyPatronsHelper patronsHelper)
    : IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
{
    // Start the test with a platform-info that has a patron
    public class Startup : StartupSxcWithDb
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Services just for this test
            services.AddTransient<VerifyPatronsHelper>();

            // Tried to reduce the dependencies, but not successful
            // appears the DB must somehow be loaded for this test to work, not sure why
            //services
            //    .AddFixtureHelpers() // Needed to get the paths etc. for all the config files
            //    .AddSxcCoreNew() // Register all features which we'll test here
            //    .AddAppStateFromFolder()
            //    .AddEavCore() // For the loader and everything it needs
            //    .AddLibFeatSys()
            //    .AddLibCore()
            //    .AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();


            base.ConfigureServices(services);
        }
    }

    // Our current test enables 6 packages, so the service should report so many active licenses
    [Fact] public void VerifyPackageOk() =>
        patronsHelper.VerifyEnabledLicenses(16);

    [Fact] public void VerifyPatronPerfectionistsActive() =>
        patronsHelper.VerifyPatronPerfectionistsActive(true);

    [Fact] public void VerifyImageFormats() =>
        patronsHelper.VerifyImageFormats(true);
}