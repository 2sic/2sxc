using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.StartUp;

internal static partial class OqtRegisterServices
{
    public static IServiceCollection AddSxcOqtane(this IServiceCollection services)
    {
        services
            .AddOqtanePlumbing()                        // Helpers to make State work etc.
            .AddSxcOqtPathsAndPlatform()                // Paths and PlatformInfo
            .AddSxcOqtContext()                         // Context objects like ISite etc.
            .AddSxcOqtAppPermissionsAndImportExport()   // App stuff
            .AddOqtaneLookUpsAndSources()               // Lookups / LookUp Engine
            .AddSxcOqtIntegratedServices()              // Oqtane Integration like Logging & Send-Mail
            .AddOqtaneInstallation()                    // Installation-complete checks and similar
            .AddOqtaneBlazorWebAssemblySupport()        // Oqtane client Blazor WebAssembly support on Oqtane server
            .AddOqtaneApiPlumbingAndHelpers()           // Things to make Oqtane APIs work
            .AddSxcOqtApiParts()                        // Overrides etc. for Sxc Objects in the APIs
            .AddSxcOqtDynCodeAndViews()                 // Stuff so the Dyn-Code and views work
            .AddSxcOqtModule()                          // Module capabilities
            .AddSxcOqtLookUps()
            .AddRazorDependencies()                     // Razor functionality
            .AddSxcOqtAdam()                            // Adam
            ;

        services.TryAddTransient<IUiContextBuilder, OqtUiContextBuilder>();

        return services;
    }
        
}