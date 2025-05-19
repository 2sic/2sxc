using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Startup;
using ToSic.Eav.StartUp;
using ToSic.Lib.Internal.FeatSys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Images;
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
            // First add the "Fallbacks" since these don't have Dnn/Oqtane specific implementations
            // Without this, some of the link tests will fail
            .AddSxcCoreFallbackServices()

            // 2sxc core
            .AddSxcApps()
            .AddSxcEdit()
            .AddSxcData()
            .AddSxcAdam()
            .AddSxcAdamWork<int, int>()
            .AddSxcBlocks()
            .AddSxcRender()
            .AddSxcCoreNew()
            .AddSxcCms()
            .AddSxcImages()
            .AddSxcServices()
            .AddSxcWeb()
            .AddSxcCode()
            .AddSxcLightSpeed()
            
            .AddLibCore()
            .AddEavDataPersistence()
            .AddEavDataBuild()
            .AddEavCore()
            .AddEavCoreFallbackServices()
            .AddLibFeatSys();
}