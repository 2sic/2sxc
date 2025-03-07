using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Eav.Testing;
using ToSic.Testing.Shared;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ConfigurationTests;

[TestClass]
public class VerifyScenarioNoPatronsPerfectionist(): TestBaseSxcDb(EavTestConfig.ScenarioBasic)
{
    // Start the test with a platform-info that has a patron
    protected override IServiceCollection SetupServices(IServiceCollection services) =>
        base.SetupServices(services)
            .AddTransient<IPlatformInfo, TestPlatformNotPatron>();

    // Our current test only has 3 auto-enabled packages, so the service should report so many active licenses
    [TestMethod] public void VerifyPackageOk() => new VerifyPatronsHelper(this).VerifyPackageOk(3 /* auto-enabled only */);

    [TestMethod] public void VerifyPatronPerfectionistsActive() => new VerifyPatronsHelper(this).VerifyPatronPerfectionistsActive(false);

    [TestMethod] public void VerifyImageFormats() => new VerifyPatronsHelper(this).VerifyImageFormats(false);
}
