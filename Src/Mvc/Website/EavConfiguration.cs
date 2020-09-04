using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Sxc.Mvc.Run;

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
            Factory.BetaUseExistingServiceCollection(services);
            Factory.ActivateNetCoreDi(sc =>
            {
                ConfigureServices.Configure(sc);
                new DependencyInjection().ConfigureNetCoreContainer(sc);
            });
        }
    }
}
