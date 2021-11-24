using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.StartUp
{
    public class StartupDi : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ToSic.Eav.Factory.UseExistingServices(services);
            //StartupDnn.DiRegister();

            var appsCache = StartupDnn.GetAppsCacheOverride();
            services.AddDnn(appsCache);
            services.AddAdamWebApi<int, int>();
            services.AddSxcWebApi();
            services.AddSxcCore();
            services.AddEav();

            // temp polymorphism - later put into AddPolymorphism
            services.TryAddTransient<Koi>();
            services.TryAddTransient<Permissions>();
        }
    }
}