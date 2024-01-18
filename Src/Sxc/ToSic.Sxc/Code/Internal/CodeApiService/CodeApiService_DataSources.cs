using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.LookUp;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService
{
    #region DataSource and ConfigurationProvider (for DS) section

    [PrivateApi]
    internal ILookUpEngine LookUpForDataSources => _lookupEngine.Get(() =>
        // check if we have a block-context, in which case the lookups also know about the module
        Data?.Configuration?.LookUpEngine
        // otherwise try to fallback to the App configuration provider, which has a lot, but not the module-context
        ?? App?.ConfigurationProvider
        // show explanation what went wrong
        ?? throw new("Tried to get Lookups for creating data-sources; neither module-context nor app is known.")
    );
    private readonly GetOnce<ILookUpEngine> _lookupEngine = new();

    [PrivateApi]
    public CodeCreateDataSourceSvc DataSources => _dataSources.Get(() => Services.DataSources.Value.Setup(App, () => LookUpForDataSources));
    private readonly GetOnce<CodeCreateDataSourceSvc> _dataSources = new();


    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource 
        => DataSources.CreateDataSource<T>(false, attach: inSource, options: configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
    {
        // if it has a source, then use this, otherwise it's null and then it uses the App-Default
        // Reason: some sources like DataTable or SQL won't have an upstream source
        var src = CreateSource<T>(source.Source);
        src.Attach(DataSourceConstants.StreamDefaultName, source);
        return src;
    }

    #endregion
}