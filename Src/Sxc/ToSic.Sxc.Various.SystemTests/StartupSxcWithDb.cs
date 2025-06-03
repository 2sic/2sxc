using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Integration;
using ToSic.Sxc.Code;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sys;

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
            .AddSxcCoreNew()
            .AddSxcApps()
            .AddSxcEdit()
            .AddSxcData()
            .AddSxcAdam()
            .AddSxcAdamWork<int, int>()
            .AddSxcBlocks()
            .AddSxcRender()
            .AddSxcCms()
            .AddSxcServices()
            .AddSxcCode()
            .AddSxcEngines()
            .AddSxcImages()
            .AddSxcWeb()
            .AddSxcLightSpeed()
            .AddSxcAppsFallbackServices()

            .AddEavEverything()
            .AddEavEverythingFallbacks();

}