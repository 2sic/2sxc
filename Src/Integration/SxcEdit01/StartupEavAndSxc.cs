using IntegrationSamples.SxcEdit01.Context;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using ToSic.Eav.Context;
using ToSic.Sxc.Adam;

namespace IntegrationSamples.SxcEdit01
{
    /// <summary>
    /// #2sxcIntegration
    /// This static class contains the important bits to connect the EAV with the Dependency Injection of this test site
    /// </summary>
    public static class StartupEavAndSxc
    {
        internal static IServiceCollection AddImplementations(this IServiceCollection services)
        {
            // Context
            services.TryAddTransient<ISite, IntSite>();
            services.TryAddTransient<IUser, IntUserSuper>();

            // ADAM
            services.TryAddTransient<IAdamPaths, AdamPathsWwwroot>();

            return services;
        }

        internal static IServiceCollection AddMvcRazor(this IServiceCollection services)
        {
            // enable use of HttpContext
            services.AddHttpContextAccessor();

            // enable use of UrlHelper for AbsolutePath
            // used by LinkPath - but this is only for older .net
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddScoped(it => it.GetService<IUrlHelperFactory>()
                .GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

            return services;
        }

        internal static IServiceCollection AddControllersAndConfigureJson(this IServiceCollection services)
        {
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
