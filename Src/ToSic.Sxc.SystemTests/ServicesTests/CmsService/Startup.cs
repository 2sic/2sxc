using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Mocks;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class Startup: StartupSxcWithDb
{
    public override void ConfigureServices(IServiceCollection services)
    {
        //services.TryAddTransient<IValueConverter, MockValueConverter>();
        base.ConfigureServices(services);
    }

}