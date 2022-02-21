using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Oqt.Server.Services;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    internal static partial class OqtRegisterServices
    {
        /// <summary>
        /// Mail and Logging
        /// </summary>
        private static IServiceCollection AddSxcOqtIntegratedServices(this IServiceCollection services)
        {
            services.TryAddTransient<ILogService, OqtLogService>();
            services.TryAddTransient<IMailService, OqtMailService>();
            return services;
        }
        
    }
}
