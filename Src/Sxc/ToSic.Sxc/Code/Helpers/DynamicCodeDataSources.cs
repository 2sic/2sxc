using System;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Catalog;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Code.Helpers;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DynamicCodeDataSources
{
    public readonly LazySvc<IDataSourcesService> DataSources;
    public readonly LazySvc<DataSourceCatalog> Catalog;

    public DynamicCodeDataSources(LazySvc<IDataSourcesService> dataSources, LazySvc<DataSourceCatalog> catalog)
    {
        DataSources = dataSources;
        Catalog = catalog;
    }

    public DynamicCodeDataSources Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
    {
        AppIdentity = appIdentity;
        _getLookup = getLookup;
        return this;
    }

    public IAppIdentity AppIdentity { get; private set; }

    public ILookUpEngine LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
    private readonly GetOnce<ILookUpEngine> _lookupEngine = new();
    private Func<ILookUpEngine> _getLookup;

    // note: this code is almost identical to the IDataService code, except that `immutable` is a parameter
    // because old code left the DataSources to be mutable
    public T CreateDataSource<T>(bool immutable, NoParamOrder noParamOrder = default, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
    {
        //Protect(noParamOrder, $"{nameof(attach)}, {nameof(options)}");

        // If no in-source was provided, make sure that we create one from the current app
        attach ??= DataSources.Value.CreateDefault(new DataSourceOptions(appIdentity: AppIdentity, lookUp: LookUpEngine, immutable: true));
        var typedOptions = new DataSourceOptions.Converter().Create(new DataSourceOptions(lookUp: LookUpEngine, immutable: immutable), options);
        return DataSources.Value.Create<T>(attach: attach, options: typedOptions);
    }

}