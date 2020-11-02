using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc
{
    public static class StartupSxc
    {
        public static IServiceCollection AddSxcCore(this IServiceCollection services)
        {
            services.TryAddTransient<CmsRuntime>();
            services.TryAddTransient<CmsManager>();

            return services;
        }
    }
}