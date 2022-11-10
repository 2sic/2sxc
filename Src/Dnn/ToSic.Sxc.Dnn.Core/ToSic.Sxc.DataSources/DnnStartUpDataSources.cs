using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Dnn.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    public static class DnnStartUpDataSources
    {
        public static IServiceCollection AddDnnSxcDataSources(this IServiceCollection services)
        {
            services.TryAddTransient<Pages, DnnPages>();
            services.TryAddTransient<Roles, DnnRoles>();
            services.TryAddTransient<Users, DnnUsers>();

            // info class to ensure SQL knows about default connections
            services.TryAddTransient<SqlPlatformInfo, DnnSqlPlatformInfo>();

            return services;
        }
    }
}
