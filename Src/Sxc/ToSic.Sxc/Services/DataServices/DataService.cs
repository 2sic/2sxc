using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Catalog;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.LookUp;
using ToSic.Eav.Services;
using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services.Internal;


namespace ToSic.Sxc.Services.DataServices;

[PrivateApi("hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

    public override void ConnectToRoot(ICodeApiService codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        Setup(codeRoot.App, () => (codeRoot as CodeApiService)?.LookUpForDataSources);
    }

    [PrivateApi]
    internal IDataService Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
    {
        _appIdentity = appIdentity ?? _appIdentity;
        _getLookup = getLookup ?? _getLookup;
        return this;
    }
    private IAppIdentity _appIdentity;
        
    public IDataService New(NoParamOrder noParamOrder = default, IAppIdentity appIdentity = default, int zoneId = default, int appId = default)
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
        if (_CodeApiSvc != null)
        {
            newDs.ConnectToRoot(_CodeApiSvc);
            newDs.Setup(appIdentity, null);
        }
        else
        {
            newDs.Setup(appIdentity, _getLookup);
        }
        return newDs;
    }

    private DataSourceOptionsMs OptionsMs => _optionsHandler.Get(() => new DataSourceOptionsMs(_appIdentity, _getLookup));
    private readonly GetOnce<DataSourceOptionsMs> _optionsHandler = new();

    private Func<ILookUpEngine> _getLookup;


    public IDataSource GetAppSource(NoParamOrder noParamOrder = default, object parameters = default, object options = default)
    {
        var l = Log.Fn<IDataSource>($"{nameof(options)}: {options}");
        //Protect(noParamOrder, $"{nameof(parameters)}, {nameof(options)}");
        var fullOptions = OptionsMs.SafeOptions(parameters, options: options, identityRequired: true);
        var appSource = _dataSources.Value.CreateDefault(fullOptions);
        return l.Return(appSource);
    }


    #region GetQuery

    public IDataSource GetQuery(string name = default,
        NoParamOrder noParamOrder = default,
        IDataSourceLinkable attach = default,
        object parameters = default)
    {
        return new GetQueryMs(_queryManager, OptionsMs, Log).GetQuery(name, noParamOrder, attach, parameters);
    }

    #endregion

}