using System;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Eav.DataSources.Linking;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using static ToSic.Eav.Parameters;

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
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource 
            => CreateDataSource<T>(attach: inSource, options: configurationProvider);

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
        public T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
        {
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(options)}");

            // If no in-source was provided, make sure that we create one from the current app
            attach = attach ?? DataSourceFactory.CreateDefault(new DataSourceOptions(appIdentity: App, lookUp: ConfigurationProvider));
            var typedOptions = new DataSourceOptions.Converter().Create(new DataSourceOptions(lookUp: ConfigurationProvider), options);
            return DataSourceFactory.Create<T>(attach: attach, options: typedOptions);
        }

        [PrivateApi]
        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default)
        {
            Protect(noParamOrder, $"{nameof(name)}, {nameof(attach)}, {nameof(options)}");
            var catalog = GetService<DataSourceCatalog>();
            var type = catalog.FindDataSourceInfo(name, App.AppId)?.Type;

            var finalConf2 =
                new DataSourceOptions.Converter().Create(
                    new DataSourceOptions(lookUp: ConfigurationProvider, appIdentity: App), options);
            var ds = DataSourceFactory.Create(type, attach: attach as IDataSource, options: finalConf2);

            // if it supports all our known context properties, attach them
            if (ds is INeedsDynamicCodeRoot needsRoot) needsRoot.ConnectToRoot(this);
            return ds;
        }

        #endregion
    }
}
