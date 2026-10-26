using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Sys;
using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Run.Startup;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sys.Services;
using ToSic.Sys.Users;
using ToSic.Sys.Users.Permissions;

#if NETFRAMEWORK
using HttpContext = System.Web.HttpContext;
using HttpRequest = System.Web.HttpRequest;
#else
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
#endif

namespace ToSic.Sxc.WebApi.Tests.DataSources;

internal sealed class QueryDraftPermissionsTestContext : IDisposable
{
    public static readonly IAppIdentity App = new AppIdentity(1, 42);
    private readonly ServiceProvider _services;
    private readonly IContentType _type;
    private readonly DataAssembler _assembler;
    private IAppReader? _appReader;

    public QueryDraftPermissionsTestContext(bool ambientShowDrafts = false, string? cookie = null, bool? ambientContentAdmin = null)
    {
        var services = new ServiceCollection();
        new StartupCoreDataSourcesAndTestData().ConfigureServices(services);
        services.AddSxcBlocks();
        services.AddSingleton<IAppsCatalog>(new FakeAppsCatalog());
        services.AddSingleton<ICurrentContextUserPermissionsService>(new FakeUserPermissions(ambientShowDrafts, ambientContentAdmin));
        services.AddSingleton<IContextOfSite>(Context(ambientShowDrafts));
        services.AddSingleton<IHttp>(new FakeHttp(cookie));
        _services = services.BuildServiceProvider();
        _assembler = _services.GetRequiredService<DataAssembler>();
        var typeName = $"DraftTest{Guid.NewGuid():N}";
        _type = _services.GetRequiredService<ContentTypeAssemblyKit>().Type.CreateContentTypeTac(
            appId: App.AppId, name: typeName, nameId: typeName, attributes: []);
    }

    public ISxcCurrentContextService CurrentContext => _services.GetRequiredService<ISxcCurrentContextService>();

    public static IContextOfSite Context(bool showDrafts)
        => new FakeSiteContext(MockSiteTestHelpers.CreateSite("unused"),
            new UserMock { IsAnonymous = false }, new EffectivePermissions(false, showDrafts));

    public ILookUpEngine Lookup(bool? showDrafts, IAppIdentity? app = null)
    {
        var sources = new List<ILookUp>();
        if (showDrafts.HasValue)
            sources.Add(new QueryUserPermissions(app ?? App, new EffectivePermissions(false, showDrafts.Value)));
        var parent = new LookUpEngine(new Log("Tst.Query"), sources: sources);
        return new LookUpEngine(parent, new Log("Tst.Part"));
    }

    public PublishingFilter Filter(ILookUpEngine lookup, bool? explicitShowDrafts = null)
    {
        var source = new TestEntitiesSource(_services.GetRequiredService<DataSourceBase.Dependencies>(),
        [
            _assembler.CreateEntityTac(App.AppId, _type, entityId: 1, isPublished: true),
            _assembler.CreateEntityTac(App.AppId, _type, entityId: 2, isPublished: false),
        ]);
        ((IServiceWithSetup<IDataSourceOptions>)source).Setup(new DataSourceOptions
        {
            AppIdentityOrReader = App,
            LookUp = lookup,
        });
        var filter = _services.GetRequiredService<IDataSourcesService>().Create<PublishingFilter>(new DataSourceOptions
        {
            AppIdentityOrReader = App,
            LookUp = lookup,
            Attach = source,
        });
        if (explicitShowDrafts.HasValue)
            filter.ShowDrafts = explicitShowDrafts;
        return filter;
    }

    public IDataStream CachedTypeStream(ILookUpEngine lookup)
    {
        if (_appReader == null)
        {
            var builder = _services.GetRequiredService<IAppStateBuilder>()
                .InitForNewApp(new ParentAppState(null, false, false), App, Guid.NewGuid().ToString(), new Log("Tst.App"));
            builder.Load("query draft test types", _ =>
            {
                builder.InitMetadata();
                builder.InitContentTypes([_type]);
                builder.SetNameAndFolder("DraftTest", "DraftTest");
            });
            _appReader = builder.Reader;
        }

        var appSource = new ToSic.Eav.DataSources.App(new ToSic.Eav.DataSources.App.Dependencies(
            _services.GetRequiredService<DataSourceBase.Dependencies>(),
            new FakeAppReaderFactory(_appReader),
            _services.GetRequiredService<IDataSourcesService>(),
            _services.GetRequiredService<ICurrentContextUserPermissionsService>()));
        ((IServiceWithSetup<IDataSourceOptions>)appSource).Setup(new DataSourceOptions
        {
            AppIdentityOrReader = App,
            LookUp = lookup,
            Attach = Filter(lookup),
        });
        return appSource.Out[_type.Name];
    }

