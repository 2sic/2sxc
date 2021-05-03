using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    // ReSharper disable once InconsistentNaming
    static partial class OqtaneDI
    {
        public static IServiceCollection AddAppApi(this IServiceCollection services)
        {
            services.AddSingleton<IActionDescriptorChangeProvider>(AppApiActionDescriptorChangeProvider.Instance);
            services.AddSingleton(AppApiActionDescriptorChangeProvider.Instance);
            services.AddSingleton<AppApiFileSystemWatcher>();
            services.AddScoped<AppApiDynamicRouteValueTransformer>();
            services.AddScoped<AppApiControllerManager>();
            services.AddScoped<AppApiActionInvoker>();

            return services;
        }
    }
}