using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.ServicesTests.Templates;

public class Startup: StartupSxcCoreOnly
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services
            .AddTransient<TemplatesTestsBaseHelper>());
    }
}