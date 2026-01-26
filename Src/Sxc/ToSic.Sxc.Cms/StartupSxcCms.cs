using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Cms.Sites.Sys;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DataSources.Sys.AppAssets;
using ToSic.Sxc.DataSources.Sys.Pages;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcCms
{
    public static IServiceCollection AddSxcCms(this IServiceCollection services)
    {
        services.TryAddTransient<IPagePublishing, BasicPagePublishing>();

        // This must never have a TRY! but only an AddTransient, as many can be registered by this type
        services.AddTransient<IPagePublishingGetSettings, PagePublishingGetSettingsOptional>(); // new v13 BETA #SwitchServicePagePublishingResolver
        services.AddTransient<IPagePublishingGetSettings, PagePublishingGetSettingsForbidden>();

        // v15 DataSource
        services.TryAddTransient<PagesDataSourceProvider, PagesDataSourceProviderUnknown>();
        services.TryAddTransient<IUsersProvider, UsersProviderUnknown>();
        services.TryAddTransient<IUserRolesProvider, UserRolesProviderUnknown>();
        services.TryAddTransient<SitesDataSourceProvider.Dependencies>();
        services.TryAddTransient<SitesDataSourceProvider, SitesDataSourceProviderUnknown>();

        services.TryAddTransient<AppAssetsDataSourceProvider>();
        services.TryAddTransient<AppAssetsDataSourceProvider.Dependencies>();
        services.TryAddTransient(typeof(AdamDataSourceProvider<,>));
        services.TryAddTransient(typeof(AdamDataSourceProvider<,>.Dependencies));


        return services;
    }


        
}