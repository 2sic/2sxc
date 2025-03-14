using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Tests.DataTests;

namespace ToSic.Sxc.DataTests.DynStack;

public class Startup: StartupSxcCoreOnly
{
    public override void ConfigureServices(IServiceCollection services) =>
        base.ConfigureServices(services.AddTransient<DynAndTypedTestHelper>());
}