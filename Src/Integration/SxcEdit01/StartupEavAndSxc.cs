using System.IO;
using IntegrationSamples.SxcEdit01.Adam;
using IntegrationSamples.SxcEdit01.Controllers;
using IntegrationSamples.SxcEdit01.Integration;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using ToSic.Eav;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Sxc;
using ToSic.Sxc.Adam;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;

namespace IntegrationSamples.SxcEdit01
{
    /// <summary>
    /// #2sxcIntegration
    /// This static class contains the important bits to connect the EAV with the Dependency Injection of this test site
    /// </summary>
    public static class StartupEavAndSxc
    {
        internal static void AddEavAndSxcIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .MvcSystemParts()
                    .AddAdam()
                    .AddImplementations()
                    .AddAdamWebApi<string, string>()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });

            var sp = services.BuildServiceProvider();
            var connectionString = configuration.GetConnectionString("SiteSqlServer");

            sp.Build<IDbConfiguration>().ConnectionString = connectionString;
            var hostingEnvironment = sp.Build<IHostEnvironment>();
            var globalConfig = sp.Build<IGlobalConfiguration>();
            globalConfig.GlobalFolder = Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\system\\sxc");
            globalConfig.GlobalSiteFolder = "todo - global apps not implemented yet";
        }

        internal static IServiceCollection AddImplementations(this IServiceCollection services)
        {
            services.TryAddTransient<IUser, IntUserSuper>();
            services.TryAddTransient<AppPermissionCheck, IntAppPermissionCheck>();
            services.TryAddTransient<IntStatefulControllerBase.Dependencies>();
            return services;
        }
        internal static IServiceCollection AddAdam(this IServiceCollection services)
        {
            // ADAM stuff
            services.TryAddTransient<ISite, IntSite>();
            services.TryAddTransient<IAdamPaths, AdamPathsWwwroot>();
            return services;
        }

        internal static IServiceCollection MvcSystemParts(this IServiceCollection services)
        {
            // enable use of HttpContext
            services.AddHttpContextAccessor();

            // enable use of UrlHelper for AbsolutePath
            // used by LinkPath - but this is only for older .net
            // don't use in .net 5 etc. (better check the Oqtane sample)
            // with the replaced ILinkPaths
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddScoped(it => it.GetService<IUrlHelperFactory>()
                .GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                // This is needed to preserve compatibility with previous api usage
                .AddNewtonsoftJson(options =>
                {
                    // this ensures that c# objects with Pascal-case keep that
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    ToSic.Eav.ImportExport.Json.JsonSettings.Defaults(options.SerializerSettings); // make sure dates are handled as we need them
                });


            return services;
        }

    }
}
