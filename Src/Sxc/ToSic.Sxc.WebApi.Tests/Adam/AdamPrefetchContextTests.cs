using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Adam.Sys.Work;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.Cms.Load.Activities;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Run.Startup;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi.Tests.DataSources;
using ToSic.Sys.Services;
using Xunit;

namespace ToSic.Sxc.WebApi.Tests.Adam;

public class AdamPrefetchContextTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void Prefetch_UsesRequestedApp_AfterEditLoadPreparesContext(bool otherApp, bool prioritizeModule)
    {
        using var data = new EntityPickerTestContext();
        var original = (IContextOfBlock)data.BlockContext();
        var target = (IContextOfBlock)data.BlockContext(otherApp: otherApp);
        var services = new ServiceCollection();
        new StartupCoreDataSourcesAndTestData().ConfigureServices(services);
        services.AddSxcBlocks();
        services.AddSingleton(original.Site);
        services.AddSingleton(original.User);
        services.AddSingleton<IContextOfSite>(original);
        services.AddSingleton<IAppReaderFactory>(new FakeAppReaders(original.AppReaderRequired, target.AppReaderRequired));
        services.AddSingleton<IContextOfBlockFactory>(new FakeBlockFactory(target));
        services.AddSingleton<IFeaturesService>(new FakeFeatures(prioritizeModule));
        var capture = new CapturingPrefetch();
        services.AddSingleton<IAdamPrefetchHelper>(capture);
        using var provider = services.BuildServiceProvider();
        var current = provider.GetRequiredService<ISxcCurrentContextService>();
        current.ShareBlockSpecs(new BlockSpecs
        {
            AppId = original.AppReaderRequired.AppId,
            ZoneId = original.AppReaderRequired.ZoneId,
            Context = original,
            IsInnerBlock = false,
            IsContentApp = false,
        });

        // EditLoadBackend resolves the requested app before calling the prefetch activity.
        var resolved = current.ShareAppStateOptimized(target.AppReaderRequired.AppId);
        var activity = new EditLoadActivityAddPrefetch(
            provider.GetRequiredService<Generator<HyperlinkBackend>>(),
            provider.GetRequiredService<Generator<IAdamPrefetchHelper, AdamWorkOptions>>(),
            null!, current);
        activity.TryToPrefectAdditionalData(resolved.AppReaderRequired.AppId, new EditLoadDto
        {
            Items =
            [
                new BundleWithHeaderOptional<JsonEntity>
                {
                    Entity = new JsonEntity
                    {
                        Guid = Guid.NewGuid(),
                        Type = new JsonType { Name = "PickerItem", Id = "PickerItem" },
                        Attributes = new JsonAttributes
                        {
                            String = new() { ["Body"] = new() { ["en-us"] = "ADAM prefetch context test" } },
                        },
                    },
                },
            ],
        });

        Assert.NotNull(capture.MyOptions.Context);
        Assert.Equal(capture.MyOptions.AppId, capture.MyOptions.Context.AppReaderRequired.AppId);
        Assert.Equal(resolved is IContextOfBlock, capture.MyOptions.Context is IContextOfBlock);
    }

    private sealed class CapturingPrefetch : ServiceWithSetup<AdamWorkOptions>, IAdamPrefetchHelper
    {
        public CapturingPrefetch() : base("Tst.Adam") { }

        public ICollection<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true) => [];
    }

    private sealed class FakeFeatures(bool enabled) : ServiceBase("Tst.Feat"), IFeaturesService
    {
        public bool Debug { get; set; }
        public bool IsEnabled(params string[] nameIds) => enabled;
    }

    private sealed class FakeBlockFactory(IContextOfBlock target) : IContextOfBlockFactory
    {
        public IContextOfBlock Update(IContextOfBlock contextOfBlock, IAppIdentity appIdentity) => target;
        public IContextOfBlock Create(ISite site, IPage page, IModule module) => throw new NotSupportedException();
    }

    private sealed class FakeAppReaders(IAppReader original, IAppReader target) : IAppReaderFactory
    {
        public IAppReader GetOrKeep(IAppIdentity app) => app as IAppReader ?? Get(app);
        public IAppReader Get(IAppIdentity app) => Get(app.AppId);
        public IAppReader Get(int appId) => appId == target.AppId ? target : original;
        public IAppReader? TryGet(IAppIdentity app) => Get(app);
        public IAppIdentityPure AppIdentity(int appId) => Get(appId).PureIdentity();
        public IAppReader GetZonePrimary(int zoneId) => throw new NotSupportedException();
        public IAppReader GetSystemPreset() => throw new NotSupportedException();
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded) => null;
        public IAppReader? ToReader(IAppStateCache state) => throw new NotSupportedException();
    }
}
