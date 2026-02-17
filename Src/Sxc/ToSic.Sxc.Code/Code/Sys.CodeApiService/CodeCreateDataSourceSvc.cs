using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys.Catalog;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;

namespace ToSic.Sxc.Code.Sys.CodeApiService;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeCreateDataSourceSvc(LazySvc<IDataSourcesService> dataSources, LazySvc<DataSourceCatalog> catalog)
{
    public readonly LazySvc<IDataSourcesService> DataSources = dataSources;
    public readonly LazySvc<DataSourceCatalog> Catalog = catalog;

    public CodeCreateDataSourceSvc Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
    {
        AppIdentity = appIdentity;
        _getLookup = getLookup;
        return this;
    }

    public IAppIdentity AppIdentity { get; private set; } = null!;

    public ILookUpEngine? LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
    private readonly GetOnce<ILookUpEngine?> _lookupEngine = new();
    private Func<ILookUpEngine>? _getLookup;

    // note: this code is almost identical to the IDataService code, except that `immutable` is a parameter
    // because old code left the DataSources to be mutable
    public T CreateDataSource<T>(bool immutable, NoParamOrder npo = default, IDataSourceLinkable? attach = null, object? options = default) where T : IDataSource
    {
        // If no in-source was provided, make sure that we create one from the current app
        attach ??= DataSources.Value.CreateDefault(new DataSourceOptions
        {
            AppIdentityOrReader = AppIdentity,
            LookUp = LookUpEngine,
            Immutable = true,
        });
        var typedOptions = new DataSourceOptionConverter().Create(new DataSourceOptions
        {
            AppIdentityOrReader = AppIdentity,
            LookUp = LookUpEngine,
            Immutable = immutable,
            Attach = attach,
        }, options);
        return DataSources.Value.Create<T>(/*attach: attach,*/ options: typedOptions);
    }

}