using System;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Catalog;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    [PrivateApi("not yet ready / public")]
    public class DataService: ServiceForDynamicCode, IDataService
    {
        public DataService(LazySvc<IDataSourcesService> dataSources, LazySvc<DataSourceCatalog> catalog) : base("Sxc.DatSvc")
        {
            DataSources = dataSources;
            Catalog = catalog;
        }
        public readonly LazySvc<IDataSourcesService> DataSources;
        public readonly LazySvc<DataSourceCatalog> Catalog;

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            Setup(codeRoot.App, () => (codeRoot as DynamicCodeRoot).LookUpForDataSources);
        }

        [PrivateApi]
        public IDataService Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
        {
            AppIdentity = appIdentity;
            _getLookup = getLookup;
            return this;
        }
        public IAppIdentity AppIdentity { get; private set; }
        public ILookUpEngine LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
        private readonly GetOnce<ILookUpEngine> _lookupEngine = new GetOnce<ILookUpEngine>();
        private Func<ILookUpEngine> _getLookup;


        // WIP - ATM the code is in _DynCodeRoot, but it should actually be moved here and removed there
        public T GetSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
        {
            //return _DynCodeRoot.DataSources.CreateDataSource<T>(true, noParamOrder: noParamOrder, attach: attach, options: options);
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(options)}");

            // If no in-source was provided, make sure that we create one from the current app
            attach = attach ?? DataSources.Value.CreateDefault(new DataSourceOptions(appIdentity: AppIdentity, lookUp: LookUpEngine, immutable: true));
            var typedOptions = new DataSourceOptions.Converter().Create(new DataSourceOptions(lookUp: LookUpEngine, immutable: true), options);
            return DataSources.Value.Create<T>(attach: attach, options: typedOptions);

        }

        public IDataSource GetSource(string noParamOrder = Protector, string name = default,
            IDataSourceLinkable attach = default, object options = default)
        {
            //return _DynCodeRoot.DataSources.CreateDataSource(noParamOrder: noParamOrder, name: name, attach: attach, options: options);

            Protect(noParamOrder, $"{nameof(name)}, {nameof(attach)}, {nameof(options)}");
            var type = Catalog.Value.FindDataSourceInfo(name, AppIdentity.AppId)?.Type;

            var finalConf2 =
                new DataSourceOptions.Converter().Create(
                    new DataSourceOptions(lookUp: LookUpEngine, appIdentity: AppIdentity, immutable: true), options);
            var ds = DataSources.Value.Create(type, attach: attach as IDataSource, options: finalConf2);

            return ds;

        }
    }
}
