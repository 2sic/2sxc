using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Code;

namespace ToSic.Sxc.Dnn
{
    public static class StartUpDnnCore
    {
        public static IServiceCollection AddDnnCore(this IServiceCollection services)
        {
            //services.TryAddScoped<CodeRootFactory, DnnCodeRootFactory>();
            return services;
        }
    }
}
