using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Integration;
using ToSic.Eav.StartUp;
using ToSic.Eav.WebApi;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Dnn.Startup;
using ToSic.Sxc.Integration.Startup;
using ToSic.Sxc.Startup;


namespace ToSic.Sxc.Dnn.StartUp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class DnnDi
{
    private static bool _alreadyRegistered;

    public static IServiceCollection RegisterServices(IServiceCollection services)
    {
        var bl = BootLog.Log.Fn("Dnn: Registering Services", timer: true);

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
            .AddSxcCodeGen() // Code generation services
            .AddEavEverything()
            .AddEavWebApiTypedAfterEav()
            .AddRazorBlade();

        // Remember this for later, when we must start the Static Dependency Injection
        OriginalServiceCollection = services;

        _alreadyRegistered = true;
        bl.Done();
        return services;
    }

    public static IServiceCollection OriginalServiceCollection;

    public static IServiceCollection AddDnnPlugins(this IServiceCollection services)
    {
        // Integrate KOI Dnn-Parts
        services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, Connect.Koi.Dnn.DetectAndCacheDnnThemeCssFramework>();
        return services;
    }
}