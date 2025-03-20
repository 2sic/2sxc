using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc;

public class StartupSxcWithDbPatronPerfectionist : StartupSxcWithDb
{
    public override void ConfigureServices(IServiceCollection services) =>
        base.ConfigureServices(
            services
                .AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>()
        );
}