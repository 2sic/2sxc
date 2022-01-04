using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.Engines;
using ToSic.Sxc.LookUp;
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
            
            // Block functionality
            services.TryAddTransient<BlockDataSourceFactory>();
            services.TryAddTransient<BlockFromModule>();
            services.TryAddTransient<BlockFromEntity>();
            services.TryAddTransient<Services.IRenderService, RenderService>();  // new 12.05
#pragma warning disable CS0618
            services.TryAddTransient<Blocks.IRenderService, RenderService>();  // Obsolete, but keep for the few apps we already released in v12
#pragma warning restore CS0618

            // Configuration Provider WIP
            services.TryAddTransient<AppConfigDelegate>();
            services.TryAddTransient<App>();
            services.TryAddTransient<ImportExportEnvironmentBase.Dependencies>();

            // Rendering
            services.TryAddTransient<IRenderingHelper, RenderingHelper>();
            services.TryAddTransient<TokenEngine>();

            // Context stuff in general
            services.TryAddTransient<IContextOfBlock, ContextOfBlock>();
            services.TryAddTransient<IContextOfApp, ContextOfApp>();
            // 2021-09-01 2dm seems unused services.TryAddTransient<ContextOfBlock>();
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
            services.TryAddTransient<ToSic.Sxc.Services.IPageService, PageService>();  // must be unique per module where it's used
#pragma warning disable CS0618
            services.TryAddTransient<ToSic.Sxc.Web.IPageService, PageService>();  // Obsolete version, needed to keep old Apps working which used this
#pragma warning restore CS0618

            services.TryAddScoped<PageServiceShared>();             // must be scoped / shared across all modules
            services.TryAddTransient<IPageFeatures, PageFeatures>();
            services.TryAddSingleton<IPageFeaturesManager, PageFeaturesManager>();

            // new in v12.02/12.04 Image Link Resize Helper
            services.TryAddTransient<ImgResizeLinker>();

            // WIP - objects which are not really final
            services.TryAddTransient<WipRemoteRouterLink>();

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


            // Add possibly missing fallback services
            // This must always be at the end here so it doesn't accidentally replace something we actually need
            services.AddSxcCoreFallbackServices();

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