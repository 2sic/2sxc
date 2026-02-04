using Custom.DataSource;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataSource.Sys.AppDataSources;
using ToSic.Sxc.DataSources.Sys;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcCustom
{
    public static IServiceCollection AddSxcCustom(this IServiceCollection services)
    {
        // Loader of AppDataSources
        services.TryAddTransient<IAppDataSourcesLoader, AppDataSourcesLoader>();

        // v15 CustomDataSources - just the dependencies needed
        services.TryAddTransient<DataSource16.Dependencies>();
        services.TryAddTransient<DataSource16.MyServices>();    // old name for compatibility

        // Kits for custom code only
        services.TryAddTransient<ServiceKitLight16>();

        return services;
    }


        
}