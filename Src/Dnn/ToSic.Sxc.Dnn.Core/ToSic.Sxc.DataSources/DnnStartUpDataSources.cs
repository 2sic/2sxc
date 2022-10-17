using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    public static class DnnStartUpDataSources
    {
        public static IServiceCollection AddDnnSxcDataSources(this IServiceCollection services)
        {
            services.TryAddTransient<Roles, DnnRoles>();
            return services;
        }
    }
}
