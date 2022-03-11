using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Data;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Cms;
using ToSic.Sxc.Oqt.Server.Data;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.Services;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    internal static partial class OqtRegisterServices
    {


        /// <summary>
        /// Ensure that the module-concept works
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddSxcOqtModule(this IServiceCollection services)
        {
            // Multi-Site Services
            services.TryAddTransient<IZoneMapper, OqtZoneMapper>();

            // Update title etc. when a module-content changes
            services.TryAddTransient<IPlatformModuleUpdater, OqtModuleUpdater>();
            
            // Page publishing - ATM neutral objects which don't do much
            services.TryAddTransient<IPagePublishing, OqtPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, OqtPagePublishingResolver>();

            services.TryAddTransient<OqtModuleHelper>();

            return services;
        }
        
        

        private static IServiceCollection AddSxcOqtDynCodeAndViews(this IServiceCollection services)
        {
            services.TryAddTransient<ILinkHelper, OqtLinkHelper>();

            services.TryAddTransient<OqtPageOutput>();
            services.TryAddTransient<OqtSxcViewBuilder>();
            services.TryAddTransient<IBlockResourceExtractor, OqtBlockResourceExtractor>();
            services.TryAddTransient<IValueConverter, OqtValueConverter>();

            // Views / Templates / Razor: View Builder
            services.TryAddTransient<OqtSxcViewBuilder>();

            services.TryAddTransient<DynamicCodeRoot, OqtaneDynamicCodeRoot>();
            services.TryAddTransient<OqtGetBlock>();

            // v13
            services.TryAddTransient<IModuleAndBlockBuilder, OqtModuleAndBlockBuilder>();

            // Views / Templates / Razor: Polymorphism Resolvers
            services.TryAddTransient<Sxc.Polymorphism.Koi>();
            services.TryAddTransient<Polymorphism.Permissions>();
            try
            {
                services.ActivateKoi2Di();
            }
            catch { /* ignore */ }

            return services;
        }

        private static IServiceCollection AddRazorDependencies(this IServiceCollection services)
        {
            // Microsoft Services used to run Razor?
            // enable use of UrlHelper for AbsolutePath
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            return services;
        }

    }
}
