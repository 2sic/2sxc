using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Sxc;
using ToSic.Sxc.Mvc.Plumbing;
using ToSic.Sxc.Razor;
using ToSic.Sxc.Razor.Engine;
using ToSic.Sxc.WebApi.Plumbing;

namespace Website.Plumbing
{
    public class EavConfiguration
    {
        internal void ConfigureConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SiteSqlServer");
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }

        internal static void ConfigureIoC(IServiceCollection services)
        {
            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcMvc()
                    .AddSxcRazor()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });
        }
    }
}
