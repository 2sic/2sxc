using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Sxc.Apps.ImportExport;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Basic;

namespace ToSic.Sxc.WebApi.Plumbing
{
    public static class SxcDependencyInjection
    {
        public static IServiceCollection AddSxc(this IServiceCollection services)
        {
            services.TryAddTransient<Eav.Conversion.EntitiesToDictionary, DataToDictionary>();
            services.TryAddScoped<IHttp, HttpAbstraction>();
            services.TryAddTransient<XmlImportWithFiles, XmlImportFull>();

            // These are usually replaced by the target platform
            services.TryAddTransient<IClientDependencyOptimizer, BasicClientDependencyOptimizer>();


            return services;
        }
    }
}
