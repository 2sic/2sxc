using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.ConfigurationTests;

public class StartupSxcWithDbPatronPerfectionist : StartupSxcWithDb
{
    public override void ConfigureServices(IServiceCollection services) =>
        base.ConfigureServices(services
            //.AddTransient<VerifyPatronsHelper>()
            .AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>()
        );
}