using ToSic.Eav.Caching;
using ToSic.Eav.DataSource;

// ReSharper disable once CheckNamespace
namespace Custom.DataSource;

public abstract partial class DataSource16
{
    #region Explicit IDataSource Implementation

    Guid IDataSource.Guid => _inner.Guid;
    string IDataSource.Name => GetType().Name;
    string IDataSource.Label => _inner.Label;
    void IDataSource.AddDebugInfo(Guid? guid, string label) => _inner.AddDebugInfo(guid, label);
    string ICacheKey.CachePartialKey => _inner.CachePartialKey;
    string ICacheKey.CacheFullKey => _inner.CacheFullKey;
    long ITimestamped.CacheTimestamp => _inner.CacheTimestamp;
    bool ICacheExpiring.CacheChanged(long dependentTimeStamp) => _inner.CacheChanged(dependentTimeStamp);
    //void ICanPurgeListCache.PurgeList(bool cascade) => _inner.PurgeList(cascade);
    IReadOnlyDictionary<string, IDataStream> IDataSource.Out => _inner.Out;
    IDataStream IDataSource.this[string outName] => _inner[outName];
    IDataStream IDataSource.GetStream(string name, NoParamOrder noParamOrder, bool nullIfNotFound, bool emptyIfNotFound) =>
        _inner.GetStream(name, noParamOrder, nullIfNotFound, emptyIfNotFound);
    IEnumerable<IEntity> IDataSource.List => _inner.List;
    public void Setup(IDataSourceOptions options, IDataSourceLinkable attach) => _inner.Setup(options, attach);

    List<string> IDataSource.CacheRelevantConfigurations => _inner.CacheRelevantConfigurations;
    ICacheKeyManager IDataSource.CacheKey => _inner.CacheKey;
    bool IDataSource.Immutable => _inner.Immutable;
    void IDataSource.DoWhileOverrideImmutable(Action action) => _inner.DoWhileOverrideImmutable(action);

    ILog IHasLog.Log => _inner.Log;

    #endregion

    public IDataSourceLink Link => ((IDataSourceLinkable)_inner).Link;
}