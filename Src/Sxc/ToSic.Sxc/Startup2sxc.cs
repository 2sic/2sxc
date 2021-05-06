using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Run;
using ToSic.Sxc.Security;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc
{
    public static class StartupSxc
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

            // Security
            services.TryAddTransient<IInputSanitizer, InputSanitizer>();
            
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

        /// <summary>
        /// This will add Do-Nothing services which will take over if they are not provided by the main system
        /// In general this will result in some features missing, which many platforms don't need or care about
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <remarks>
        /// All calls in here MUST use TryAddTransient, and never without the Try
        /// </remarks>
        public static IServiceCollection AddSxcCoreFallbackServices(this IServiceCollection services)
        {
            // basic environment, pages, modules etc.
            services.TryAddTransient<IEnvironmentInstaller, BasicEnvironmentInstaller>();
            services.TryAddTransient<IPlatformModuleUpdater, BasicModuleUpdater>();
            services.TryAddTransient<IPagePublishingResolver, BasicPagePublishingResolver>();
            services.TryAddTransient<IPagePublishing, BasicPagePublishing>();

            // Code / Dynamic Code
            services.TryAddTransient<DynamicCodeRoot, BasicDynamicCodeRoot>();
            services.TryAddTransient<IModule, ModuleUnknown>();
            
            // 11.08 - fallback in case not added
            services.TryAddSingleton<IPlatform, PlatformUnknown>();

            // ADAM basics
            services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

            return services;
        }
    }
}