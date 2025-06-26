using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.LinksAndImages.LinkImageTests;

public class Startup: StartupSxcCoreOnly
{
    public override void ConfigureServices(IServiceCollection services) =>
        base.ConfigureServices(services.AddTransient<LinkImageTestHelper>());
}