using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Web.Sys.LightSpeed;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcLightSpeed
{
    public static IServiceCollection AddSxcLightSpeed(this IServiceCollection services)
    {
        // v13 LightSpeed
        services.TryAddTransient<IOutputCache, LightSpeed>();
        services.TryAddTransient<OutputCacheManager>();
        services.TryAddTransient<LightSpeedExternalDependencies>();
        services.TryAddTransient<LightSpeedStats>();
        services.TryAddTransient<IOutputCacheFlushService, OutputCacheFlushService>();

        // Replace the fallback flusher with the LightSpeed-backed implementation.
        //services.Replace(ServiceDescriptor.Transient<IOutputCacheFlushService, OutputCacheFlushService>());


        return services;
    }
}
