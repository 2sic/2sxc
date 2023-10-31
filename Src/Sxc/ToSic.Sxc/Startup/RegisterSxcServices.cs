using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource.Catalog;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.CmsSys;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Helpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Images;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Plumbing;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services.GoogleMaps;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.EditUi;
using ToSic.Sxc.Web.JsContext;
using ToSic.Sxc.Web.LightSpeed;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Startup
{
    public static partial class RegisterSxcServices
    {
        public static IServiceCollection AddSxcCore(this IServiceCollection services)
        {
            // Runtimes
            services.TryAddTransient<CmsManager>();
            services.TryAddTransient<CmsZones>();
            services.TryAddTransient<AppsRuntime>();
            services.TryAddTransient<AppsManager>();
            services.TryAddTransient<ViewsManager>();
            services.TryAddTransient<BlocksManager>();

            // New runtimes, better architecture v16.07+
            services.TryAddTransient<AppWorkSxc>();
            services.TryAddTransient<AppBlocks>();
            services.TryAddTransient<AppViews>();

            // Code
            services.TryAddTransient<DynamicCodeRoot.MyServices>();

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
            services.TryAddTransient<AppConfigDelegate>();
            services.TryAddTransient<App>();
            services.TryAddTransient<ImportExportEnvironmentBase.MyServices>();

            // Rendering
            services.TryAddTransient<IRenderingHelper, RenderingHelper>();
            services.TryAddTransient<TokenEngine>();

            // Context stuff in general
            services.TryAddTransient<IContextOfBlock, ContextOfBlock>();

            // Context stuff for the page (not EAV)
            services.TryAddTransient<IPage, Page>();
            services.TryAddTransient<Page>();


            // Context stuff, which is explicitly scoped
            services.TryAddScoped<Context.IContextResolver, Context.ContextResolver>();
            // New v15.04 WIP
            services.TryAddScoped<Eav.Context.IContextResolver>(x => x.GetRequiredService<Context.IContextResolver>());
            services.TryAddScoped<IContextResolverUserPermissions>(x => x.GetRequiredService<Context.IContextResolver>());
            services.TryAddScoped<AppIdResolver>();


            // JS UI Context
            services.TryAddTransient<JsContextAll>();
            services.TryAddTransient<JsContextLanguage>();
            services.TryAddScoped<JsApiCache>(); // v16.01

            // Adam stuff
            services.TryAddTransient<AdamSecurityChecksBase, AdamSecurityChecksBasic>();
            services.TryAddTransient<IAdamPaths, AdamPathsBase>();
            services.TryAddTransient<AdamConfiguration>();

            services.AddTransient<AdamManager.MyServices>();

            // WIP - add net-core specific stuff
            services.AddNetVariations();

            // Polymorphism
            services.TryAddTransient<Polymorphism.Polymorphism>();


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
            services.TryAddTransient<AppFilesDataSourceProvider>();
            services.TryAddTransient<AppFilesDataSourceProvider.MyServices>();
            services.TryAddTransient(typeof(AdamDataSourceProvider<,>));
            services.TryAddTransient(typeof(AdamDataSourceProvider<,>.MyServices));
            services.TryAddTransient<IAppDataSourcesLoader, AppDataSourcesLoader>();

            // v15 EditUi Resources
            services.TryAddTransient<EditUiResources>();

            // v15
            services.TryAddTransient<DynamicCodeDataSources>();

            // v16 DynamicJacket and DynamicRead factories
            services.TryAddTransient<CodeDataWrapper>();
            services.TryAddTransient<CodeJsonWrapper>();
            services.TryAddTransient<WrapObjectTyped>();
            services.TryAddTransient<WrapObjectTypedItem>();

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

        public static IServiceCollection AddCmsContext(this IServiceCollection services)
        {
            services.TryAddTransient<ICmsContext, CmsContext>();

            // v13 Site
            services.TryAddTransient<ICmsSite, CmsSite>();

            return services;
        }

        public static IServiceCollection ExternalConfig(this IServiceCollection services)
        {
            // new v15
            services.TryAddTransient<GoogleMapsSettings>();
            return services;
        }

        public static IServiceCollection AddKoi(this IServiceCollection services)
        {
            services.TryAddTransient<Connect.Koi.KoiCss.Dependencies>();
            services.TryAddTransient<Connect.Koi.ICss, Connect.Koi.KoiCss>();

            return services;
        }

        
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
}