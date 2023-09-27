using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn
{
    public static class StartUpDnnCore
    {
        public static IServiceCollection AddDnnCore(this IServiceCollection services)
        {
            //services.TryAddScoped<CodeRootFactory, DnnCodeRootFactory>();
            services.TryAddSingleton<IHostingEnvironmentWrapper, HostingEnvironmentWrapper>();
            services.TryAddSingleton<IReferencedAssembliesProvider, ReferencedAssembliesProvider>();
            return services;
        }
    }
}
