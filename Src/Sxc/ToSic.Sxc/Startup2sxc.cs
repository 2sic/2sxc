using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
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

            return services;
        }
    }
}