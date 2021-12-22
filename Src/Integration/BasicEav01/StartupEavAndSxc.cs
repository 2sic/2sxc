using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;

namespace IntegrationSamples.BasicEav01
{
    /// <summary>
    /// #2sxcIntegration
    /// This static class contains the important bits to connect the EAV with the Dependency Injection of this test site
    /// </summary>
    public static class StartupEavAndSxc
    {
        internal static void AddEavAndSxcIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            //Factory.UseExistingServices(services);
            //Factory.ActivateNetCoreDi(services2 =>
            //{
                services.AddEav();
            //});

            var connectionString = configuration.GetConnectionString("SiteSqlServer");
            services.BuildServiceProvider().Build<IDbConfiguration>().ConnectionString = connectionString;
        }

    }
}
