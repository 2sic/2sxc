using Connect.Koi.Detectors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Integration.Modules;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Startup;

static partial class RegisterSxcServices
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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSxcCoreFallbackServices(this IServiceCollection services)
    {
        // basic environment, pages, modules etc.
        services.TryAddTransient<IEnvironmentInstaller, BasicEnvironmentInstaller>();
        services.TryAddTransient<IPlatformAppInstaller, BasicEnvironmentInstaller>();
        services.TryAddTransient<IPlatformModuleUpdater, BasicModuleUpdater>();
        //services.TryAddTransient<IPagePublishingResolver, BasicPagePublishingResolver>();
        services.TryAddTransient<IPagePublishing, BasicPagePublishing>();

        // This must never have a TRY! but only an AddTransient, as many can be registered by this type
        services.AddTransient<IPagePublishingGetSettings, PagePublishingGetSettingsOptional>(); // new v13 BETA #SwitchServicePagePublishingResolver
        services.AddTransient<IPagePublishingGetSettings, PagePublishingGetSettingsForbidden>();

        // Code / Dynamic Code
        services.TryAddTransient<CodeApiServiceFactory>();
        services.TryAddTransient<CodeApiService, CodeApiServiceUnknown>();
        services.TryAddTransient(typeof(CodeApiService<,>), typeof(CodeApiServiceUnknown<,>));
        services.TryAddTransient<IModule, ModuleUnknown>();
            
        // 11.08 - fallback in case not added
        services.TryAddSingleton<IPlatform, PlatformUnknown>();

        // ADAM basics
        // TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

        // v12.05 - linkhelperunknown - for testing etc.
        services.TryAddTransient<ILinkService, LinkServiceUnknown>();

        // v12.05
        services.TryAddTransient<ISystemLogService, SystemLogServiceUnknown>();

        // v12.05
        services.TryAddTransient<IMailService, MailServiceUnknown>();

        // v13.02
        services.TryAddTransient<IDynamicCodeService, DynamicCodeServiceUnknown>();

        // v13.02
        services.TryAddTransient<ILinkPaths, LinkPathsUnknown>();
        services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();

        // Koi, mainly so tests don't fail
        services.TryAddTransient<ICssFrameworkDetector, CssFrameworkDetectorUnknown>();

        // v15 DataSource
        services.TryAddTransient<PagesDataSourceProvider, PagesDataSourceProviderUnknown>();
        services.TryAddTransient<UsersDataSourceProvider, UsersDataSourceProviderUnknown>();
        services.TryAddTransient<RolesDataSourceProvider, RolesDataSourceProviderUnknown>();
        services.TryAddTransient<SitesDataSourceProvider, SitesDataSourceProviderUnknown>();

        // v16
        services.TryAddScoped<IJsApiService, JsApiServiceUnknown>();
        services.TryAddTransient<CodeErrorHelpService>();

        // v17.01
        services.TryAddTransient<IUserService, UsersService>();
        services.TryAddTransient<UserSourceProvider, UsersServiceProviderUnknown>();

        return services;
    }
}