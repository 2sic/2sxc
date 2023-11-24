using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Admin;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn
{
    public static class StartUpDnnWebApi
    {
        public static IServiceCollection AddDnnWebApi(this IServiceCollection services)
        {
            // Settings / WebApi stuff
            services.TryAddTransient<IUiContextBuilder, DnnUiContextBuilder>();
            services.TryAddTransient<IApiInspector, DnnApiInspector>();

            // new #2160
            services.TryAddTransient<AdamSecurityChecksBase, DnnAdamSecurityChecks>();

            services.TryAddTransient<DnnGetBlock>();

            services.TryAddTransient<DnnAppFolderUtilities>(); // v14.12-01

            // new v15
            services.TryAddTransient<DynamicApiServices>();

            return services;
        }
    }
}
