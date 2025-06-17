using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcAppsStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcApps(this IServiceCollection services)
    {
        // App Dependencies
        services.TryAddTransient<SxcAppBase.MyServices>();

        // Configuration objects
        services.TryAddTransient<GlobalPaths>();

        services.TryAddTransient<App>();

        services.TryAddTransient<IAppTyped, AppTyped>();
        services.TryAddTransient(typeof(IAppTyped<,>), typeof(AppTyped<,>));    // new v17

        services.TryAddTransient<AppFolderInitializer>();

        services.TryAddTransient<AppIconHelpers>();

        // Runtimes - new: better architecture v16.07+
        //services.TryAddTransient<WorkBlocks>();
        //services.TryAddTransient<WorkBlocksMod>();
        services.TryAddTransient<WorkViewsMod>();
        services.TryAddTransient<WorkViews>();
        services.TryAddTransient<WorkApps>();
        services.TryAddTransient<WorkAppsRemove>();

        // Simple DataController - registration was missing
        services.TryAddTransient<SimpleDataEditService>();

        return services;
    }

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcAppsFallbackServices(this IServiceCollection services)
    {
        services.TryAddTransient<IAppDataConfigProvider, AppDataConfigProviderUnknown>();

        return services;
    }

}