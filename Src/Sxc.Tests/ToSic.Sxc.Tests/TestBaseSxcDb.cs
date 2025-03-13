using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests;

public class TestBaseSxcDb(TestScenario testScenario = default) : TestBaseDiEavFullAndDb(testScenario)
{
    protected override IServiceCollection SetupServices(IServiceCollection services) =>
        base.SetupServices(services)
            .AddSxcCore();
}