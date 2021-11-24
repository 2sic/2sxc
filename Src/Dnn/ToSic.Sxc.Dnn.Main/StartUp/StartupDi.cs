using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Dnn.StartUp
{
    public class StartupDi : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ToSic.Eav.Factory.UseExistingServices(services);
            StartupDnn.DiRegister();
        }
    }
}