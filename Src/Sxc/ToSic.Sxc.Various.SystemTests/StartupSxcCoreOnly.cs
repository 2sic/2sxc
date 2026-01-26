using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Run.Startup;
using ToSic.Sxc.Run.Startup;

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
            // First add the "Fallbacks" since these don't have Dnn/Oqtane specific implementations
            // Without this, some of the link tests will fail
            .AddSxcCoreFallbacks()

            // 2sxc core
            .AddSxcApps()
            .AddSxcEdit()
            .AddSxcData()
            .AddSxcAdam()
            .AddSxcAdamWork<int, int>()
            .AddSxcBlocks()
            .AddSxcRender()
            .AddSxcCore()
            .AddSxcCms()
            .AddSxcImages()
            .AddSxcServices()
            .AddSxcWeb()
            .AddSxcCode()
            .AddSxcLightSpeed()

            .AddEavPersistence()
            .AddEavDataBuild()
            .AddEavDataStack()
            .AddEavData()
            // EAV Core and Downstream
            .AddAllLibAndSys()

            .AddSxcAppsFallbacks()
            .AddEavDataBuildFallbacks()
            .AddEavDataFallbacks()
            .AddAllLibAndSysFallbacks();
}