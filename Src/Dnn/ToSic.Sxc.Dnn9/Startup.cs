using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Dnn9
{
    public class Startup : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ToSic.Eav.Factory.UseExistingServices(services);
            ToSic.Sxc.Dnn.StartUp.StartupDnn.DiRegister(); // service configuration for DNN9
        }
    }
}