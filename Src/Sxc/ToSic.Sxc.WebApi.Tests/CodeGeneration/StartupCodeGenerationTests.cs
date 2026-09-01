using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Run.Startup;
using ToSic.Sxc.Run.Startup;

namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

public class StartupCodeGenerationTests : StartupTestsEavDataBuild
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services
            .AddEavApps()
            .AddSxcCodeGen();
    }
}