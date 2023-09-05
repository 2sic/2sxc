using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Oqt.Server.Services;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    internal static partial class OqtRegisterServices
    {
        /// <summary>
        /// Mail, Logging and other services.
        /// </summary>
        private static IServiceCollection AddSxcOqtIntegratedServices(this IServiceCollection services)
        {
            services.TryAddTransient<ISystemLogService, OqtSystemLogService>();
            services.TryAddTransient<IMailService, OqtMailService>();
            services.TryAddTransient<IUserService, OqtUsersService>();
            return services;
        }
    }
}
