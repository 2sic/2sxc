using System;
using System.Collections.Generic;
using ToSic.Eav.Caching;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Caching;
using ToSic.Lib.Logging;

// ReSharper disable once CheckNamespace
namespace Custom.DataSource
{
    public abstract partial class DataSource15
    {
        #region Explicit IDataSource Implementation

        Guid IDataSourceShared.Guid => _inner.Guid;
        string IDataSourceShared.Name => GetType().Name;
        string IDataSourceShared.Label => _inner.Label;
        void IDataSourceShared.AddDebugInfo(Guid? guid, string label) => _inner.AddDebugInfo(guid, label);
        string ICacheKey.CachePartialKey => _inner.CachePartialKey;
        string ICacheKey.CacheFullKey => _inner.CacheFullKey;
        long ITimestamped.CacheTimestamp => _inner.CacheTimestamp;
        bool ICacheExpiring.CacheChanged(long dependentTimeStamp) => _inner.CacheChanged(dependentTimeStamp);
        void ICanPurgeListCache.PurgeList(bool cascade) => _inner.PurgeList(cascade);
        IDictionary<string, IDataStream> IDataSourceSource.Out => _inner.Out;
        IDataStream IDataSourceSource.this[string outName] => _inner[outName];
        IDataStream IDataSourceSource.GetStream(string name, string noParamOrder, bool nullIfNotFound,
            bool emptyIfNotFound) =>
            _inner.GetStream(name, noParamOrder, nullIfNotFound, emptyIfNotFound);
        IEnumerable<IEntity> IDataSourceSource.List => _inner.List;
        public void Setup(IDataSourceOptions options, IDataSourceLinkable attach) => _inner.Setup(options, attach);

        List<string> IDataSourceSource.CacheRelevantConfigurations => _inner.CacheRelevantConfigurations;
        ICacheKeyManager IDataSourceSource.CacheKey => _inner.CacheKey;
        bool IDataSourceSource.Immutable => _inner.Immutable;

        ILog IHasLog.Log => _inner.Log;

        #endregion

        public IDataSourceLink Link => ((IDataSourceLinkable)_inner).Link;
    }
}
