using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Blazor;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.Adam;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server
{
    // ReSharper disable once InconsistentNaming
    static class OqtaneDI
    {
        public static IServiceCollection AddSxcOqtane(this IServiceCollection services)
        {
            services.AddScoped<ILinkPaths, OqtLinkPaths>();
            services.AddTransient<IServerPaths, OqtServerPaths>();

            services.AddTransient<ISite, OqtSite>();
            services.AddTransient<IUser, OqtUser>();
            services.AddTransient<IZoneCultureResolver, OqtSite>();
            services.AddTransient<IZoneMapper, OqtZoneMapper>();
            services.AddTransient<AppPermissionCheck, OqtPermissionCheck>();
            services.AddTransient<DynamicCodeRoot, OqtaneDynamicCode>();
            services.AddTransient<IPlatformModuleUpdater, OqtModuleUpdater>();
            services.AddTransient<IEnvironmentInstaller, OqtEnvironmentInstaller>();
            services.AddTransient<ILookUpEngineResolver, OqtGetLookupEngine>();
            services.AddTransient<OqtUiContextBuilder>();
            services.AddTransient<IModule, OqtModule>();
            services.AddTransient<OqtModule>();
            services.AddTransient<OqtTempInstanceContext>();
            services.AddTransient<OqtSite>();
            services.AddTransient<OqtZoneMapper>();
            services.AddTransient<SettingsHelper>();
            //// add page publishing
            services.AddTransient<IPagePublishing, OqtPagePublishing>();
            services.AddTransient<IPagePublishingResolver, OqtPagePublishingResolver>();

            //// Oqtane Specific stuff
            services.AddScoped<OqtAssetsAndHeaders>();
            services.AddTransient<SxcOqtane>();
            services.AddTransient<IClientDependencyOptimizer, OqtClientDependencyOptimizer>();
            services.AddTransient<IValueConverter, OqtValueConverter>();

            services.AddSingleton<IPlatform, OqtPlatformContext>();

            // ADAM stuff
            services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
            services.AddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.AddTransient(typeof(AdamItemDtoMaker<,>), typeof(OqtAdamItemDtoMaker<,>));

            //// Still pending...
            ////sc.AddTransient<XmlExporter, DnnXmlExporter>();
            services.AddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // 2020-10-22 2dm test
            services.AddTransient<ISxcOqtane, SxcOqtane>();
            services.AddTransient<StatefulControllerDependencies>();


            // Experimental - it seems that Blazor hides the url params in the request
            services.TryAddTransient<IHttp, HttpBlazor>();


            return services;
        }



    }
}
