using ToSic.Eav.DataSource;
using ToSic.Sys.Caching;
using ToSic.Sys.Caching.Keys;

namespace Custom.DataSource;

public abstract partial class DataSource16
{
    // These APIs must work, but are typically handled as if they were explicit, to hid them in the docs.
    // In the old dynamic code, this still worked,
    // but in the typed code, these APIs are not available unless you cast to IDataSource.
    // Because of this in v19.01 we'll change many to be not-explicit, but hide from docs.

    #region Visual Query Properties, explicit only

    Guid IDataSource.Guid => _inner.Guid;

    string IDataSource.Name => GetType().Name;
    string IDataSource.Label => _inner.Label;
    void IDataSource.AddDebugInfo(Guid? guid, string? label)
        => _inner.AddDebugInfo(guid, label);

    #endregion

    #region Explicit IDataSource Implementation - mainly "explicit" to hide in the docs, so the relevant APIs are visible.

    [PrivateApi("Hide in docs to only show important APIs for DataSource creators")]
    public IReadOnlyDictionary<string, IDataStream> Out => _inner.Out;

    [Obsolete("This is an old API, better use GetStream(...) as it provides more options to handle errors.")]
    IDataStream IDataSource.this[string outName] => _inner[outName]!;

    /// <inheritdoc />
    [PrivateApi("Hide in docs to only show important APIs for DataSource creators")]
    public IDataStream? GetStream(string? name = null, NoParamOrder npo = default, bool nullIfNotFound = false, bool emptyIfNotFound = false)
        => _inner.GetStream(name, npo, nullIfNotFound, emptyIfNotFound);

    /// <inheritdoc />
    [PrivateApi("Hide in docs to only show important APIs for DataSource creators")]
    public IEnumerable<IEntity> List => _inner.List;

    // Note: changed to explicit in v19.01; not sure why it was not explicit before
    void IServiceWithSetup<IDataSourceOptions>.Setup(IDataSourceOptions options)
        => ((IServiceWithSetup<IDataSourceOptions>)_inner).Setup(options);

    ILog IHasLog.Log => _inner.Log;

    #endregion

    [PrivateApi("Hide in docs to only show important APIs for DataSource creators")]
    public IDataSourceLink GetLink() => ((IDataSourceLinkable)_inner).GetLink();

    #region Caching stuff - all explicit as the DataSource Developer shouldn't need this

    string ICacheKey.CachePartialKey => _inner.CachePartialKey;
    string ICacheKey.CacheFullKey => _inner.CacheFullKey;
    long ITimestamped.CacheTimestamp => _inner.CacheTimestamp;
    bool ICacheExpiring.CacheChanged(long dependentTimeStamp) => _inner.CacheChanged(dependentTimeStamp);
    ICacheKeyManager IDataSource.CacheKey => _inner.CacheKey;
    List<string> IDataSource.CacheRelevantConfigurations => _inner.CacheRelevantConfigurations;

    #endregion

    #region Immutability stuff, all explicit

    bool IDataSource.Immutable => _inner.Immutable;
    void IDataSource.DoWhileOverrideImmutable(Action action) => _inner.DoWhileOverrideImmutable(action);

    #endregion
}