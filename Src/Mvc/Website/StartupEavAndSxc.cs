using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Sxc;
using ToSic.Sxc.Mvc;
using ToSic.Sxc.Razor.Engine;
using ToSic.Sxc.WebApi;

namespace Website
{
    public class StartupEavAndSxc
    {
        internal static void ConfigureConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SiteSqlServer");
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }

        internal static void ConfigureIoC(IServiceCollection services)
        {
            ToSic.Eav.Factory.UseExistingServices(services);
            ToSic.Eav.Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcMvc()
                    .AddNotImplemented()
                    .AddSxcRazor()

                    // probably the basics needed to read data
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });
        }
    }
}
