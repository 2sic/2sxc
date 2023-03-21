using System;
using ToSic.Eav.Configuration;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region DataSource and ConfigurationProvider (for DS) section
        private ILookUpEngine _configurationProvider;

        [PrivateApi]
        public ILookUpEngine ConfigurationProvider
        {
            get
            {
                // check already retrieved
                if (_configurationProvider != null) return _configurationProvider;

                // check if we have a block-context, in which case the lookups also know about the module
                _configurationProvider = Data?.Configuration?.LookUpEngine;
                if (_configurationProvider != null) return _configurationProvider;

                // otherwise try to fallback to the App configuration provider, which has a lot, but not the module-context
                _configurationProvider = App?.ConfigurationProvider;
                if (_configurationProvider != null) return _configurationProvider;

                // show explanation what went wrong
                throw new Exception("Tried to get Lookups for creating a data-source, but neither the module-context nor app is known.");
            }
        }

        [PrivateApi]
        public IDataSourceFactory DataSourceFactory => _dataSourceFactory.Get(() => Services.DataSourceFactory.Value);
        private readonly GetOnce<IDataSourceFactory> _dataSourceFactory = new GetOnce<IDataSourceFactory>();



        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, IConfiguration configuration = default) where T : IDataSource
        {
            configuration = configuration ?? ConfigurationProvider;

            // If no in-source was provided, make sure that we create one from the current app
            inSource = inSource ?? DataSourceFactory.CreateDefault(appIdentity: App, configSource: ConfigurationProvider);
            return DataSourceFactory.Create<T>(source: inSource, configuration: configuration);
        }

        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
        {
            // if it has a source, then use this, otherwise it's null and that works too. Reason: some sources like DataTable or SQL won't have an upstream source
            var src = CreateSource<T>(inStream.Source);
            src.In.Clear();
            src.Attach(DataSourceConstants.StreamDefaultName, inStream);
            return src;
        }

        [PrivateApi]
        public IDataSource CreateSourceWip(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            IDataSource source = default,
            IConfiguration configuration = default)
        {
            // VERY WIP
            var catalog = GetService<DataSourceCatalog>();
            var type = catalog.FindDataSourceInfo(name, App.AppId)?.Type;
            var configurationSourceNew = new ConfigurationWip
            {
                LookUpEngine = configuration?.GetLookupEngineWip() ?? ConfigurationProvider?.GetLookupEngineWip(),
                Values = null // todo configuration
            };
            var ds = DataSourceFactory.Create(type, appIdentity: App, source: source, configuration: configurationSourceNew);

            // if it supports all our known context properties, attach them
            if (ds is INeedsDynamicCodeRoot needsRoot) needsRoot.ConnectToRoot(this);
            return ds;
        }
        #endregion
    }
}
