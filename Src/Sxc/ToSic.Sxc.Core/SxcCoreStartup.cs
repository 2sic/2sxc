using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.Integration;
using ToSic.Sxc.Sys.Integration.Installation;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sys.Boot;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCoreStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCoreNew(this IServiceCollection services)
    {
        services.TryAddScoped<ILinkPaths, LinkPaths>();

        // Configuration Provider WIP
        services.TryAddTransient<SxcImportExportEnvironmentBase.Dependencies>();

        // Sxc StartUp Routines - MUST be AddTransient, not TryAddTransient so many start-ups can be registered
        // Add StartUp Registration of FeaturesCatalog
        services.AddTransient<IBootProcess, SxcBootFeaturesRegistrations>();

        // v13 Provide page scoped services
        // This is important, as most services are module scoped, but very few are actually scoped one level higher
        services.TryAddScoped<PageScopeAccessor>();
        services.TryAddScoped(typeof(PageScopedService<>));

        // 12.06.01 moved here from WebApi, but it should probably be in Dnn as it's probably just used there
        services.TryAddTransient<IServerPaths, ServerPaths>();

        return services;
    }

    /// <summary>
    /// This will add Do-Nothing services which will take over if they are not provided by the main system
    /// In general this will result in some features missing, which many platforms don't need or care about
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <remarks>
    /// All calls in here MUST use TryAddTransient, and never without the Try
    /// </remarks>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCoreFallbackServices(this IServiceCollection services)
    {
        // basic environment, pages, modules etc.
        services.TryAddTransient<IEnvironmentInstaller, EnvironmentInstallerUnknown>();

        //// ADAM basics
        //// TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        //services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

        // v13.02
        services.TryAddTransient<ILinkPaths, LinkPathsUnknown>();

        return services;
    }

}