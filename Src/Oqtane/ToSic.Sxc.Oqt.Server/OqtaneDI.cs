using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Adam;
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
            services.AddScoped<ILinkPaths, OqtaneLinkPaths>();
            services.AddTransient<IServerPaths, OqtaneServerPaths>();


            services.AddTransient<IAppEnvironment, OqtaneEnvironment>();
            services.AddTransient<IEnvironment, OqtaneEnvironment>();
            services.AddTransient<ISite, OqtSite>();
            services.AddTransient<IRenderingHelper, OqtaneRenderingHelper>();
            services.AddTransient<IZoneMapper, OqtaneZoneMapper>();
            services.AddTransient<AppPermissionCheck, OqtanePermissionCheck>();
            services.AddTransient<DynamicCodeRoot, OqtaneDynamicCode>();
            services.AddTransient<IPlatformModuleUpdater, OqtaneModuleUpdater>();
            services.AddTransient<IEnvironmentInstaller, OqtaneEnvironmentInstaller>();
            services.AddTransient<IGetEngine, OqtaneGetLookupEngine>();
            services.AddTransient<OqtaneContextBuilder>();
            services.AddTransient<OqtaneContainer>();
            services.AddTransient<OqtTempInstanceContext>();
            services.AddTransient<OqtSite>();
            services.AddTransient<OqtaneZoneMapper>();
            services.AddTransient<SettingsHelper>();
            //// add page publishing
            services.AddTransient<IPagePublishing, OqtanePagePublishing>();

            //// Oqtane Specific stuff
            services.AddScoped<OqtAssetsAndHeaders>();
            services.AddTransient<OqtaneSiteFactory>();
            services.AddTransient<SxcOqtane>();
            services.AddTransient<IClientDependencyOptimizer, OqtClientDependencyOptimizer>();

            services.AddSingleton<Sxc.Run.Context.PlatformContext, OqtPlatformContext>();

            services.AddTransient<SecurityChecksBase, OqtAdamSecurityChecks>();
            services.AddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.AddTransient(typeof(AdamItemDtoMaker<,>), typeof(OqtAdamItemDtoMaker<,>));

            //// Add SxcEngineTest
            //services.AddTransient<SxcMvc>();
            //// Still pending...
            ////sc.AddTransient<XmlExporter, DnnXmlExporter>();
            services.AddTransient<IImportExportEnvironment, OqtaneImportExportEnvironment>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // 2020-10-22 2dm test
            services.AddTransient<ISxcOqtane, SxcOqtane>();
            //services.AddTransient<IRenderRazor, RenderRazor>();
            //services.AddTransient<IEngineFinder, OqtaneEngineFinder>();
            services.AddTransient<StatefulControllerDependencies>();

            return services;
        }



    }
}
