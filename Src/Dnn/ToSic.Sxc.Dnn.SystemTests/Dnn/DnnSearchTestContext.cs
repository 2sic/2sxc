using System.Reflection;
using System.Web;
using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Search;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.DI;
using ToSic.Sys.Logging;

namespace ToSic.Sxc.Dnn;

internal sealed class DnnSearchTestContext : IDisposable
{
    // Native DNN and DnnStaticDi bootstrap once per AppDomain. Do not reset their private caches in shared tests.
    private readonly AppDomain _domain = AppDomain.CreateDomain(
        "DnnSearch-" + Guid.NewGuid().ToString("N"), null, AppDomain.CurrentDomain.SetupInformation);

    public DnnSearchProbe Probe { get; }

    public DnnSearchTestContext()
        => Probe = (DnnSearchProbe)_domain.CreateInstanceAndUnwrap(
            typeof(DnnSearchProbe).Assembly.FullName, typeof(DnnSearchProbe).FullName);

    public void Dispose() => AppDomain.Unload(_domain);
}

/// <summary>Exercises the real search entry in isolation from the host and other tests' static DNN state.</summary>
public sealed class DnnSearchProbe : MarshalByRefObject
{
    public DnnSearchProbeResult RunSearch(int ownerPortalId, bool isContent, bool failSearch)
    {
        var state = new SearchState();
        using var root = CreateServices(state, failSearch);
        var errors = DnnEnvironmentLogger.SearchErrorsCount;

        var documents = new DnnBusinessController().GetModifiedSearchDocumentsTac(Module(isContent, ownerPortalId), DateTime.MinValue);

        var result = Result(state, errors);
        result.Documents = documents.Count;
        return result;
    }

    public DnnSearchProbeResult RunRepeatedBackgroundSearch()
    {
        var state = new SearchState();
        using var root = CreateServices(state, failSearch: false);
        var controller = new DnnBusinessController();
        var errors = DnnEnvironmentLogger.SearchErrorsCount;

        controller.GetModifiedSearchDocumentsTac(Module(isContent: true, ownerPortalId: 0), DateTime.MinValue);
        controller.GetModifiedSearchDocumentsTac(Module(isContent: false, ownerPortalId: 7), DateTime.MinValue);

        return Result(state, errors);
    }

    private static DnnSearchProbeResult Result(SearchState state, int errorsBefore) => new()
    {
        Created = state.Created,
        DisposedAfterCall = state.Disposed,
        ErrorsAdded = DnnEnvironmentLogger.SearchErrorsCount - errorsBefore,
        RequestedZones = state.RequestedZones.ToArray(),
        ResolvedSentinels = state.ResolvedSentinels.ToArray(),
    };

