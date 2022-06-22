using Connect.Koi.Detectors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Startup
{
    public static partial class RegisterSxcServices
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
            //services.TryAddTransient<IPagePublishingResolver, BasicPagePublishingResolver>();
            services.TryAddTransient<IPagePublishing, BasicPagePublishing>();

            // This must never have a TRY! but only an AddTransient, as many can be registered by this type
            services.AddTransient<IPagePublishingGetSettings, PagePublishingGetSettingsOptional>(); // new v13 BETA #SwitchServicePagePublishingResolver
            services.AddTransient<IPagePublishingGetSettings, PagePublishingGetSettingsForbidden>();

            // Code / Dynamic Code
            services.TryAddTransient<DynamicCodeRoot, BasicDynamicCodeRoot>();
            services.TryAddTransient<IModule, ModuleUnknown>();
            
            // 11.08 - fallback in case not added
            services.TryAddSingleton<IPlatform, PlatformUnknown>();

            // ADAM basics
            // TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
            services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

            // v12.05 - linkhelperunknown - for testing etc.
            services.TryAddTransient<ILinkService, LinkServiceUnknown>();

            // v12.05
            services.TryAddTransient<IRazorService, RazorServiceUnknown>();

            // v12.05
            services.TryAddTransient<ILogService, LogServiceUnknown>();

            // v12.05
            services.TryAddTransient<IMailService, MailServiceUnknown>();

            // v13.02
            services.TryAddTransient<IDynamicCodeService, DynamicCodeServiceUnknown>();

            // v13.02
            services.TryAddTransient<ILinkPaths, LinkPathsUnknown>();
            services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();

            // v13.04
            services.TryAddTransient<IUserInformationService, UserInformationServiceUnknown>();

            // Koi, mainly so tests don't fail
            services.TryAddTransient<ICssFrameworkDetector, CssFrameworkDetectorUnknown>();

            return services;
        }
    }
}