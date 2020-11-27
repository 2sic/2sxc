using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Run;

using ToSic.Sxc.Web;

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

            // Context stuff
            services.TryAddTransient<IContextOfBlock, ContextOfBlock>();
            services.TryAddTransient<ContextOfBlock>();
            services.TryAddTransient<IPage, SxcPage>();
            services.TryAddTransient<SxcPage>();
            services.TryAddTransient<ICmsContext, CmsContext>();

            // Add possibly missing fallbacks
            services.AddSxcCoreFallbackServices();

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
            services.TryAddTransient<IEnvironmentInstaller, BasicEnvironmentInstaller>();
            services.TryAddTransient<IPlatformModuleUpdater, BasicModuleUpdater>();
            services.TryAddTransient<IPagePublishingResolver, BasicPagePublishingResolver>();
            services.TryAddTransient<IImportExportEnvironment, BasicImportExportEnvironment>();
            services.TryAddTransient<IPagePublishing, BasicPagePublishing>();
            return services;
        }
    }
}