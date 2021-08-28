using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            services.TryAddTransient<ContextOfBlock>();
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

            // WIP - add net-core specific stuff
            services.AddNetVariations();

            // Add possibly missing fallbacks
            services.AddSxcCoreFallbackServices();
            
            // Polymorphism
            services.TryAddTransient<Polymorphism.Polymorphism>();

            // new in v12.02 - PageService & Page Features
            services.TryAddTransient<IPageService, PageService>();  // must be unique per module where it's used
            services.TryAddScoped<PageServiceShared>();             // must be scoped / shared across all modules
            services.TryAddTransient<IPageFeatures, PageFeatures>();
            services.TryAddSingleton<IPageFeaturesManager, PageFeaturesManager>();

            // new in v12.02/12.04 Image Link Resize Helper
            services.TryAddTransient<ImgResizeLinker>();

            // WIP - objects which are not really final
            services.TryAddTransient<WipRemoteRouterLink>();

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