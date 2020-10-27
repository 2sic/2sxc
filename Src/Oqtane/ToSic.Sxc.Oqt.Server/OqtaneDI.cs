using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Page;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server
{
    static class OqtaneDI
    {
        public static IServiceCollection AddSxcOqtane(this IServiceCollection services)
        {
            services.AddScoped<ILinkPaths, OqtaneLinkPaths>();
            services.AddTransient<IServerPaths, OqtaneServerPaths>();


            services.AddTransient<IAppEnvironment, OqtaneEnvironment>();
            services.AddTransient<IEnvironment, OqtaneEnvironment>();
            services.AddTransient<ITenant, OqtaneTenantSite>();
            services.AddTransient<IRenderingHelper, OqtaneRenderingHelper>();
            services.AddTransient<IZoneMapper, OqtaneZoneMapper>();
            services.AddTransient<AppPermissionCheck, OqtanePermissionCheck>();
            services.AddTransient<DynamicCodeRoot, OqtaneDynamicCode>();
            services.AddTransient<IPlatformModuleUpdater, OqtanePlatformModuleUpdater>();
            services.AddTransient<IEnvironmentInstaller, OqtaneEnvironmentInstaller>();
            services.AddTransient<IGetEngine, OqtaneGetLookupEngine>();
            services.AddTransient<OqtaneContextBuilder>();

            //// add page publishing
            services.AddTransient<IPagePublishing, OqtanePagePublishing>();

            //// MVC Specific stuff
            services.AddScoped<OqtAssetsAndHeaders>();
            services.AddTransient<OqtaneSiteFactory>();
            services.AddTransient<SxcOqtane>();

            services.AddSingleton<Sxc.Run.Context.PlatformContext, OqtPlatformContext>();

            //services.AddTransient<SecurityChecksBase, MvcAdamSecurityChecks>();

            //// Add SxcEngineTest
            //services.AddTransient<SxcMvc>();
            services.AddTransient<IAdamFileSystem<string, string>, OqtaneAdamFileSystem>();
            //// Still pending...
            ////sc.AddTransient<XmlExporter, DnnXmlExporter>();
            services.AddTransient<IImportExportEnvironment, OqtaneImportExportEnvironment>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // 2020-10-22 2dm test
            services.AddTransient<IRenderTestIds, SxcOqtane>();
            //services.AddTransient<IRenderRazor, RenderRazor>();
            //services.AddTransient<IEngineFinder, OqtaneEngineFinder>();

            return services;
        }



    }
}
