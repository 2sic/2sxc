using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Caching;
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
namespace Custom.DataSources
{
    public abstract partial class DataSource15: IDataSource, IAppIdentitySync
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
            _inner = BreachExtensions.CustomDataSourceLight(services.ParentServices, this, logName ?? "Cus.HybDs");
            _inner.BreachProvideOut(GetDefault);
        }

        private readonly CustomDataSourceLight _inner;

        protected virtual IEnumerable<IRawEntity> GetDefault() => new List<IRawEntity>();


        protected void ProvideOut(Func<IEnumerable> getList,
            string noParamOrder = Parameters.Protector,
            string name = DataSourceConstants.StreamDefaultName,
            DataFactoryOptions options = default
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



        #region IDataTarget - all public

        public IDictionary<string, IDataStream> In => _inner.In;

        public void Attach(IDataSource dataSource) => _inner.Attach(dataSource);

        public void Attach(string streamName, IDataSource dataSource, string sourceName = DataSourceConstants.StreamDefaultName) => _inner.Attach(streamName, dataSource, sourceName);

        public void Attach(string streamName, IDataStream dataStream) => _inner.Attach(streamName, dataStream);

        public IImmutableList<IEntity> TryGetIn(string name = DataSourceConstants.StreamDefaultName) => _inner.TryGetIn(name);

        #endregion

        void IAppIdentitySync.UpdateAppIdentity(IAppIdentity appIdentity) => ((IAppIdentitySync)_inner).UpdateAppIdentity(appIdentity);
    }
}
