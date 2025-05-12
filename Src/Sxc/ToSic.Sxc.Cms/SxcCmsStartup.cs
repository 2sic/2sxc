using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Cms.Users.Internal;
using ToSic.Sxc.DataSources.Internal;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCmsStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCms(this IServiceCollection services)
    {
        // v15 DataSource
        services.TryAddTransient<PagesDataSourceProvider, PagesDataSourceProviderUnknown>();
        services.TryAddTransient<IUsersProvider, UsersProviderUnknown>();
        services.TryAddTransient<IUserRolesProvider, UserRolesProviderUnknown>();
        services.TryAddTransient<SitesDataSourceProvider.MyServices>();
        services.TryAddTransient<SitesDataSourceProvider, SitesDataSourceProviderUnknown>();
        

        return services;
    }


        
}