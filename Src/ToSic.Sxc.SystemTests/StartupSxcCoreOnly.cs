using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Startup;
using ToSic.Eav.StartUp;
using ToSic.Lib.Internal.FeatSys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Startup;

#pragma warning disable CA1822

namespace ToSic.Sxc;

// NOTE: QUITE A FEW DUPLICATES OF THIS - may want to consolidate

/// <summary>
/// A Startup helper for tests which need Dependency-Injection setup for EAV Core.
/// </summary>
/// <remarks>
/// Use by adding this kind of attribute to your test class:
/// `[Startup(typeof(TestStartupEavCore))]`
/// </remarks>
public class StartupSxcCoreOnly
{
    /// <summary>
    /// Startup helper
    /// </summary>
    public virtual void ConfigureServices(IServiceCollection services) =>
        services
            .AddFixtureHelpers()
            .AddDataSourceTestHelpers()
            // 2sxc core
            .AddSxcCore()
            .AddSxcServices()
            .AddLibCore()
            .AddEavDataPersistence()
            .AddEavDataBuild()
            .AddEavCore()
            .AddEavCoreFallbackServices()
            .AddLibFeatSys();
}