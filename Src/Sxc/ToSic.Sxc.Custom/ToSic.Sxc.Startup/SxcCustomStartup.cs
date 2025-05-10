using Custom.DataSource;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataSource.Internal.AppDataSources;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCustomStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCustom(this IServiceCollection services)
    {
        // Loader of AppDataSources
        services.TryAddTransient<IAppDataSourcesLoader, AppDataSourcesLoader>();

        // v15 CustomDataSources - just the dependencies needed
        services.TryAddTransient<DataSource16.MyServices>();

        // Kits for custom code only
        services.TryAddTransient<ServiceKitLight16>();

        return services;
    }


        
}