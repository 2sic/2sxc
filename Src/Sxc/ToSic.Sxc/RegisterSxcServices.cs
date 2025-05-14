using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam.Internal;
//using ToSic.Sxc.Integration;
//using ToSic.Sxc.Integration.Installation;
//using ToSic.Sxc.Integration.Paths;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static partial class RegisterSxcServices
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCore(this IServiceCollection services)
    {
        //// Configuration Provider WIP
        //services.TryAddTransient<SxcImportExportEnvironmentBase.MyServices>();


        // Adam stuff
        services.TryAddTransient<AdamSecurityChecksBase, AdamSecurityChecksBasic>();
        services.TryAddTransient<IAdamPaths, AdamPathsBase>();
        services.TryAddTransient<AdamConfiguration>();

        services.AddTransient<AdamManager.MyServices>();


        //// WIP - objects which are not really final
        //services.TryAddTransient<RemoteRouterLink>();


        //// 12.06.01 moved here from WebApi, but it should probably be in Dnn as it's probably just used there
        //services.TryAddTransient<IServerPaths, ServerPaths>();


        //// v13 Provide page scoped services
        //// This is important, as most services are module scoped, but very few are actually scoped one level higher
        //services.TryAddScoped<PageScopeAccessor>();
        //services.TryAddScoped(typeof(PageScopedService<>));

        //// Sxc StartUp Routines - MUST be AddTransient, not TryAddTransient so many start-ups can be registered
        //services.AddTransient<IStartUpRegistrations, SxcStartUpRegistrations>();

        //// Polymorphism - moved here v17.08
        //services.AddTransient<IPolymorphismResolver, PolymorphismKoi>();
        //services.AddTransient<IPolymorphismResolver, PolymorphismPermissions>();


        // Add possibly missing fallback services
        // This must always be at the end here so it doesn't accidentally replace something we actually need
        services.AddKoi();
        services.AddSxcCoreFallbackServices();

        return services;
    }



    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddKoi(this IServiceCollection services)
    {
        services.TryAddTransient<Connect.Koi.KoiCss.Dependencies>();
        services.TryAddTransient<Connect.Koi.ICss, Connect.Koi.KoiCss>();

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
        //// basic environment, pages, modules etc.
        //services.TryAddTransient<IEnvironmentInstaller, BasicEnvironmentInstaller>();
        //services.TryAddTransient<IPlatformAppInstaller, BasicEnvironmentInstaller>();

        // ADAM basics
        // TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

        // v13.02
        //services.TryAddTransient<ILinkPaths, LinkPathsUnknown>();
        //services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();

        //// Koi, mainly so tests don't fail
        //services.TryAddTransient<ICssFrameworkDetector, CssFrameworkDetectorUnknown>();

        return services;
    }
}