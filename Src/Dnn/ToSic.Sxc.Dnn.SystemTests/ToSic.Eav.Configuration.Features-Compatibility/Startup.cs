using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSources;
using ToSic.Eav.Integration;
using ToSic.Eav.Repository.Efc;
using ToSic.Eav.StartUp;
using ToSic.Lib;
using ToSic.Sxc.Compatibility;
using ToSic.Sys;
using ToSic.Sys.Capabilities.Platform;
using ToSic.Testing.Shared.Platforms;

#pragma warning disable CA1822

namespace ToSic.Sxc.ToSic.Eav.Configuration.Features_Compatibility;

/// <summary>
/// A Startup helper for tests which need Dependency-Injection setup for EAV Core.
/// </summary>
/// <remarks>
/// Use by adding this kind of attribute to your test class:
/// `[Startup(typeof(StartupTestFullWithDb))]`
/// </remarks>
public class Startup
{
    /// <summary>
    /// Startup helper
    /// </summary>
    public virtual void ConfigureServices(IServiceCollection services) =>
        services
            .AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>()
            .AddDnnCompatibility()

            .AddFixtureHelpers()
            // Apps
            .AddEavWork()
            .AddEavContext()
            .AddEavAppsPersistence()
            .AddEavApps()

            // SQL Server
            .AddRepositoryAndEfc()
            // Import/Export as well as File Based Json loading
            .AddEavImportExport()
            .AddEavPersistence()
            // DataSources
            .AddDataSources()
            // EAV Core
            //.AddEavDataPersistence()
            .AddEavDataBuild()

            // EAV Core and Downstream
            .AddEavCoreLibAndSys()

            // Fallbacks for services which were not implemented - must come last
            .AddContextFallbackServices()
            .AddAppPersistenceFallbackServices()
            .AddAppFallbackServices()
            .AddEavImportExportFallback()
            .AddEavDataBuildFallbacks()
            .AddEavCoreLibAndSysFallbackServices();
}