using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcAppsStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcApps(this IServiceCollection services)
    {
        services.TryAddTransient<App>();

        services.TryAddTransient<IAppTyped, AppTyped>();
        services.TryAddTransient(typeof(IAppTyped<,>), typeof(AppTyped<,>));    // new v17

        services.TryAddTransient<AppFolderInitializer>();


        return services;
    }


        
}