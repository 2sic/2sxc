using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Configuration;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Compatibility;

internal static class StartUpCompatibility
{
    /// <summary>
    /// Add obsolete interfaces which had previously been supported
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDnnCompatibility(this IServiceCollection services)
    {
        services.TryAddTransient<ILogService, LogServiceUsingOldInterface>();
        services.TryAddTransient<Eav.Configuration.IFeaturesService, FeaturesServiceCompatibility>();

        return services;
    }
}