    private ServiceProvider CreateServices(SearchState state, bool failSearch)
    {
        HttpContext.Current = null;
        var portals = new Mock<IPortalController>(MockBehavior.Strict);
        // Background search: like the DNN scheduler, there is no current portal.
        portals.Setup(p => p.GetCurrentPortalSettings()).Returns((PortalSettings)null!);
        portals.Setup(p => p.GetPortal(It.IsAny<int>())).Returns((int id) => new PortalInfo { PortalID = id });
        PortalController.SetTestableInstance(portals.Object);
        var settings = new Mock<IPortalSettingsController>(MockBehavior.Strict);
        settings.Setup(s => s.LoadPortalSettings(It.IsAny<PortalSettings>())).Callback((PortalSettings portal) =>
        {
            portal.CultureCode = "en-US";
            portal.DefaultLanguage = "en-US";
        });
        settings.Setup(s => s.LoadPortal(It.IsAny<PortalInfo>(), It.IsAny<PortalSettings>()));
        settings.Setup(s => s.GetActiveTab(It.IsAny<int>(), It.IsAny<PortalSettings>()))
            .Returns((int tabId, PortalSettings portal) => new TabInfo { TabID = tabId, PortalID = portal.PortalId });

        var services = new ServiceCollection();
        services.AddSingleton<ILogFactory>(LogFactory.Initialize(false));
        services.AddSingleton(settings.Object);
        services.AddSingleton(new Mock<IApplicationStatusInfo>(MockBehavior.Strict).Object);
        services.AddSingleton(new Mock<INavigationManager>(MockBehavior.Strict).Object);
        services.AddSingleton(new Mock<ILogStore>(MockBehavior.Strict).Object);
        var features = new Mock<ISysFeaturesService>(MockBehavior.Strict);
        features.Setup(f => f.IsEnabled(It.IsAny<Guid>())).Returns(false);
        services.AddSingleton(features.Object);
        var paths = new Mock<ILinkPaths>(MockBehavior.Strict);
        paths.Setup(p => p.GetCurrentRequestUrl()).Returns("https://search-tests.invalid/");
        services.AddSingleton(paths.Object);
        services.AddSingleton<IZoneOfSiteSvc, FakeZoneOfSiteSvc>();
        services.AddSingleton(new Mock<IContextOfBlockFactory>(MockBehavior.Strict).Object);
        services.AddSingleton(new Mock<IEditionService>(MockBehavior.Strict).Object);
        services.AddTransient(typeof(LazySvc<>));
        services.AddTransient(typeof(Generator<>));
        services.AddTransient<AppsCacheSwitch>();
        services.AddTransient<DnnCultureResolver>();
        services.AddTransient<DnnSiteFactory>();
        services.AddScoped<DnnSiteShareWip>();
        services.AddTransient<ISite>(sp => sp.GetRequiredService<DnnSiteShareWip>().TempSite);
        services.AddTransient<DnnModuleFactory>();
        services.AddTransient<DnnPageFactory>();
        services.AddTransient<DnnModuleContextFactory>();
        var catalog = new RecordingAppsCatalog(state.RequestedZones);
        services.AddSingleton<IAppsCatalog>(catalog);
        services.AddTransient(_ => new AppFinder(catalog, null!));
        services.AddScoped(_ => new DisposableSentinel(state));
        services.AddTransient(provider =>
        {
            // Observe resources resolved by the actual entry point, including failure before search initialization.
            state.ResolvedSentinels.Add(provider.GetRequiredService<DisposableSentinel>().Id);
            if (failSearch)
                throw new InvalidOperationException("Controlled search construction failure after resolving a scoped disposable.");
            return ActivatorUtilities.CreateInstance<SearchController>(provider);
        });

        var root = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        // The setter is internal to DNN. This AppDomain is owned exclusively by this one test.
        typeof(Globals).GetProperty("DependencyProvider", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, root);
        DnnStaticDi.StaticDiReady(root.GetRequiredService<ISysServiceProvider>);
        // Avoid native event logging in the isolated fixture; increments still reveal swallowed initialization errors.
        DnnEnvironmentLogger.SearchErrorsCount = DnnEnvironmentLogger.SearchErrorsMax + 1;
        return root;
    }

    private static ModuleInfo Module(bool isContent, int ownerPortalId)
    {
        var module = new ModuleInfo { OwnerPortalID = ownerPortalId, PortalID = 0 };
        module.DesktopModule.ModuleName = isContent ? "2sxc" : "2sxc-app";
        if (!isContent)
            module.ModuleSettings[ModuleSettingNames.AppName] = "deliberately-uninstalled-app";
        return module;
    }

    private sealed class SearchState
    {
        public int Created;
        public int Disposed;
        public List<int> RequestedZones { get; } = [];
        public List<int> ResolvedSentinels { get; } = [];
    }

    private sealed class DisposableSentinel(SearchState state) : IDisposable
    {
        public int Id { get; } = ++state.Created;
        public void Dispose() => state.Disposed++;
    }

    private sealed class FakeZoneOfSiteSvc : IZoneOfSiteSvc
    {
        public int GetZoneId(int siteId) => siteId + 100;
    }

    private sealed class RecordingAppsCatalog(List<int> requestedZones) : IAppsCatalog
    {
        public IReadOnlyDictionary<int, Zone> Zones { get; } = new Dictionary<int, Zone>
        {
            [100] = new(100, 1, AppConstants.AppIdNotFound, new Dictionary<int, string>(), []),
            [107] = new(107, 2, AppConstants.AppIdNotFound, new Dictionary<int, string>(), []),
        };

        public Zone Zone(int zoneId)
        {
            requestedZones.Add(zoneId);
            return Zones[zoneId];
        }

        public IReadOnlyDictionary<int, string> Apps(int zoneId) => Zone(zoneId).Apps;
        public IAppIdentityPure DefaultAppIdentity(int zoneId) => new AppIdentityPure(zoneId, Zone(zoneId).DefaultAppId);
        public IAppIdentityPure PrimaryAppIdentity(int zoneId) => throw new NotSupportedException();
        public IAppIdentityPure AppIdentity(int appId) => throw new NotSupportedException();
        public string AppNameId(IAppIdentity appIdentity) => throw new NotSupportedException();
    }
}

/// <summary>Only stable values cross the isolated AppDomain boundary.</summary>
[Serializable]
public sealed class DnnSearchProbeResult
{
    public int Documents { get; set; }
    public int ErrorsAdded { get; set; }
    public int Created { get; set; }
    public int DisposedAfterCall { get; set; }
    public int[] RequestedZones { get; set; } = [];
    public int[] ResolvedSentinels { get; set; } = [];
}
