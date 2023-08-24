using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Catalog;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services.DataServices;
using static ToSic.Eav.DataSource.DataSourceOptions;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    [PrivateApi("hide implementation")]
    internal partial class DataService: ServiceForDynamicCode, IDataService
    {
        private readonly LazySvc<IDataSourcesService> _dataSources;
        private readonly LazySvc<DataSourceCatalog> _catalog;
        private readonly LazySvc<IAppStates> _appStates;
        private readonly LazySvc<QueryManager> _queryManager;
        private readonly IUser _user;

        public DataService(
            LazySvc<IDataSourcesService> dataSources,
            LazySvc<DataSourceCatalog> catalog,
            LazySvc<IAppStates> appStates,
            LazySvc<QueryManager> queryManager,
            IUser user) : base("Sxc.DatSvc")
        {
            _user = user;
            ConnectServices(
                _dataSources = dataSources,
                _catalog = catalog,
                _appStates = appStates,
                _queryManager = queryManager
            );
        }

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
                if (appId != default)
                    appIdentity = zoneId == default
                        ? _appStates.Value.IdentityOfApp(appId)
                        : new AppIdentity(zoneId, appId);
                else
                    appIdentity = _appIdentity;
            }

            var newDs = new DataService(_dataSources, _catalog, _appStates, _queryManager, _user);
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

        private DataSourceOptionsMs OptionsMs => _optionsHandler.Get(() => new DataSourceOptionsMs(_appIdentity, _getLookup));
        private readonly GetOnce<DataSourceOptionsMs> _optionsHandler = new GetOnce<DataSourceOptionsMs>();

        //private IDataSourceOptions SafeOptions(object parameters, object options, bool identityRequired = false)
        //{
        //    var l = Log.Fn<IDataSourceOptions>($"{nameof(options)}: {options}, {nameof(identityRequired)}: {identityRequired}");
        //    // Ensure we have a valid AppIdentity
        //    var appIdentity = _appIdentity ?? (options as IDataSourceOptions)?.AppIdentity
        //        ?? (identityRequired
        //            ? throw new Exception(
        //                "Creating a DataSource requires an AppIdentity which must either be supplied by the context, " +
        //                $"(the Module / WebApi call) or provided manually by spawning a new {nameof(IDataService)} with the AppIdentity using {nameof(New)}.")
        //            : new AppIdentity(0, 0)
        //        );
        //    // Convert to a pure identity, in case the original object was much more
        //    appIdentity = new AppIdentity(appIdentity);
        //    var opts = new Converter().Create(new DataSourceOptions(appIdentity: appIdentity, lookUp: LookUpEngine, immutable: true), options);

        //    // Check if parameters were supplied, if yes, they override any values in the existing options (16.01)
        //    var values = new Converter().Values(parameters, false, true);
        //    if (values != null) opts = new DataSourceOptions(opts, values: values);

        //    return l.Return(opts);
        //}

        //private ILookUpEngine LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
        //private readonly GetOnce<ILookUpEngine> _lookupEngine = new GetOnce<ILookUpEngine>();
        private Func<ILookUpEngine> _getLookup;


        public IDataSource GetAppSource(string noParamOrder = Protector, object parameters = default, object options = default)
        {
            var l = Log.Fn<IDataSource>($"{nameof(options)}: {options}");
            Protect(noParamOrder, $"{nameof(parameters)}, {nameof(options)}");
            var fullOptions = OptionsMs.SafeOptions(parameters, options: options, identityRequired: true);
            var appSource = _dataSources.Value.CreateDefault(fullOptions);
            return l.Return(appSource);
        }

    }
}
