using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Security.Encryption;
using ToSic.Sxc.Context;
using ToSic.Sxc.Integration.Installation;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Oqt.Server.Code.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.Services;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{
    /// <summary>
    /// Path resolvers and IPlatform information
    /// </summary>
    private static IServiceCollection AddSxcOqtPathsAndPlatform(this IServiceCollection services)
    {
        services.TryAddTransient<IServerPaths, OqtServerPaths>();
        services.AddScoped<ILinkPaths, OqtLinkPaths>();
        services.TryAddTransient<Compiler>();

        // TODO: Review - it looks a bit fishy to have the same class as singleton and transient
        services.AddSingleton<IPlatform, OqtPlatformContext>();
        services.TryAddTransient<IPlatformInfo, OqtPlatformContext>();
        return services;
    }
        
    /// <summary>
    /// Plumbing helpers to make sure everything can work, the context is ready etc.
    /// </summary>
    private static IServiceCollection AddOqtanePlumbing(this IServiceCollection services)
    {
        // Helper to access settings of a Site, Module etc.
        services.TryAddTransient<SettingsHelper>();

        // Helper to get header, query string and route information from current request
        //services.TryAddScoped<RequestHelper>();

        // Manage oqtane site culture info
        services.TryAddTransient<OqtCulture>();

        // Site State Initializer for APIs etc. to ensure that the SiteState exists and is correctly preloaded
        services.TryAddTransient<AliasResolver>();

        // Views / Templates / Razor: Get url params in the request
        services.TryAddTransient<IHttp, HttpBlazor>();

        // Special Key generator for security implementation, which doesn't exist in .net standard
        services.TryAddTransient<Rfc2898Generator, Rfc2898NetCoreGenerator>();

        return services;
    }
        

    private static IServiceCollection AddOqtaneInstallation(this IServiceCollection services)
    {
        // Installation: Helper to ensure the installation is complete
        services.TryAddTransient<IEnvironmentInstaller, OqtEnvironmentInstaller>();
        services.TryAddTransient<IPlatformAppInstaller, OqtEnvironmentInstaller>();

        // Installation: Verify the Razor Helper DLLs are available
        services.TryAddSingleton<GlobalTypesCheck>();

        return services;
    }
    private static IServiceCollection AddOqtaneBlazorWebAssemblySupport(this IServiceCollection services)
    {
        // following registrations in the server project will override the previous one in client project
        services.AddScoped<IOqtDebugStateService, OqtDebugStateService>();
        services.AddScoped<IOqtPageChangesOnServerService, OqtPageChangesOnServerService>();
        services.AddScoped<IOqtPrerenderService, OqtPrerenderService>();
        services.AddScoped<IOqtSxcRenderService, OqtSxcRenderService>();
        services.AddScoped<IRenderInfoService, RenderInfoService>();
        services.AddScoped<IOqtTurnOnService, OqtTurnOnService>();

        return services;
    }
}