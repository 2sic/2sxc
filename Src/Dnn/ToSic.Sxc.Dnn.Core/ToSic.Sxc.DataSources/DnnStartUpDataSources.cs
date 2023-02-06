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
            // TODO: Change to use the DataSourceProvider model
            services.TryAddTransient<Roles, DnnRoles>();
            services.TryAddTransient<UsersDataSourceProvider, DnnUsersDsProvider>();

            // info class to ensure SQL knows about default connections
            services.TryAddTransient<SqlPlatformInfo, DnnSqlPlatformInfo>();

            // General data sources
            services.TryAddTransient<DnnSql>();
            services.TryAddTransient<DnnUserProfile>();
            services.TryAddTransient<DnnUserProfile.Dependencies>();

            services.TryAddTransient<PagesDataSourceProvider, DnnPagesDsProvider>();

            return services;
        }
        public static IServiceCollection AddDnnDataSources(this IServiceCollection services)
        {
            // General data sources
            services.TryAddTransient<DnnSql>();
            services.TryAddTransient<DnnUserProfile>();
            services.TryAddTransient<DnnUserProfile.Dependencies>();

            return services;
        }
    }
}
