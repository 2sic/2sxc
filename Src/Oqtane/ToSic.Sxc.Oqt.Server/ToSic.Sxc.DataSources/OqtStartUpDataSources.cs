using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    internal static class OqtStartUpDataSources
    {
        public static IServiceCollection AddOqtSxcDataSources(this IServiceCollection services)
        {
            services.TryAddTransient<Roles, OqtRoles>();
            services.TryAddTransient<Users, OqtUsers>();
            return services;
        }
    }
}
