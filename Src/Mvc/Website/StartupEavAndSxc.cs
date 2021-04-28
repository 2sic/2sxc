using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Sxc;
using ToSic.Sxc.Mvc;
using ToSic.Sxc.Razor;
using ToSic.Sxc.WebApi;

namespace Website
{
    public class StartupEavAndSxc
    {
        internal static void ConfigureIoC(IServiceCollection services, IConfiguration configuration)
        {
            ToSic.Eav.Factory.UseExistingServices(services);
            ToSic.Eav.Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcMvc()
                    .AddNotImplemented()
                    .AddSxcRazor()

                    // probably the basics needed to read data
                    .AddAdamWebApi<string, string>()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });

            var connectionString = configuration.GetConnectionString("SiteSqlServer");
            services.BuildServiceProvider().Build<IDbConfiguration>().ConnectionString = connectionString;
        }
    }
}
