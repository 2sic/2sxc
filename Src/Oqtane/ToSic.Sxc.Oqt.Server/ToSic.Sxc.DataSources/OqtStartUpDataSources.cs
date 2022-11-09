using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    internal static class OqtStartUpDataSources
    {
        public static IServiceCollection AddOqtSxcDataSources(this IServiceCollection services)
        {
            services.TryAddTransient<Pages, OqtPages>();
            services.TryAddTransient<Roles, OqtRoles>();
            services.TryAddTransient<Users, OqtUsers>();

            // info class to ensure SQL knows about default connections
            services.TryAddTransient<SqlPlatformInfo, OqtSqlPlatformInfo>();

            return services;
        }
    }
}
