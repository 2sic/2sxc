using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Polymorphism.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcEnginesStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcEngines(this IServiceCollection services)
    {
        services.TryAddTransient<EngineFactory>();
        services.TryAddTransient<IEngineFactory, EngineFactory>();

        services.TryAddTransient<EngineBase.Dependencies>();
        services.TryAddTransient<EngineCheckTemplate>();
        services.TryAddTransient<EnginePolymorphism>();
        services.TryAddTransient<EngineAppRequirements>();

        // Polymorphism
        services.TryAddTransient<PolymorphConfigReader>();

        return services;
    }


        
}