using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;

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


        // Runtimes - new: better architecture v16.07+
        services.TryAddTransient<WorkBlocks>();
        services.TryAddTransient<WorkViews>();
        services.TryAddTransient<WorkViewsMod>();
        services.TryAddTransient<WorkBlocksMod>();
        services.TryAddTransient<WorkApps>();
        services.TryAddTransient<WorkAppsRemove>();


        return services;
    }


        
}