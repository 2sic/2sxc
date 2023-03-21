using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Caching;
using ToSic.Eav.Conventions;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Caching;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace

// TODO:
// - ability to get in-stream...?
namespace Custom.DataSources
{
    public abstract class DataSource15: IDataSource, IDataTarget, IGetAccessors<string>, ISetAccessors<string>, ISetAccessors<object>
    {
        [PrivateApi]
        public class MyServices: MyServicesBase<CustomDataSourceAdvanced.MyServices>
        {
            public MyServices(CustomDataSourceAdvanced.MyServices parentServices) : base(parentServices)
            {
            }
        }

        protected DataSource15(MyServices services): this(services, null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// You need to call this constructor in your code to ensure that
        /// </summary>
        /// <param name="services"></param>
        /// <param name="logName"></param>
        protected DataSource15(MyServices services, string logName = null)
        {
            _inner = BreachExtensions.CustomDataSourceLight(services.ParentServices, logName ?? "Cus.HybDs");
            _inner.BreachAutoLoadAllConfigMasks(GetType());
        }

        private readonly CustomDataSourceLight _inner;

        protected void ProvideOut<T>(
            Func<IEnumerable<IHasRawEntity<T>>> source,
            string noParamOrder = Parameters.Protector,
            DataFactoryOptions options = default) where T : IRawEntity =>
            _inner.BreachProvideOut(source, options: options);

        protected void ProvideOut<T>(
            Func<IEnumerable<T>> source,
            string noParamOrder = Parameters.Protector,
            DataFactoryOptions options = default) where T : IRawEntity =>
            _inner.BreachProvideOut(source, options: options);

        

        #region CodeLog

        public ICodeLog Log => _codeLog.Get(() => new CodeLog(_inner.Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        #endregion

        #region Public IDataSource Implementation

        public IDataSourceConfiguration Configuration => _inner.Configuration;

        #endregion

        #region Explicit IDataSource Implementation

        Guid IDataPartShared.Guid
        {
            get => _inner.Guid;
            set => _inner.Guid = value;
        }

        string IDataPartShared.Name => _inner.Name;

        string IDataPartShared.Label
        {
            get => _inner.Label;
            set => _inner.Label = value;
        }

        int IZoneIdentity.ZoneId => _inner.ZoneId;

        int IAppIdentityLight.AppId => _inner.AppId;

        string ICacheKey.CachePartialKey => _inner.CachePartialKey;

        string ICacheKey.CacheFullKey => _inner.CacheFullKey;

        long ITimestamped.CacheTimestamp => _inner.CacheTimestamp;

        bool ICacheExpiring.CacheChanged(long dependentTimeStamp)
        {
            return _inner.CacheChanged(dependentTimeStamp);
        }

        void ICanPurgeListCache.PurgeList(bool cascade)
        {
            _inner.PurgeList(cascade);
        }

        IDictionary<string, IDataStream> IDataSource.Out => _inner.Out;

        IDataStream IDataSource.this[string outName] => _inner[outName];

        IDataStream IDataSource.GetStream(string name, string noParamOrder, bool nullIfNotFound,
            bool emptyIfNotFound)
        {
            return _inner.GetStream(name, noParamOrder, nullIfNotFound, emptyIfNotFound);
        }

        IEnumerable<IEntity> IDataSource.List => _inner.List;

        List<string> IDataSource.CacheRelevantConfigurations
        {
            get => _inner.CacheRelevantConfigurations;
            set => _inner.CacheRelevantConfigurations = value;
        }

        ICacheKeyManager IDataSource.CacheKey => _inner.CacheKey;

        DataSourceErrorHelper IDataSource.Error => _inner.Error;

        ILog IHasLog.Log => _inner.Log;

        #endregion

        #region Get/Set settings

        
        public string Get(string name) => _inner.Get(name);

        public TValue Get<TValue>(string name) => _inner.Get<TValue>(name);

        public TValue Get<TValue>(string name, string noParamOrder = Parameters.Protector, TValue fallback = default) => _inner.Get(name, noParamOrder, fallback);

        public void Set(string name, string value) => _inner.Set(name, value);

        public void Set(string name, object value) => _inner.Set(name, (string)value);

        #endregion

        #region IDataTarget - all public

        public IDictionary<string, IDataStream> In => _inner.In;

        public void Attach(IDataSource dataSource) => _inner.Attach(dataSource);

        public void Attach(string streamName, IDataSource dataSource, string sourceName = DataSourceConstants.StreamDefaultName) => _inner.Attach(streamName, dataSource, sourceName);

        public void Attach(string streamName, IDataStream dataStream) => _inner.Attach(streamName, dataStream);

        public IImmutableList<IEntity> TryGetIn(string name = DataSourceConstants.StreamDefaultName) => _inner.TryGetIn(name);

        #endregion
    }
}
