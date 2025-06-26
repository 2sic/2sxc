using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.ServicesTests.Mail;

public class Startup: StartupSxcCoreOnly
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services
            .AddTransient<MailServiceTestsHelper>());
    }
}