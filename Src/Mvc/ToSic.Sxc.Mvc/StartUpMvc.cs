using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Mvc.NotImplemented;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.Web;
using ToSic.Sxc.Mvc.WebApi.Adam;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Mvc
{
    public static class StartUpMvc
    {
        public static IServiceCollection AddSxcMvc(this IServiceCollection services)
        {
            services.AddTransient<IEnvironment, MvcEnvironment>();
            services.AddTransient<ISite, MvcSite>();
            services.AddTransient<IZoneMapper, MvcZoneMapper>();
            services.AddTransient<AppPermissionCheck, MvcPermissionCheck>();
            //services.AddTransient<DynamicCodeRoot, MvcDynamicCode>();
            services.AddTransient<MvcContextBuilder>();

            // add page publishing
            services.AddTransient<IPagePublishing, MvcPagePublishing>();
            services.AddTransient<IPagePublishingResolver, NotImplementedPagePublishingResolver>();

            // MVC Specific stuff
            services.AddScoped<MvcPageProperties>();

            services.AddTransient<SecurityChecksBase, MvcAdamSecurityChecks>();

            // Add SxcEngineTest
            services.AddTransient<SxcMvc>();
            services.AddTransient<IAdamFileSystem<string, string>, MvcFileSystem>();
            // Still pending...
            //sc.AddTransient<XmlExporter, DnnXmlExporter>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();


            return services;
        }
    }
}
