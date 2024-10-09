using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Eav.DataSource.Internal.AppDataSources;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.StartUp;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Customizer;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Integration;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Internal.Plumbing;
using ToSic.Sxc.LookUp.Internal;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Internal.EditUi;
using ToSic.Sxc.Web.Internal.JsContext;
using ToSic.Sxc.Web.Internal.LightSpeed;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Startup;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static partial class RegisterSxcServices
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSxcCore(this IServiceCollection services)
    {
        // Runtimes - new: better architecture v16.07+
        services.TryAddTransient<WorkBlocks>();
        services.TryAddTransient<WorkViews>();
        services.TryAddTransient<WorkViewsMod>();
        services.TryAddTransient<WorkBlocksMod>();
        services.TryAddTransient<WorkApps>();
        services.TryAddTransient<WorkAppsRemove>();

        // Code
        services.TryAddTransient<CodeApiService.MyServices>();

        // Block Editors
        services.TryAddTransient<BlockEditorForEntity>();
        services.TryAddTransient<BlockEditorForModule>();
        services.TryAddTransient<BlockEditorBase.MyServices>();

        // Engine and Rendering
        services.TryAddTransient<EngineFactory>();
        services.TryAddTransient<BlockBuilder>();
        services.TryAddTransient<BlockBuilder.MyServices>();

        // Block functionality
        services.TryAddTransient<BlockDataSourceFactory>();
        services.TryAddTransient<DataSources.CmsBlock.MyServices>(); // new v15
        services.TryAddTransient<BlockFromModule>();
        services.TryAddTransient<BlockFromEntity>();
        services.TryAddTransient<BlockBase.MyServices>();

        // Configuration Provider WIP
        services.TryAddTransient<IAppDataConfigProvider, SxcAppDataConfigProvider>(); // new v17
        services.TryAddTransient<App>();
        services.TryAddTransient<SxcImportExportEnvironmentBase.MyServices>();
        services.TryAddTransient<IAppTyped, AppTyped>();
        services.TryAddTransient(typeof(IAppTyped<,>), typeof(AppTyped<,>));    // new v17
        services.TryAddTransient<ICodeCustomizer, Customizer>();

        // Rendering
        services.TryAddTransient<IRenderingHelper, RenderingHelper>();
        services.TryAddTransient<TokenEngine>();

        // Context stuff in general
        services.TryAddTransient<IContextOfBlock, ContextOfBlock>();

        // Context stuff for the page (not EAV)
        services.TryAddTransient<IPage, Page>();
        services.TryAddTransient<Page>();


        // Context stuff, which is explicitly scoped
        services.TryAddScoped<ISxcContextResolver, SxcContextResolver>();
        // New v15.04 WIP
        services.TryAddScoped<IContextResolver>(x => x.GetRequiredService<ISxcContextResolver>());
        services.TryAddScoped<IContextResolverUserPermissions>(x => x.GetRequiredService<ISxcContextResolver>());
        services.TryAddScoped<AppIdResolver>();


        // JS UI Context
        services.TryAddTransient<JsContextAll>();
        services.TryAddTransient<JsContextLanguage>();
        services.TryAddScoped<JsApiCacheService>(); // v16.01

        // Adam stuff
        services.TryAddTransient<AdamSecurityChecksBase, AdamSecurityChecksBasic>();
        services.TryAddTransient<IAdamPaths, AdamPathsBase>();
        services.TryAddTransient<AdamConfiguration>();

        services.AddTransient<AdamManager.MyServices>();

        // WIP - add net-core specific stuff
        services.AddNetVariations();

        // Polymorphism
        services.TryAddTransient<Polymorphism.Internal.PolymorphConfigReader>();


        // 2022-02-07 2dm experimental
        // The PageServiceShared must always be generated from the PageScope
        // I previously thought the PageServiceShared must be scoped at page level, but this is wrong
        // Reason is that it seems to collect specs per module, and then actually only flushes it
        // Because it shouldn't remain in the list for the second module
        // So it actually looks like it's very module-scoped already, but had workarounds for it.
        // So I think it really doesn't need to be have workarounds for it
        services.TryAddScoped<PageServiceShared>();
        services.TryAddTransient<PageChangeSummary>();

        // CSP
        services.TryAddTransient<CspOfApp>();   // must be transient
        services.TryAddScoped<CspOfModule>();   // important: must be scoped!
        services.TryAddTransient<CspOfPage>();
        services.TryAddTransient<CspParameterFinalizer>();

        // Page Features
        services.TryAddTransient<IPageFeatures, PageFeatures>();
        services.TryAddTransient<IPageFeaturesManager, PageFeaturesManager>();
        services.TryAddSingleton<PageFeaturesCatalog>();

        // new in v12.02/12.04 Image Link Resize Helper
        services.TryAddTransient<ImgResizeLinker>();

        // WIP - objects which are not really final
        services.TryAddTransient<RemoteRouterLink>();


        // 12.06.01 moved here from WebApi, but it should probably be in Dnn as it's probably just used there
        services.TryAddTransient<IServerPaths, ServerPaths>();

            
        // 13 - cleaning up handling of app paths
        services.TryAddTransient<AppFolderInitializer>();
        services.TryAddTransient<AppIconHelpers>();

        // v13 Provide page scoped services
        // This is important, as most services are module scoped, but very few are actually scoped one level higher
        services.TryAddScoped<PageScopeAccessor>();
        services.TryAddScoped(typeof(PageScopedService<>));


        // v13 LightSpeed
        services.TryAddTransient<IOutputCache, LightSpeed>();

        services.TryAddTransient<BlockEditorSelector>();

        // Sxc StartUp Routines - MUST be AddTransient, not TryAddTransient so many start-ups can be registered
        services.AddTransient<IStartUpRegistrations, SxcStartUpRegistrations>();

        // v15 DataSource Dependencies
        services.TryAddTransient<SitesDataSourceProvider.MyServices>();
        services.TryAddTransient<AppAssetsDataSourceProvider>();
        services.TryAddTransient<AppAssetsDataSourceProvider.MyServices>();
        services.TryAddTransient(typeof(AdamDataSourceProvider<,>));
        services.TryAddTransient(typeof(AdamDataSourceProvider<,>.MyServices));
        services.TryAddTransient<IAppDataSourcesLoader, AppDataSourcesLoader>();

        // v15 EditUi Resources
        services.TryAddTransient<EditUiResources>();

        // v15
        services.TryAddTransient<CodeCreateDataSourceSvc>();

        // v16 DynamicJacket and DynamicRead factories
        services.TryAddTransient<CodeDataWrapper>();
        services.TryAddTransient<CodeJsonWrapper>();
        services.TryAddTransient<WrapObjectTyped>();
        services.TryAddTransient<WrapObjectTypedItem>();

        // v17
        services.TryAddTransient<AssemblyCacheManager>();
        services.TryAddTransient<AppCodeLoader>();
        services.TryAddTransient<SourceAnalyzer>();
        services.TryAddSingleton<AssemblyResolver>();
        services.TryAddTransient<DependenciesLoader>();

        // Polymorphism - moved here v17.08
        services.AddTransient<IPolymorphismResolver, PolymorphismKoi>();
        services.AddTransient<IPolymorphismResolver, PolymorphismPermissions>();

        // v18
        services.TryAddSingleton<Util>();
        services.TryAddTransient<LightSpeedStats>();
        services.TryAddTransient<SourceCodeHasher>();

        // Add possibly missing fallback services
        // This must always be at the end here so it doesn't accidentally replace something we actually need
        services
            .AddSxcCoreLookUps()
            .AddServicesAndKits()
            .AddCmsContext()
            .ExternalConfig()
            .AddKoi()
            .AddSxcCoreFallbackServices();

        return services;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddCmsContext(this IServiceCollection services)
    {
        services.TryAddTransient<ICmsContext, CmsContext>();

        // v13 Site
        services.TryAddTransient<ICmsSite, CmsSite>();

        return services;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection ExternalConfig(this IServiceCollection services)
    {
        // new v15
        services.TryAddTransient<GoogleMapsSettings>();
        return services;
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddKoi(this IServiceCollection services)
    {
        services.TryAddTransient<Connect.Koi.KoiCss.Dependencies>();
        services.TryAddTransient<Connect.Koi.ICss, Connect.Koi.KoiCss>();

        return services;
    }


    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddNetVariations(this IServiceCollection services)
    {
#if NETFRAMEWORK
        // WebForms implementations
        services.TryAddScoped<IHttp, HttpNetFramework>();
#else
        services.TryAddTransient<IHttp, HttpNetCore>();
#endif
        return services;
    }
        
}