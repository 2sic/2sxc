using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.ConfigurationTests;

public class VerifyScenarioNoPatronsPerfectionist(VerifyPatronsHelper patronsHelper) : IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    // Start the test with a platform-info that has a patron
    public class Startup : StartupSxcWithDbBasic
    {
        public override void ConfigureServices(IServiceCollection services) =>
            base.ConfigureServices(
                services
                    .AddTransient<VerifyPatronsHelper>()
                    //.AddTransient<IPlatformInfo, TestPlatformNotPatron>()
            );
    }

    // Our current test only has 3 auto-enabled packages, so the service should report so many active licenses
    [Fact] public void VerifyPackageOk() =>
        patronsHelper.VerifyPackageOk(3 /* auto-enabled only */);

    [Fact] public void VerifyPatronPerfectionistsActive() =>
        patronsHelper.VerifyPatronPerfectionistsActive(false);

    [Fact] public void VerifyImageFormats() =>
        patronsHelper
            .VerifyImageFormats(false);
}
