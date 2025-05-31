using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Integration;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Internal.Plumbing;
using ToSic.Sxc.Startup;
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
        services.TryAddTransient<SxcImportExportEnvironmentBase.MyServices>();

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
        services.TryAddTransient<IEnvironmentInstaller, BasicEnvironmentInstaller>();

        //// ADAM basics
        //// TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        //services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

        // v13.02
        services.TryAddTransient<ILinkPaths, LinkPathsUnknown>();
        //services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();

        //// Koi, mainly so tests don't fail
        //services.TryAddTransient<ICssFrameworkDetector, CssFrameworkDetectorUnknown>();

        return services;
    }

}