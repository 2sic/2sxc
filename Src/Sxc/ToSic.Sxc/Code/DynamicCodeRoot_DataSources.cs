using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        #region DataSource and ConfigurationProvider (for DS) section
        private ILookUpEngine _configurationProvider;

        internal ILookUpEngine ConfigurationProvider
            => _configurationProvider ??
               (_configurationProvider = Data.Configuration.LookUps);

        internal DataSourceFactory DataSourceFactory => _dataSourceFactory ??
                                                        (_dataSourceFactory = ServiceProvider.Build<DataSourceFactory>().Init(Log));
        private DataSourceFactory _dataSourceFactory;



        /// <inheritdoc />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null) where T : IDataSource
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSourceFactory.GetDataSource<T>(inSource, inSource, configurationProvider);

            var userMayEdit = Block.Context.UserMayEdit;

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
            srcDs.In.Add(Eav.Constants.DefaultStreamName, inStream);
            return src;
        }
        #endregion
    }
}
