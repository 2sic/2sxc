using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Compatibility
{
    public static class StartUpCompatibility
    {
        /// <summary>
        /// Add obsolete interfaces which had previously been supported
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDnnCompatibility(this IServiceCollection services)
        {
            services.TryAddTransient<ILogService, DnnSystemLogService>();
            return services;
        }
    }
}
