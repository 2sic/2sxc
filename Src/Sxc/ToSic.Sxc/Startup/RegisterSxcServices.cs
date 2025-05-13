using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.StartUp;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Integration;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Internal.Plumbing;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static partial class RegisterSxcServices
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCore(this IServiceCollection services)
    {

        // Configuration Provider WIP
        //services.TryAddTransient<IAppDataConfigProvider, SxcAppDataConfigProvider>(); // new v17
        //services.TryAddTransient<App>();
        services.TryAddTransient<SxcImportExportEnvironmentBase.MyServices>();

        // Context stuff for the page (not EAV)
        //services.TryAddTransient<IPage, Page>();
        //services.TryAddTransient<Page>();


        // Adam stuff
        services.TryAddTransient<AdamSecurityChecksBase, AdamSecurityChecksBasic>();
        services.TryAddTransient<IAdamPaths, AdamPathsBase>();
        services.TryAddTransient<AdamConfiguration>();

        services.AddTransient<AdamManager.MyServices>();

        //// WIP - add net-core specific stuff
        //services.AddNetVariations();

        //// Polymorphism
        //services.TryAddTransient<Polymorphism.Internal.PolymorphConfigReader>();


        //// 2022-02-07 2dm experimental
        //// The PageServiceShared must always be generated from the PageScope
        //// I previously thought the PageServiceShared must be scoped at page level, but this is wrong
        //// Reason is that it seems to collect specs per module, and then actually only flushes it
        //// Because it shouldn't remain in the list for the second module
        //// So it actually looks like it's very module-scoped already, but had workarounds for it.
        //// So I think it really doesn't need to be have workarounds for it
        //services.TryAddScoped<PageServiceShared>();
        //services.TryAddTransient<PageChangeSummary>();


        // Page Features
        //services.TryAddTransient<IPageFeatures, PageFeatures>();
        //services.TryAddTransient<IPageFeaturesManager, PageFeaturesManager>();
        //services.TryAddSingleton<PageFeaturesCatalog>();

        //// new in v12.02/12.04 Image Link Resize Helper
        //services.TryAddTransient<ImgResizeLinker>();

        // WIP - objects which are not really final
        services.TryAddTransient<RemoteRouterLink>();


        // 12.06.01 moved here from WebApi, but it should probably be in Dnn as it's probably just used there
        services.TryAddTransient<IServerPaths, ServerPaths>();

            
        // 13 - cleaning up handling of app paths
        //services.TryAddTransient<AppFolderInitializer>();
        //services.TryAddTransient<AppIconHelpers>();

        // v13 Provide page scoped services
        // This is important, as most services are module scoped, but very few are actually scoped one level higher
        services.TryAddScoped<PageScopeAccessor>();
        services.TryAddScoped(typeof(PageScopedService<>));


        //// v13 LightSpeed
        //services.TryAddTransient<IOutputCache, LightSpeed>();

        //services.TryAddTransient<BlockEditorSelector>();

        // Sxc StartUp Routines - MUST be AddTransient, not TryAddTransient so many start-ups can be registered
        services.AddTransient<IStartUpRegistrations, SxcStartUpRegistrations>();

        //// v15 EditUi Resources
        //services.TryAddTransient<EditUiResources>();

        // v15
        services.TryAddTransient<CodeCreateDataSourceSvc>();

        // v16 DynamicJacket and DynamicRead factories
        //services.TryAddTransient<ICodeDataPoCoWrapperService, CodeDataPoCoWrapperService>();
        //services.TryAddTransient<CodeJsonWrapper>();
        //services.TryAddTransient<WrapObjectTyped>();
        //services.TryAddTransient<WrapObjectTypedItem>();

        // Polymorphism - moved here v17.08
        services.AddTransient<IPolymorphismResolver, PolymorphismKoi>();
        services.AddTransient<IPolymorphismResolver, PolymorphismPermissions>();


        // Add possibly missing fallback services
        // This must always be at the end here so it doesn't accidentally replace something we actually need
        services.AddSxcCoreLookUps();
        //services.AddServicesAndKits();
        //services.ExternalConfig();
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

}