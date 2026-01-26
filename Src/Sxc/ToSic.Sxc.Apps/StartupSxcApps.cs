using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Api01;
using ToSic.Sxc.Apps.Sys.AppTyped;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Apps.Sys.Work;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcApps
{

    public static IServiceCollection AddSxcApps(this IServiceCollection services)
    {
        // App Dependencies
        services.TryAddTransient<SxcAppBase.Dependencies>();

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

    public static IServiceCollection AddSxcAppsFallbacks(this IServiceCollection services)
    {
        services.TryAddTransient<IAppDataConfigProvider, AppDataConfigProviderUnknown>();

        return services;
    }

}