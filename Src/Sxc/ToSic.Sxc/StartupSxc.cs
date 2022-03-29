using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Images;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Plumbing;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.JsContext;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc
{
    public static partial class StartupSxc
    {
        public static IServiceCollection AddSxcCore(this IServiceCollection services)
        {
            // Runtimes
            services.TryAddTransient<CmsRuntime>();
            services.TryAddTransient<CmsManager>();
            services.TryAddTransient<CmsZones>();
            services.TryAddTransient<AppsRuntime>();
            services.TryAddTransient<AppsManager>();
            services.TryAddTransient<ViewsRuntime>();
            services.TryAddTransient<ViewsManager>();
            services.TryAddTransient<BlocksRuntime>();
            services.TryAddTransient<BlocksManager>();

            // Code
            services.TryAddTransient<DynamicCodeRoot.Dependencies>();
            services.TryAddTransient<DynamicEntityDependencies>();

            // Block Editors
            services.TryAddTransient<BlockEditorForEntity>();
            services.TryAddTransient<BlockEditorForModule>();
            services.TryAddTransient<BlockEditorBaseDependencies>();

            // Block functionality
            services.TryAddTransient<BlockDataSourceFactory>();
            services.TryAddTransient<BlockFromModule>();
            services.TryAddTransient<BlockFromEntity>();
            services.TryAddTransient<BlockBase.Dependencies>();
            services.TryAddTransient<Services.IRenderService, RenderService>();  // new 12.05
#pragma warning disable CS0618
            services.TryAddTransient<Blocks.IRenderService, RenderService>();  // Obsolete, but keep for the few apps we already released in v12
#pragma warning restore CS0618

            // Configuration Provider WIP
            services.TryAddTransient<QueryStringLookUp>();
            services.TryAddTransient<AppConfigDelegate>();
            services.TryAddTransient<App>();
            services.TryAddTransient<ImportExportEnvironmentBase.Dependencies>();

            // Rendering
            services.TryAddTransient<IRenderingHelper, RenderingHelper>();
            services.TryAddTransient<TokenEngine>();

            // Context stuff in general
            services.TryAddTransient<IContextOfBlock, ContextOfBlock>();
            services.TryAddTransient<IContextOfApp, ContextOfApp>();
            services.TryAddTransient<ContextOfApp.ContextOfAppDependencies>();
            services.TryAddTransient<ContextOfSite.ContextOfSiteDependencies>();
            services.TryAddTransient<IPage, Page>();
            services.TryAddTransient<Page>();
            services.TryAddTransient<ICmsContext, CmsContext>();

            // Context stuff, which is explicitly scoped
            services.TryAddScoped<IContextResolver, ContextResolver>();
            services.TryAddScoped<AppIdResolver>();

            // JS UI Context
            services.TryAddTransient<JsContextAll>();
            services.TryAddTransient<JsContextLanguage>();

            // Adam stuff
            services.TryAddTransient<AdamMetadataMaker>();
            services.TryAddTransient<AdamSecurityChecksBase, AdamSecurityChecksBasic>();
            services.TryAddTransient<IAdamPaths, AdamPathsBase>();
            services.TryAddTransient<AdamConfiguration>();

            // WIP - add net-core specific stuff
            services.AddNetVariations();

            // Polymorphism
            services.TryAddTransient<Polymorphism.Polymorphism>();

            // new in v12.02 - PageService & Page Features
            services.TryAddTransient<Services.IPageService, PageService>();  // must be unique per module where it's used
#pragma warning disable CS0618
            services.TryAddTransient<Web.IPageService, PageService>();  // Obsolete version, needed to keep old Apps working which used this
#pragma warning restore CS0618

            // 2022-02-07 2dm experimental
            // The PageServiceShared must always be generated from the PageScope
            // I thought that the PageServiceShared must be scoped at page level, but I believe this is wrong
            // Reason is that it seems to collect specs per module, and then actually only flushes it
            // Because it shouldn't remain in the list for the second module
            // So it actually looks like it's very module-scoped already, but had workarounds for it.
            // So I think it really doesn't need to be have workarounds for it
            services.TryAddScoped<PageServiceShared>();
            //services.TryAddTransient<PageServiceShared>(); // this is only used for the next line where we create the scoped version
            //services.TryAddScoped<IPageServiceShared>(sp => sp.Build<PageScopedService<PageServiceShared>>().Value);             // must be scoped / shared across all modules

            services.TryAddTransient<IPageFeatures, PageFeatures>();
            services.TryAddSingleton<IPageFeaturesManager, PageFeaturesManager>();

            // new in v12.02/12.04 Image Link Resize Helper
            services.TryAddTransient<ImgResizeLinker>();

            // WIP - objects which are not really final
            services.TryAddTransient<RemoteRouterLink>();

            // WIP 12.05 - json converter
            services.TryAddTransient<IJsonService, JsonService>();
            services.TryAddTransient<IConvertService, ConvertService>();

            // New 12.05: SecureData
            services.TryAddTransient<ISecureDataService, SecureDataService>();

            // 12.06.01 moved here from WebApi, but it should probably be in Dnn as it's probably just used there
            services.TryAddTransient<IServerPaths, ServerPaths>();

            // 13 - ToolbarService & IFeaturesService
            services.TryAddTransient<IToolbarService, ToolbarService>();    // New 13.00
            services.TryAddTransient<IFeaturesService, FeaturesService>();  // New 13.01
            services.TryAddTransient<IImageService, ImageService>();

            // 13 - cleaning up handling of app paths
            services.TryAddTransient<AppFolderInitializer>();
            services.TryAddTransient<AppIconHelpers>();

            // v13 Provide page scoped services
            // This is important, as most services are module scoped, but very few are actually scoped one level higher
            services.TryAddScoped<PageScopeAccessor>();
            services.TryAddScoped(typeof(PageScopedService<>));

            // v13 DynamicCodeService
            services.TryAddTransient<IEditService, EditService>();
            services.TryAddTransient<DynamicCodeService.Dependencies>();
            services.TryAddTransient<IDynamicCodeService, DynamicCodeService>();

            // Add possibly missing fallback services
            // This must always be at the end here so it doesn't accidentally replace something we actually need
            services
                .AddKoi()
                .AddSxcCoreFallbackServices();

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
#if NETSTANDARD
            services.TryAddTransient<IHttp, HttpNetCore>();
#else
            // WebForms implementations
            services.TryAddScoped<IHttp, HttpNetFramework>();
#endif
            return services;
        }
        
    }
}