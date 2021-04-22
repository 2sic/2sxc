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
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Blazor;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Extensions.Koi;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server
{
    // ReSharper disable once InconsistentNaming
    static partial class OqtaneDI
    {
        public static IServiceCollection AddSxcOqtane(this IServiceCollection services)
        {
            services.AddScoped<ILinkPaths, OqtLinkPaths>();
            services.TryAddTransient<IServerPaths, OqtServerPaths>();

            services.TryAddTransient<ISite, OqtSite>();
            services.TryAddTransient<IUser, OqtUser>();
            services.TryAddTransient<IZoneCultureResolver, OqtSite>();
            services.TryAddTransient<IZoneMapper, OqtZoneMapper>();
            services.TryAddTransient<AppPermissionCheck, OqtPermissionCheck>();
            services.TryAddTransient<ILinkHelper, OqtLinkHelper>();
            // services.TryAddTransient<DynamicCodeRoot, OqtaneDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, OqtModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, OqtEnvironmentInstaller>();
            services.TryAddTransient<ILookUpEngineResolver, OqtGetLookupEngine>();
            services.TryAddTransient<OqtUiContextBuilder>();
            services.TryAddTransient<IModule, OqtModule>();
            services.TryAddTransient<OqtModule>();
            services.TryAddTransient<OqtTempInstanceContext>();
            services.TryAddTransient<OqtSite>();
            services.TryAddTransient<OqtZoneMapper>();
            services.TryAddTransient<SettingsHelper>();
            //// add page publishing
            services.TryAddTransient<IPagePublishing, OqtPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, OqtPagePublishingResolver>();

            //// Oqtane Specific stuff
            services.AddScoped<OqtAssetsAndHeaders>();
            services.TryAddTransient<SxcOqtane>();
            services.TryAddTransient<IClientDependencyOptimizer, OqtClientDependencyOptimizer>();
            services.TryAddTransient<IValueConverter, OqtValueConverter>();

            services.AddSingleton<IPlatform, OqtPlatformContext>();

            // ADAM stuff
            services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
            services.TryAddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.TryAddTransient(typeof(AdamItemDtoMaker<,>), typeof(OqtAdamItemDtoMaker<,>));

            //// Still pending...
            ////sc.TryAddTransient<XmlExporter, DnnXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();
            //sc.TryAddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.TryAddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // 2020-10-22 2dm test
            services.TryAddTransient<ISxcOqtane, SxcOqtane>();
            services.TryAddTransient<StatefulControllerDependencies>();


            // Experimental - it seems that Blazor hides the url params in the request
            services.TryAddTransient<IHttp, HttpBlazor>();

            // new in v12 - integrate KOI - experimental!
            try
            {
                services.ActivateKoi2Di();
            }
            catch { /* ignore */ }


            return services;
        }



    }
}
