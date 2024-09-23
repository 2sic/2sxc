using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataSources.Internal;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal static class OqtStartUpDataSources
{
    public static IServiceCollection AddOqtSxcDataSources(this IServiceCollection services)
    {
        // DataSourceProvider model
        services.TryAddTransient<RolesDataSourceProvider, OqtRolesDsProvider>();
        services.TryAddTransient<UsersDataSourceProvider, OqtUsersDsProvider>();
        services.TryAddTransient<SitesDataSourceProvider, OqtSitesDsProvider>();
        services.TryAddTransient<PagesDataSourceProvider, OqtPagesDsProvider>();

        // info class to ensure SQL knows about default connections
        services.TryAddTransient<SqlPlatformInfo, OqtSqlPlatformInfo>();

        return services;
    }
}