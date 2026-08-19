using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Render.Engines.Sys;
using ToSic.Sxc.Render.Polymorphism.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcEngines
{
    public static IServiceCollection AddSxcEngines(this IServiceCollection services)
    {
        services.TryAddTransient<EngineCheckTemplate>();
        services.TryAddTransient<EnginePolymorphism>();
        services.TryAddTransient<EngineRequirementsApp>();

        // Polymorphism
        services.TryAddTransient<IEditionService, EditionService>();

        // New v21
        services.TryAddTransient<EngineSpecsService>();

        return services;
    }
}