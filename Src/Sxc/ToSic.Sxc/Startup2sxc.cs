using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Edit;

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

            // Block Editors
            services.TryAddTransient<BlockEditorForEntity>();
            services.TryAddTransient<BlockEditorForModule>();

            return services;
        }
    }
}