using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataSources.Internal;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Dnn.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal static class DnnStartUpDataSources
{
    public static IServiceCollection AddDnnSxcDataSources(this IServiceCollection services)
    {
        // DataSourceProvider model
        services.TryAddTransient<RolesDataSourceProvider, DnnRolesDsProvider>();
        services.TryAddTransient<UsersDataSourceProvider, DnnUsersDsProvider>();

        // info class to ensure SQL knows about default connections
        services.TryAddTransient<SqlPlatformInfo, DnnSqlPlatformInfo>();

        // General data sources
        services.TryAddTransient<DnnSql>();
        services.TryAddTransient<DnnUserProfile>();
        services.TryAddTransient<DnnUserProfile.MyServices>();

        services.TryAddTransient<PagesDataSourceProvider, DnnPagesDsProvider>();
        services.TryAddTransient<SitesDataSourceProvider, DnnSitesDsProvider>();

        return services;
    }
    public static IServiceCollection AddDnnDataSources(this IServiceCollection services)
    {
        // General data sources
        services.TryAddTransient<DnnSql>();
        services.TryAddTransient<DnnUserProfile>();
        services.TryAddTransient<DnnUserProfile.MyServices>();

        return services;
    }
}