using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.Web;
using ToSic.Sxc.Mvc.WebApi.Adam;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.Web.JsContext;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Mvc
{
    public static class StartUpMvc
    {
        public static IServiceCollection AddSxcMvc(this IServiceCollection services)
        {
            services.AddTransient<ISite, MvcSite>();
            services.AddTransient<IZoneCultureResolver, MvcSite>();
            services.AddTransient<IModule, MvcModule>();
            services.AddTransient<IZoneMapper, MvcZoneMapper>();
            services.AddTransient<AppPermissionCheck, MvcPermissionCheck>();
            //services.AddTransient<DynamicCodeRoot, MvcDynamicCode>();
            services.AddTransient<MvcContextBuilder>();

            // MVC Specific stuff
            services.AddScoped<MvcPageProperties>();

            // Add SxcEngineTest
            services.AddTransient<SxcMvc>();
            services.AddTransient<IAdamFileSystem<string, string>, MvcFileSystem>();
            // Still pending...
            //sc.AddTransient<XmlExporter, DnnXmlExporter>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // v16
            services.AddScoped<JsApiCache>();

            return services;
        }
    }
}
