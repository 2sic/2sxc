using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests;

public class TestBaseSxcDb(EavTestConfig testConfig = default) : TestBaseDiEavFullAndDb(testConfig)
{
    protected override void SetupServices(IServiceCollection services)
    {
        base.SetupServices(services);
        services
            .AddSxcCore();
    }

}