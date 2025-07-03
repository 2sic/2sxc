using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.DataSource.Sys.Catalog;
using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;

// TODO: MAKE PRIVATE AGAIN AFTER MOVING TO ToSic.Sxc.Custom

namespace ToSic.Sxc.Services.DataServices;

[PrivateApi("hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class DataService(
    LazySvc<IDataSourcesService> dataSources,
    LazySvc<DataSourceCatalog> catalog,
    LazySvc<IAppsCatalog> appsCatalog,
    LazySvc<QueryManager> queryManager,
    IUser user)
    : ServiceWithContext("Sxc.DatSvc", connect: [user, dataSources, catalog, appsCatalog, queryManager]), IDataService
{
    public override void ConnectToRoot(IExecutionContext exCtx)
    {
        base.ConnectToRoot(exCtx);
        Setup(exCtx.GetState<Sxc.Apps.IApp>(), () => (exCtx as IExCtxLookUpEngine)?.LookUpForDataSources);
    }

    // TODO: MAKE PRIVATE AGAIN AFTER MOVING TO ToSic.Sxc.Custom
    public IDataService Setup(IAppIdentity? appIdentity, Func<ILookUpEngine?>? getLookup)
    {
        _appIdentity = appIdentity ?? _appIdentity;
        _getLookup = getLookup ?? _getLookup;
        return this;
    }
    private IAppIdentity? _appIdentity;
        
    public IDataService SpawnNew(NoParamOrder noParamOrder = default, IAppIdentity? appIdentity = default, int zoneId = default, int appId = default)
    {
        // Make sure we have an AppIdentity if possible - or reuse the existing, though it could be null
        if (appIdentity == default)
        {
            if (appId != default)
                appIdentity = zoneId == default
                    ? appsCatalog.Value.AppIdentity(appId)
                    : new AppIdentity(zoneId, appId);
            else
                appIdentity = _appIdentity;
        }

        var newDs = new DataService(dataSources, catalog, appsCatalog, queryManager, user);
        if (ExCtxOrNull != null)
        {
            newDs.ConnectToRoot(ExCtxOrNull);
            newDs.Setup(appIdentity, null);
        }
        else
        {
            newDs.Setup(appIdentity, _getLookup);
        }
        return newDs;
    }

    private DataSourceOptionsMs OptionsMs => _optionsHandler.Get(() => new(_appIdentity, _getLookup))!;
    private readonly GetOnce<DataSourceOptionsMs> _optionsHandler = new();

    private Func<ILookUpEngine?>? _getLookup;


    public IDataSource GetAppSource(NoParamOrder noParamOrder = default, object? parameters = default, object? options = default)
    {
        var l = Log.Fn<IDataSource>($"{nameof(options)}: {options}");
        var fullOptions = OptionsMs.SafeOptions(parameters, options: options, identityRequired: true);
        var appSource = dataSources.Value.CreateDefault(fullOptions);
        return l.Return(appSource);
    }


    #region GetQuery

    public IDataSource? GetQuery(string? name = default,
        NoParamOrder noParamOrder = default,
        IDataSourceLinkable? attach = default,
        object? parameters = default)
        => new GetQueryMs(queryManager, OptionsMs, Log).GetQuery(name, noParamOrder, attach, parameters);

    #endregion

}