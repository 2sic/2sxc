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
using static ToSic.Eav.DataSource.DataSourceOptions;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    [PrivateApi("not yet ready / public")]
    public class DataService: ServiceForDynamicCode, IDataService
    {
        public DataService(LazySvc<IDataSourcesService> dataSources, LazySvc<DataSourceCatalog> catalog, LazySvc<IAppStates> appStates) : base("Sxc.DatSvc")
        {
            _dataSources = dataSources;
            _catalog = catalog;
            _appStates = appStates;
        }
        private readonly LazySvc<IDataSourcesService> _dataSources;
        private readonly LazySvc<DataSourceCatalog> _catalog;
        private readonly LazySvc<IAppStates> _appStates;

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            Setup(codeRoot.App, () => (codeRoot as DynamicCodeRoot)?.LookUpForDataSources);
        }

        [PrivateApi]
        internal IDataService Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
        {
            _appIdentity = appIdentity ?? _appIdentity;
            _getLookup = getLookup ?? _getLookup;
            return this;
        }
        private IAppIdentity _appIdentity;
        
        public IDataService New(string noParamOrder = Protector, IAppIdentity appIdentity = default, int zoneId = default, int appId = default)
        {
            // Make sure we have an AppIdentity if possible - or reuse the existing, though it could be null
            if (appIdentity == default)
            {
                if (appId != 0)
                    appIdentity = zoneId == default
                        ? _appStates.Value.IdentityOfApp(appId)
                        : new AppIdentity(zoneId, appId);
                else
                    appIdentity = _appIdentity;
            }

            var newDs = new DataService(_dataSources, _catalog, _appStates);
            if (_DynCodeRoot != null)
            {
                newDs.ConnectToRoot(_DynCodeRoot);
                newDs.Setup(appIdentity, null);
            }
            else
            {
                newDs.Setup(appIdentity, _getLookup);
            }
            return newDs;
        }

        private IDataSourceOptions SafeOptions(object options, bool identityRequired = false)
        {
            // Ensure we have a valid AppIdentity
            var appIdentity = _appIdentity ?? (options as IDataSourceOptions)?.AppIdentity
                ?? (identityRequired
                    ? throw new Exception(
                        "Creating a DataSource requires an AppIdentity which must either be supplied by the context, " +
                        $"(the Module / WebApi call) or provided manually by spawning a new {nameof(IDataService)} with the AppIdentity using {nameof(New)}.")
                    : new AppIdentity(0, 0)
                );
            // Convert to a pure identity, in case the original object was much more
            appIdentity = new AppIdentity(appIdentity);
            return new Converter().Create(new DataSourceOptions(appIdentity: appIdentity, lookUp: LookUpEngine, immutable: true), options);
        }

        private ILookUpEngine LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
        private readonly GetOnce<ILookUpEngine> _lookupEngine = new GetOnce<ILookUpEngine>();
        private Func<ILookUpEngine> _getLookup;


        public IDataSource GetAppSource(string noParamOrder = Protector, object options = null)
        {
            Protect(noParamOrder, $"{nameof(options)}");
            var fullOptions = SafeOptions(options, true);
            var appSource = _dataSources.Value.CreateDefault(fullOptions);
            return appSource;
        }

        // WIP - ATM the code is in _DynCodeRoot, but it should actually be moved here and removed there
        // IMPORTANT - this is different! from the _DynCodeRoot - as it shouldn't auto attach at all!
        public T GetSource<T>(string noParamOrder = Protector,
            IDataSourceLinkable attach = null, object options = null) where T : IDataSource
        {
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(options)}");

            // If no in-source was provided, make sure that we create one from the current app
            //attach = attach ?? GetAppSource();
            var fullOptions = SafeOptions(options);
            return _dataSources.Value.Create<T>(attach: attach, options: fullOptions);

        }

        public IDataSource GetSource(string noParamOrder = Protector,
            string name = null,
            IDataSourceLinkable attach = null,
            object options = null)
        {
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(options)}");
            // Do this first, to ensure AppIdentity is really known/set
            var safeOptions = SafeOptions(options);
            var type = _catalog.Value.FindDataSourceInfo(name, safeOptions.AppIdentity.AppId)?.Type;

            var ds = _dataSources.Value.Create(type, attach: attach as IDataSource, options: safeOptions);
            return ds;
        }
    }
}
