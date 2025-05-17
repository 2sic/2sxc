using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.ConfigurationTests;

public class VerifyScenarioPatronsPerfectionist(VerifyPatronsHelper patronsHelper)
    : IClassFixture<DoFixtureStartup<ScenarioFullPatrons>>
{
    // Start the test with a platform-info that has a patron
    public class Startup : StartupSxcWithDbPatronPerfectionist
    {
        public override void ConfigureServices(IServiceCollection services) =>
            base.ConfigureServices(services
                .AddTransient<VerifyPatronsHelper>()
                //.AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>()
            );
    }

    // Our current test enables 6 packages, so the service should report so many active licenses
    [Fact] public void VerifyPackageOk() =>
        patronsHelper
            .VerifyPackageOk(16);

    [Fact] public void VerifyPatronPerfectionistsActive() =>
        patronsHelper
            .VerifyPatronPerfectionistsActive(true);

    [Fact] public void VerifyImageFormats() =>
        patronsHelper
            .VerifyImageFormats(true);
}