using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Tests.DataTests;

namespace ToSic.Sxc.DataTests.DynWrappers;

public class Startup: StartupSxcCoreOnly
{
    public override void ConfigureServices(IServiceCollection services) =>
        base.ConfigureServices(services.AddTransient<DynAndTypedTestHelper>());
}