using System;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Catalog;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code.Helpers
{
    [PrivateApi]
    public class DynamicCodeDataSources
    {
        public readonly LazySvc<IDataSourcesService> DataSources;
        public readonly LazySvc<DataSourceCatalog> Catalog;

        public DynamicCodeDataSources(LazySvc<IDataSourcesService> dataSources, LazySvc<DataSourceCatalog> catalog)
        {
            DataSources = dataSources;
            Catalog = catalog;
        }

        public DynamicCodeDataSources Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
        {
            AppIdentity = appIdentity;
            _getLookup = getLookup;
            return this;
        }

        public IAppIdentity AppIdentity { get; private set; }

        public ILookUpEngine LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
        private readonly GetOnce<ILookUpEngine> _lookupEngine = new GetOnce<ILookUpEngine>();
        private Func<ILookUpEngine> _getLookup;

        public T CreateDataSource<T>(bool immutable, string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
        {
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(options)}");

            // If no in-source was provided, make sure that we create one from the current app
            attach = attach ?? DataSources.Value.CreateDefault(new DataSourceOptions(appIdentity: AppIdentity, lookUp: LookUpEngine, immutable: true));
            var typedOptions = new DataSourceOptions.Converter().Create(new DataSourceOptions(lookUp: LookUpEngine, immutable: immutable), options);
            return DataSources.Value.Create<T>(attach: attach, options: typedOptions);
        }

        [PrivateApi]
        public IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default)
        {
            Protect(noParamOrder, $"{nameof(name)}, {nameof(attach)}, {nameof(options)}");
            var type = Catalog.Value.FindDataSourceInfo(name, AppIdentity.AppId)?.Type;

            var finalConf2 =
                new DataSourceOptions.Converter().Create(
                    new DataSourceOptions(lookUp: LookUpEngine, appIdentity: AppIdentity, immutable: true), options);
            var ds = DataSources.Value.Create(type, attach: attach as IDataSource, options: finalConf2);

            // if it supports all our known context properties, attach them
            //if (ds is INeedsDynamicCodeRoot needsRoot) needsRoot.ConnectToRoot(this);
            return ds;
        }

    }
}
