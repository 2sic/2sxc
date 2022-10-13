using IntegrationSamples.SxcEdit01.Context;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using ToSic.Eav.Context;
using ToSic.Eav.Serialization;
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
            services.TryAddTransient<IUser, IntUser>();

            // ADAM
            services.TryAddTransient<AdamManager, AdamManager<string, string>>();
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
                // Set the JSON serializer options
                .AddJsonOptions(options => options.JsonSerializerOptions.SetUnsafeJsonSerializerOptions());

            return services;
        }
    }
}