    public RelationshipDataSourceBase Relationships(ILookUpEngine lookup)
    {
        var related = Filter(lookup, explicitShowDrafts: true);
        var source = new TestRelationships(_services.GetRequiredService<DataSourceBase.Dependencies>(),
            _services.GetRequiredService<ICurrentContextUserPermissionsService>(), related.List);
        ((IServiceWithSetup<IDataSourceOptions>)source).Setup(new DataSourceOptions
        {
            AppIdentityOrReader = App,
            LookUp = lookup,
            Attach = related,
        });
        return source;
    }

    public bool QueryParameter(ILookUpEngine lookup)
    {
        var header = _assembler.CreateEntityTac(App.AppId, _type, entityId: 10, values: []);
        var definition = _services.GetRequiredService<QueryDefinitionFactory>().Create(App.AppId, header);
        var query = _services.GetRequiredService<QueryFactory>().CreateWithParams(definition, lookup);
        return bool.Parse(query.Main.Configuration.LookUpEngine.FindSource(DataSourceConstants.ParamsSourceName)!
            .Get(QueryConstants.ParamsShowDraftsKey, ""));
    }

    public void Dispose() => _services.Dispose();

    private sealed class TestEntitiesSource : DataSourceBase
    {
        public TestEntitiesSource(DataSourceBase.Dependencies services, IEnumerable<IEntity> items)
            : base(services, "Tst.Items")
            => ProvideOut(() => items);
    }

    private sealed class TestRelationships(
        DataSourceBase.Dependencies services,
        ICurrentContextUserPermissionsService permissions,
        IEnumerable<IEntity> related)
        : RelationshipDataSourceBase(services, permissions, "Tst.Rels")
    {
        public override string? FieldName => null;
        public override string? ContentTypeName => null;
        protected override Func<IEntity, IEnumerable<IEntity>> InnerGet(string? fieldName, string? typeName)
            => _ => related;
    }

    private sealed record FakeSiteContext(ISite Site, IUser User, EffectivePermissions Permissions) : IContextOfSite
    {
        public IContextOfSite WithSwappedSite(ISite site) => this with { Site = site };
    }

    private sealed class FakeUserPermissions(bool showDrafts, bool? contentAdmin) : ICurrentContextUserPermissionsService
    {
        public EffectivePermissions UserPermissions() => new(false, contentAdmin ?? showDrafts, showDrafts, showDrafts);
    }

    private sealed class FakeHttp(string? cookie) : IHttp
    {
        public HttpContext Current => throw new NotSupportedException();
        public HttpRequest Request => throw new NotSupportedException();
        public NameValueCollection QueryStringParams => new();
        public List<KeyValuePair<string, string>> QueryStringKeyValuePairs() => [];
        public string? GetCookie(string cookieName) => cookieName == "app-*-data-preview" ? cookie : null;
    }

    private sealed class FakeAppReaderFactory(IAppReader app) : IAppReaderFactory
    {
        public IAppReader GetOrKeep(IAppIdentity identity) => identity as IAppReader ?? app;
        public IAppReader Get(IAppIdentity identity) => app;
        public IAppReader Get(int appId) => app;
        public IAppReader? TryGet(IAppIdentity identity) => app;
        public IAppIdentityPure AppIdentity(int appId) => app.PureIdentity();
        public IAppReader GetZonePrimary(int zoneId) => throw new NotSupportedException();
        public IAppReader GetSystemPreset() => throw new NotSupportedException();
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded) => null;
        public IAppReader? ToReader(IAppStateCache state) => throw new NotSupportedException();
    }

    private sealed class FakeAppsCatalog : IAppsCatalog
    {
        public IAppIdentityPure AppIdentity(int appId) => new AppIdentity(App.ZoneId, appId).PureIdentity();
        public IReadOnlyDictionary<int, string> Apps(int zoneId) => throw new NotSupportedException();
        public IReadOnlyDictionary<int, Zone> Zones => throw new NotSupportedException();
        public Zone Zone(int zoneId) => throw new NotSupportedException();
        public IAppIdentityPure DefaultAppIdentity(int zoneId) => throw new NotSupportedException();
        public IAppIdentityPure PrimaryAppIdentity(int zoneId) => throw new NotSupportedException();
        public string AppNameId(IAppIdentity appIdentity) => throw new NotSupportedException();
    }
}
