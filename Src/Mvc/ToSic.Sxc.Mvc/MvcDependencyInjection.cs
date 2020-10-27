using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Interfaces;
//using ToSic.Sxc.Mvc.Code;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.Web;
using ToSic.Sxc.Mvc.WebApi.Adam;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Mvc.Plumbing
{
    public static class MvcDependencyInjection
    {
        public static IServiceCollection AddSxcMvc(this IServiceCollection services)
        {
            services.AddTransient<IAppEnvironment, MvcEnvironment>();
            services.AddTransient<IEnvironment, MvcEnvironment>();
            services.AddTransient<ITenant, MvcTenant>();
            services.AddTransient<IRenderingHelper, MvcRenderingHelper>();
            services.AddTransient<IZoneMapper, MvcZoneMapper>();
            services.AddTransient<AppPermissionCheck, MvcPermissionCheck>();
            //services.AddTransient<DynamicCodeRoot, MvcDynamicCode>();
            services.AddTransient<IPlatformModuleUpdater, MvcModuleUpdater>();
            services.AddTransient<IEnvironmentInstaller, MvcEnvironmentInstaller>();
            services.AddTransient<IGetEngine, MvcGetLookupEngine>();
            services.AddTransient<MvcContextBuilder>();

            // add page publishing
            services.AddTransient<IPagePublishing, MvcPagePublishing>();

            // MVC Specific stuff
            services.AddScoped<MvcPageProperties>();

            services.AddTransient<SecurityChecksBase, MvcAdamSecurityChecks>();

            // Add SxcEngineTest
            services.AddTransient<SxcMvc>();
            services.AddTransient<IAdamFileSystem<string, string>, MvcFileSystem>();
            // Still pending...
            //sc.AddTransient<XmlExporter, DnnXmlExporter>();
            services.AddTransient<IImportExportEnvironment, MvcImportExportEnvironment>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();


            return services;
        }
    }
}
