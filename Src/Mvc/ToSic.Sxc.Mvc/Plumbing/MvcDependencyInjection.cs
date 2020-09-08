using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Mvc.Code;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.Web;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

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
            services.AddTransient<DynamicCodeRoot, MvcDynamicCode>();
            services.AddTransient<IEnvironmentConnector, MvcEnvironmentConnector>();
            services.AddTransient<IEnvironmentInstaller, MvcEnvironmentInstaller>();
            services.AddTransient<IGetEngine, MvcGetLookupEngine>();
            services.AddTransient<MvcContextBuilder>();

            // add page publishing
            services.AddTransient<IPagePublishing, MvcPagePublishing>();

            // MVC Specific stuff
            services.AddScoped<MvcPageProperties>();

            // Still pending...
            //sc.AddTransient<XmlExporter, DnnXmlExporter>();
            //sc.AddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IEnvironmentFileSystem, DnnFileSystem>();


            return services;
        }
    }
}
