using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Helpers;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.DataSource
{
    public abstract partial class DataSource15: ServiceBase<DataSource15.MyServices>, IDataSource, IAppIdentitySync
    {
        /// <summary>
        /// These are dependencies of DataSource15.
        /// At the moment we could use something else, but this ensures that all users must have this
        /// in the constructor, so we can be sure we can add more dependencies as we need them.
        /// </summary>
        [PrivateApi]
        public class MyServices: MyServicesBase<CustomDataSource.MyServices>
        {
            public LazySvc<DynamicCodeDataSources> DataSources { get; }

            public MyServices(CustomDataSource.MyServices parentServices, LazySvc<DynamicCodeDataSources> dataSources) : base(parentServices)
            {
                ConnectServices(
                    DataSources = dataSources
                );
            }
        }

        /// <summary>
        /// Constructor with the option to provide a log name.
        /// </summary>
        /// <param name="services">All the needed services - see [](xref:NetCode.Conventions.MyServices)</param>
        /// <param name="logName">Optional name for logging such as `My.JsonDS`</param>
        protected DataSource15(MyServices services, string logName = default): base(services, logName ?? "Cus.HybDs")
        {
            _inner = BreachExtensions.CustomDataSourceLight(services.ParentServices, this, logName ?? "Cus.HybDs");
            _inner.BreachProvideOut(GetDefault);
        }
        private readonly CustomDataSource _inner;

        private DynamicCodeDataSources DataSources => _ds.Get(() =>
            Services.DataSources.Value.Setup(this, () => _inner.Configuration.LookUpEngine));
        private readonly GetOnce<DynamicCodeDataSources> _ds = new GetOnce<DynamicCodeDataSources>();

        protected virtual IEnumerable<IRawEntity> GetDefault() => new List<IRawEntity>();


        protected void ProvideOut(
            Func<IEnumerable> getList,
            string noParamOrder = Parameters.Protector,
            string name = DataSourceConstants.StreamDefaultName,
            Func<DataFactoryOptions> options = default
            )
            => _inner.BreachProvideOut(getList, name: name, options: options);


        #region CodeLog

        public ICodeLog Log => _codeLog.Get(() => new CodeLog(_inner.Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        #endregion

        #region Public IDataSource Implementation

        public IDataSourceConfiguration Configuration => _inner.Configuration;

        public DataSourceErrorHelper Error => _inner.Error;

        public int ZoneId => _inner.ZoneId;

        public int AppId => _inner.AppId;

        #endregion

        [PrivateApi]
        public T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
            => DataSources.CreateDataSource<T>(true, noParamOrder: noParamOrder, attach: attach, options: options);

        [PrivateApi]
        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default)
            => DataSources.CreateDataSource(noParamOrder: noParamOrder, name: name, attach: attach, options: options);



        #region IDataTarget - allmost all hidden

        /// <inheritdoc/>
        public IImmutableList<IEntity> TryGetIn(string name = DataSourceConstants.StreamDefaultName) => _inner.TryGetIn(name);

        // The rest is all explicit implementation only

        IDictionary<string, IDataStream> IDataTarget.In => _inner.In;
        IDictionary<string, IDataStream> IDataSourceTarget.In => _inner.In;

        // todo: attach must error - but only once the query has been optimized
        // note also that temporarily the old interface IDataTarget will already error
        // but soon the new one must too
        private static readonly string AttachNotSupported = $"Attach(...) is not supported on new data sources. Provide 'attach:' in CreateDataSource(...) instead";
        void IDataTarget.Attach(IDataSource dataSource) => throw new NotSupportedException(AttachNotSupported);

        void IDataSourceTarget.Attach(IDataSource dataSource) => _inner.Attach(dataSource);

        void IDataTarget.Attach(string streamName, IDataSource dataSource, string sourceName = DataSourceConstants.StreamDefaultName) => throw new NotSupportedException(AttachNotSupported);

        void IDataSourceTarget.Attach(string streamName, IDataSource dataSource, string sourceName = DataSourceConstants.StreamDefaultName) => _inner.Attach(streamName, dataSource, sourceName);

        void IDataTarget.Attach(string streamName, IDataStream dataStream) => throw new NotSupportedException(AttachNotSupported);

        void IDataSourceTarget.Attach(string streamName, IDataStream dataStream) => _inner.Attach(streamName, dataStream);


        #endregion

        void IAppIdentitySync.UpdateAppIdentity(IAppIdentity appIdentity) => ((IAppIdentitySync)_inner).UpdateAppIdentity(appIdentity);
    }
}
