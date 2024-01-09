using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.StartUp;
using ToSic.Eav.WebApi;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Dnn.Startup;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.Startup;
using ToSic.Sxc.WebApi;


namespace ToSic.Sxc.Dnn.StartUp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class DnnDi
{
    private static bool _alreadyRegistered;

    public static IServiceCollection RegisterServices(IServiceCollection services)
    {
        if (_alreadyRegistered)
            return OriginalServiceCollection;

        // If this is called from Dnn 7 - 9.3 it won't have services, so we must create our own
        // This is because the old Dnn wasn't DI aware
        services ??= new ServiceCollection();

        services
            .AddDnnPlugins()
            .AddDnnCore() // TODO: Move core stuff from AddDnn to AddDnnCore and make implementations internal
            .AddDnnSxcDataSources()
            .AddDnnDataSources()
            .AddDnnWebApi()
            .AddDnnRazor()
            .AddDnnCompatibility()
            .AddAdamWebApi<int, int>()
            .AddSxcWebApi()
            .AddSxcCore()
            .AddEav()
            .AddEavWebApiTypedAfterEav()
            .AddRazorBlade();

        // temp polymorphism - later put into AddPolymorphism
        services.TryAddTransient<Koi>();
        services.TryAddTransient<Permissions>();

        // Remember this for later, when we must start the Static Dependency Injection
        OriginalServiceCollection = services;

        _alreadyRegistered = true;
        return services;
    }

    public static IServiceCollection OriginalServiceCollection;

    public static IServiceCollection AddDnnPlugins(this IServiceCollection services)
    {
        services.TryAddTransient<IRazorEngine, DnnRazorEngine>();

        // Integrate KOI Dnn-Parts
        services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, Connect.Koi.Dnn.DetectAndCacheDnnThemeCssFramework>();

        return services;
    }
}