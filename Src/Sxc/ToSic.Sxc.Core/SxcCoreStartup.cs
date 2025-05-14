using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.StartUp;
using ToSic.Sxc.Startup;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCoreStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCoreNew(this IServiceCollection services)
    {
        // Sxc StartUp Routines - MUST be AddTransient, not TryAddTransient so many start-ups can be registered
        // Add StartUp Registration of FeaturesCatalog
        services.AddTransient<IStartUpRegistrations, SxcStartUpFeaturesRegistrations>();


        return services;
    }


        
}