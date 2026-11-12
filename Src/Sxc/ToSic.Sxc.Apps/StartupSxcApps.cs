using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Api01;
using ToSic.Sxc.Apps.Sys.AppDynamic;
using ToSic.Sxc.Apps.Sys.AppTyped;
using ToSic.Sxc.Apps.Sys.Work;
using ToSic.Sxc.Services.DataService.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcApps
{

    public static IServiceCollection AddSxcApps(this IServiceCollection services)
    {
        // Configuration objects
        services.TryAddTransient<GlobalPaths>();

        services.TryAddTransient<AppForCode>();  // new v22
        services.TryAddTransient<AppForCodeDataSourceService>(); // new v22
        services.TryAddTransient<IAppDynamic, AppDynamic>();

        services.TryAddTransient<IAppTyped, AppTyped>();
        services.TryAddTransient(typeof(IAppTyped<,>), typeof(AppTyped<,>));    // new v17

        services.TryAddTransient<AppDynamicQueries>();
        services.TryAddTransient(typeof(QueryBuilder<>));   // new v20
        services.TryAddTransient(typeof(GetQueryMs<>));     // new v22

        services.TryAddTransient<AppFolderInitializer>();

        services.TryAddTransient<AppIconHelpers>();

        // Runtimes - new: better architecture v16.07+
        services.TryAddTransient<WorkViewDelete>();
        services.TryAddTransient<WorkViews>();
        services.TryAddTransient<WorkApps>();

        services.TryAddTransient<WorkViewsContentTypes>(); // new v22
        
        // Simple DataController - registration was missing
        services.TryAddTransient<SimpleDataEditService>();

        services.TryAddTransient<AppDataCrud>();    // new v22

        return services;
    }

    public static IServiceCollection AddSxcAppsFallbacks(this IServiceCollection services)
    {
        services.TryAddTransient<IAppDataConfigProvider, AppDataConfigProviderUnknown>();

        return services;
    }

}