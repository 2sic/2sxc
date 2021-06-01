using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;

namespace ToSic.Sxc
{
    public static partial class StartupSxc
    {

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