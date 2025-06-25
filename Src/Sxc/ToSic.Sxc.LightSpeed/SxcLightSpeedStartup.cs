using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Web.Sys.LightSpeed;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcLightSpeedStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcLightSpeed(this IServiceCollection services)
    {
        // v13 LightSpeed
        services.TryAddTransient<IOutputCache, LightSpeed>();
        services.TryAddTransient<LightSpeedStats>();

        return services;
    }


        
}