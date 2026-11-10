using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Run.Startup;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.StartUp;
using ToSic.Sxc.Run.Startup;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using Log = ToSic.Sys.Logging.Log;

namespace ToSic.Sxc.Oqt.StartUp;

public class OqtCodeServicesTests
{
    /// <summary>
    /// Ensure Oqtane can construct the supported typed API after the legacy dynamic service moved to DNN.
    /// This catches missing DI registrations or constructor dependencies before app code requests the service.
    /// </summary>
    [Fact]
    public void GetRequiredService_ResolvesTypedApi_WithOqtaneRegistrations()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ITypedApiService>();

        NotNull(service);
    }

    /// <summary>
    /// Check that a Custom.Hybrid.Code12 owner selects Oqtane's non-generic execution context.
    /// This protects platform-specific context selection for the shared legacy base used by apps such as Faq4.
    /// This smoke test covers context creation; it does not compile app files or exercise Razor CreateInstance().
    /// </summary>
    [Fact]
    public void New_CreatesOqtaneExecutionContext_ForLegacyCode()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<IExecutionContextFactory>().New(new()
        {
            OwnerOrNull = new LegacyCode(),
            ParentLog = new Log("Test.Legacy"),
        });

        IsType<OqtExecutionContext>(context);
    }

    /// <summary>
    /// Keep typed code on Oqtane's generic execution context with the requested ServiceKit16.
    /// The factory catches generic construction failures and falls back to a regular context,
    /// so checking the exact context detects a failure that would otherwise silently lose the requested kit.
    /// </summary>
    [Fact]
    public void New_CreatesOqtaneExecutionContextWithKit_ForTypedCode()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<IExecutionContextFactory>().New(new()
        {
            OwnerOrNull = new TypedCode(),
            ParentLog = new Log("Test.Typed"),
        });

        IsType<OqtExecutionContext<object, ServiceKit16>>(context);
    }

    private static ServiceProvider CreateServices()
        => new ServiceCollection()
            .AddLogging()
            .AddHttpContextAccessor()
            .AddScoped<IAliasAccessor, AliasAccessor>()
            .AddScoped(_ => new SiteState { Alias = new Alias { AliasId = 1, SiteId = 1, TenantId = 1, Name = "localhost" } })
            // Register the Oqtane overrides before the shared services, as OqtStartup does.
            .AddSxcOqtane()
            .AddSxcCode()
            .AddSxcApps()
            .AddSxcData()
            .AddSxcCore()
            .AddSxcRender()
            .AddSxcEngines()
            .AddSxcServices()
            .AddSxcWeb()
            .AddSxcCodeFallbacks()
            .AddEavAll()
            .AddEavAllFallbacks()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

    private sealed class LegacyCode : Custom.Hybrid.Code12;

    private sealed class TypedCode : Custom.Hybrid.CodeTyped;
}
