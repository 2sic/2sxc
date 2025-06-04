using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Eav.Integration;
using ToSic.Eav.WebApi;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.HotBuild;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Dnn.Startup;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Images;
using ToSic.Sxc.Integration.Startup;
using ToSic.Sxc.Services;
using ToSic.Sxc.Startup;
using ToSic.Sxc.Web;


namespace ToSic.Sxc.Dnn.StartUp;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class DnnDi
{
    private static bool _alreadyRegistered;

    public static IServiceCollection RegisterServices(IServiceCollection services)
    {
        var l = BootLog.Log.Fn("Dnn: Registering Services", timer: true);

        if (_alreadyRegistered)
            return OriginalServiceCollection;

        // If this is called from Dnn 7 - 9.3 it won't have services, so we must create our own
        // This is because the old Dnn wasn't DI aware
        services ??= new ServiceCollection();

        l.A("Will start with DNN parts");
        services
            .AddDnnPlugins()
            .AddObsoleteServicesAndKits()
            .AddDnnCore() // TODO: Move core stuff from AddDnn to AddDnnCore and make implementations internal
            .AddDnnSxcDataSources()
            .AddDnnDataSources()
            .AddDnnWebApi()
            .AddDnnRazor()
            .AddDnnCompatibility();

        l.A("Will start with ADAM and 2sxc parts");
        services
            .AddAdamWebApi<int, int>()
            .AddSxcWebApi();

        l.A("Will start with 2sxc Code / Engines etc.");
        services
            .AddSxcCustom()
            .AddSxcCode()
            .AddSxcCodeHotBuild()
            .AddSxcEngines();

        l.A("Will start with 2sxc Core");
        services
            .AddSxcImages()
            .AddSxcApps()
            .AddSxcEdit()
            .AddSxcData()
            .AddSxcAdam()
            .AddSxcAdamWork<int, int>()
            .AddSxcBlocks()
            .AddSxcRender()
            .AddSxcCms()
            .AddSxcServices()
            .AddSxcWeb()
            .AddSxcLightSpeed()
            .AddSxcCodeGen() // Code generation services
            .AddSxcCoreNew();

        l.A("Will start with 2sxc Fallbacks and RazorBlade parts");
        services
            .AddSxcAppsFallbackServices()
            .AddSxcCoreFallbackServices()
            .AddRazorBlade();

        l.A("Will start with EAV and WebApi Typed parts");
        services
            .AddEavEverything()
            .AddEavEverythingFallbacks()
            .AddEavWebApiTypedAfterEav();

        // Remember this for later, when we must start the Static Dependency Injection
        OriginalServiceCollection = services;

        _alreadyRegistered = true;
        l.Done();
        return services;
    }

    public static IServiceCollection OriginalServiceCollection;

    public static IServiceCollection AddDnnPlugins(this IServiceCollection services)
    {
        // Integrate KOI Dnn-Parts
        services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, Connect.Koi.Dnn.DetectAndCacheDnnThemeCssFramework>();
        return services;
    }


#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    public static IServiceCollection AddObsoleteServicesAndKits(this IServiceCollection services)
    {
        services.TryAddTransient<ToSic.Sxc.Web.IPageService, WebPageServiceObsolete>();  // Obsolete version, needed to keep old Apps working which used this
        return services;
    }
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
}