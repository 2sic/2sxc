using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Integration;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

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
public class StartupSxcWithDb
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
            .AddEavEverything();
            
}