using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.Environment.Sys.Permissions;
using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.DataSources.Sys.Sources;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Services;
using ToSic.Sys.Users;

namespace ToSic.Sxc.WebApi.Tests.DataSources;

internal sealed class EntityPickerTestContext : IDisposable
{
    private const string TypeName = "PickerItem";
    private readonly ServiceProvider _builderServices;
    private readonly ServiceProvider _services;
    private readonly IAppReader _app;
    private readonly IAppReader _otherApp;
    private readonly ISite _site = MockSiteTestHelpers.CreateSite("unused");
    private readonly IUser _user;

    public int[] EntityIds { get; } = [1, 2];

    public EntityPickerTestContext(bool isSystemAdmin = false)
    {
        var services = new ServiceCollection();
        new StartupCoreDataSourcesAndTestData().ConfigureServices(services);
        services.AddEavWork();
        _builderServices = services.BuildServiceProvider();
        var appId = Math.Abs(Guid.NewGuid().GetHashCode() / 2) + 100;
        _app = CreateApp(appId);
        _otherApp = CreateApp(appId + 1);
        _user = new UserMock { Id = 8, IsAnonymous = false, IsSystemAdmin = isSystemAdmin };

        services.AddSingleton(_site);
        services.AddSingleton(_user);
        services.AddSingleton<IContextOfSite>(AppContext());
        services.AddSingleton<IAppReaderFactory>(new FakeAppReaderFactory(_app, _otherApp));
        services.AddTransient<IContextOfAppFactory, ContextOfAppFactory>();
        services.AddTransient<ContextOfAppFactory.Dependencies>();
        services.AddTransient<IEnvironmentPermission, FakeEnvironmentPermission>();
        services.AddTransient<EntityPicker>();
        _services = services.BuildServiceProvider();
    }

    public EntityPicker CreatePicker(IContextOfApp? context, bool filterByType)
    {
        var sources = context == null ? new List<ILookUp>() : [new EntityPickerContext(context)];
        var queryLookup = new LookUpEngine(new Log("Tst.Picker"), sources: sources);
        // Query parts receive derived lookup engines; the prepared context must survive that chain.
        var partLookup = new LookUpEngine(queryLookup, new Log("Tst.Part"));
        return _services.GetRequiredService<IDataSourcesService>().Create<EntityPicker>(new DataSourceOptions
        {
            AppIdentityOrReader = _app,
            LookUp = partLookup,
            MyConfigValues = new Dictionary<string, string>
            {
                [nameof(EntityPicker.TypeNames)] = filterByType ? TypeName : "",
                [nameof(EntityPicker.ItemIds)] = "",
            }.ToImmutableDictionary(),
        });
    }

    public IContextOfApp AppContext() => new ContextOfApp(_site, _user, _app)
    {
        InternalMachinery = null!,
        SettingsStackSvc = null!,
    };

    public IContextOfApp BlockContext(bool canEdit = true, bool otherApp = false)
        => new FakeBlockContext(_site, _user, otherApp ? _otherApp : _app, canEdit)
        {
            InternalMachinery = null!,
            SettingsStackSvc = null!,
        };

    private IAppReader CreateApp(int appId)
    {
        var builder = _builderServices.GetRequiredService<IAppStateBuilder>()
            .InitForNewApp(new ParentAppState(null, false, false), new AppIdentity(_site.ZoneId, appId),
                Guid.NewGuid().ToString(), new Log("Tst.App"));
        var type = _builderServices.GetRequiredService<ContentTypeAssemblyKit>().Type.CreateContentTypeTac(
            appId: appId, name: TypeName, nameId: TypeName, scope: ScopeConstants.Default, attributes: []);
        var assembler = _builderServices.GetRequiredService<DataAssembler>();
        builder.Load("picker test data", _ =>
        {
            builder.InitMetadata();
            builder.InitContentTypes([type]);
            builder.SetNameAndFolder("PickerTest", "PickerTest");
            foreach (var id in EntityIds)
                builder.Add(assembler.CreateEntityTac(appId, type, entityId: id, repositoryId: id,
                    guid: Guid.NewGuid(), isPublished: id == 1), publishedId: null, log: false);
        });
        return builder.Reader;
    }

    public void Dispose()
    {
        _services.Dispose();
        _builderServices.Dispose();
    }

    private sealed record FakeBlockContext(ISite Site, IUser User, IAppReader AppReader, bool CanEdit)
        : ContextOfApp(Site, User, AppReader), IContextOfBlock
    {
        public IModule Module { get; } = new FakeModule();
        public IPage Page => throw new NotSupportedException();
        public BlockPublishingSettings Publishing => throw new NotSupportedException();
        public IPageServiceShared PageServiceShared => throw new NotSupportedException();
        IContextOfSite IContextOfSite.WithSwappedSite(ISite site) => this with { Site = site };
    }

    private sealed class FakeModule : IModule
    {
        public int Id => 42;
        public bool IsContent => false;
        public IBlockIdentifier? BlockIdentifier => null;
    }

    private sealed class FakeEnvironmentPermission() : ServiceWithSetup<EnvironmentPermissionOptions>("Tst.Env"), IEnvironmentPermission
    {
        public void Setup(EnvironmentPermissionOptionsBase options) => base.Setup((EnvironmentPermissionOptions)options);

        public PermissionCheckInfo EnvironmentAllows(List<Grants> grants)
        {
            var allowed = new EnvironmentPermissionsLogic(Log).EnvironmentAllows(grants, MyOptions,
                userIsModuleAdmin: () => false,
                userIsModuleEditor: () => MyOptions.ContextOfSiteOrBlock is FakeBlockContext { CanEdit: true });
            return new(allowed, allowed ? Conditions.EnvironmentGlobal : Conditions.Undefined);
        }

        public bool VerifyConditionOfEnvironment(string condition) => false;
    }

    private sealed class FakeAppReaderFactory(params IAppReader[] readers) : IAppReaderFactory
    {
        public IAppReader GetOrKeep(IAppIdentity app) => app as IAppReader ?? Get(app);
        public IAppReader Get(IAppIdentity app) => readers.Single(r => r.AppId == app.AppId && r.ZoneId == app.ZoneId);
        public IAppReader Get(int appId) => readers.Single(r => r.AppId == appId);
        public IAppReader? TryGet(IAppIdentity app) => Get(app);
        public IAppIdentityPure AppIdentity(int appId) => Get(appId).PureIdentity();
        public IAppReader GetZonePrimary(int zoneId) => throw new NotSupportedException();
        public IAppReader GetSystemPreset() => throw new NotSupportedException();
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded) => null;
        public IAppReader? ToReader(IAppStateCache state) => throw new NotSupportedException();
    }
}
