using System;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Context;

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
        public DataSourceFactory DataSourceFactory => _dataSourceFactory ??
                                                        (_dataSourceFactory = GetService<DataSourceFactory>().Init(Log));
        private DataSourceFactory _dataSourceFactory;



        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSourceFactory.GetDataSource<T>(inSource, inSource, configurationProvider);

            var userMayEdit = (CmsContext as CmsContext)?.CtxSite?.UserMayEdit ?? false;

            var initialSource = DataSourceFactory.GetPublishing(
                App, userMayEdit, ConfigurationProvider as LookUpEngine);
            return DataSourceFactory.GetDataSource<T>(initialSource, initialSource, configurationProvider);
        }

        /// <inheritdoc />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
        {
            // if it has a source, then use this, otherwise it's null and that works too. Reason: some sources like DataTable or SQL won't have an upstream source
            var src = CreateSource<T>(inStream.Source);

            var srcDs = (IDataTarget)src;
            srcDs.In.Clear();
            srcDs.Attach(Eav.Constants.DefaultStreamName, inStream);
            return src;
        }
        #endregion
    }
}
