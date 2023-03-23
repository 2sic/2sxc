using System;
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
        public ILookUpEngine ConfigurationProvider // todo: rename to LookUpForDataSources
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
        public T CreateSource<T>(IDataSource source = null, object options = null) where T : IDataSource
        {
            // If no in-source was provided, make sure that we create one from the current app
            source = source ?? DataSourceFactory.CreateDefault(new DataSourceOptions(appIdentity: App, lookUp: ConfigurationProvider));
            var typedOptions = new DataSourceOptions.Converter().Create(new DataSourceOptions(lookUp: ConfigurationProvider), options);
            return DataSourceFactory.Create<T>(source: source, options: typedOptions);
        }

        /// <inheritdoc />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
        {
            // if it has a source, then use this, otherwise it's null and then it uses the App-Default
            // Reason: some sources like DataTable or SQL won't have an upstream source
            var src = CreateSource<T>(source.Source);
            src.In.Clear();
            src.Attach(DataSourceConstants.StreamDefaultName, source);
            return src;
        }

        [PrivateApi]
        public IDataSource CreateSourceWip(string name,
            string noParamOrder = ToSic.Eav.Parameters.Protector,
            IDataSource source = null,
            object options = null)
        {
            // VERY WIP
            var catalog = GetService<DataSourceCatalog>();
            var type = catalog.FindDataSourceInfo(name, App.AppId)?.Type;

            var finalConf2 =
                new DataSourceOptions.Converter().Create(
                    new DataSourceOptions(lookUp: ConfigurationProvider, appIdentity: App), options);
            var ds = DataSourceFactory.Create(type, source: source, options: finalConf2);

            // if it supports all our known context properties, attach them
            if (ds is INeedsDynamicCodeRoot needsRoot) needsRoot.ConnectToRoot(this);
            return ds;
        }
        #endregion
    }
}
