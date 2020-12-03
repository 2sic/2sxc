using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;

namespace IntegrationSamples.ReadOnly.Integration
{
    public static class StartupEavAndSxc
    {
        internal static void ConfigureConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SiteSqlServer");
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }

        internal static void AddEavAndSxcIntegration(this IServiceCollection services)
        {
            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2.AddEav();
            });
        }

    }
}
