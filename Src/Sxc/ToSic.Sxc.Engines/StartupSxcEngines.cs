using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Polymorphism.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcEngines
{
    public static IServiceCollection AddSxcEngines(this IServiceCollection services)
    {
        services.TryAddTransient<EngineFactory>();
        services.TryAddTransient<IEngineFactory, EngineFactory>();

        services.TryAddTransient<EngineCheckTemplate>();
        services.TryAddTransient<EnginePolymorphism>();
        services.TryAddTransient<EngineAppRequirements>();

        // Polymorphism
        services.TryAddTransient<PolymorphConfigReader>();

        // New v21
        services.TryAddTransient<EngineSpecsService>();

        return services;
    }
}