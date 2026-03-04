using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSource.Sys.Catalog;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Services.Sys.DataService;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;

// TODO: MAKE PRIVATE AGAIN AFTER MOVING TO ToSic.Sxc.Custom

namespace ToSic.Sxc.Services.Data.Sys;

[PrivateApi("hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class DataService(
    LazySvc<IDataSourcesService> dataSources,
    LazySvc<DataSourceCatalog> catalog,
    LazySvc<IAppsCatalog> appsCatalog,
    LazySvc<QueryManager<Query>> queryManager,
    IUser user)
    : ServiceWithContext("Sxc.DatSvc", connect: [user, dataSources, catalog, appsCatalog, queryManager]),
        IDataService,
        IServiceWithSetup<DataService.Options>
{
    public record Options(IAppIdentity? AppIdentity, Func<ILookUpEngine?>? GetLookup);

    public override void ConnectToRoot(IExecutionContext exCtx)
    {
        base.ConnectToRoot(exCtx);
        Setup(new(exCtx.GetState<Sxc.Apps.IApp>(), () => (exCtx as IExCtxLookUpEngine)?.LookUpForDataSources));
    }

    public void Setup(Options opts)
    {
        _appIdentity = opts.AppIdentity ?? _appIdentity;
        _getLookup = opts.GetLookup ?? _getLookup;

    }
    private IAppIdentity? _appIdentity;

    //// TODO: MAKE PRIVATE AGAIN AFTER MOVING TO ToSic.Sxc.Custom
    //public IDataService SetupOld(IAppIdentity? appIdentity, Func<ILookUpEngine?>? getLookup)
    //{
    //    _appIdentity = appIdentity ?? _appIdentity;
    //    _getLookup = getLookup ?? _getLookup;
    //    return this;
    //}


    public IDataService SpawnNew(NoParamOrder npo = default, IAppIdentity? appIdentity = default, int zoneId = default, int appId = default)
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
            newDs.Setup(new(appIdentity, null));
        }
        else
        {
            newDs.Setup(new(appIdentity, _getLookup));
        }
        return newDs;
    }

    private DataSourceOptionsMs OptionsMs => _optionsHandler.Get(() => new(_appIdentity, _getLookup))!;
    private readonly GetOnce<DataSourceOptionsMs> _optionsHandler = new();

    private Func<ILookUpEngine?>? _getLookup;


    public IDataSource GetAppSource(NoParamOrder npo = default, object? parameters = default, object? options = default)
    {
        var l = Log.Fn<IDataSource>($"{nameof(options)}: {options}");
        var fullOptions = OptionsMs.SafeOptions(parameters, options: options, identityRequired: true);
        var appSource = dataSources.Value.CreateDefault(fullOptions);
        return l.Return(appSource);
    }


    #region GetQuery

    public IDataSource? GetQuery(string? name = default,
        NoParamOrder npo = default,
        IDataSourceLinkable? attach = default,
        object? parameters = default)
        => new GetQueryMs<Query>(queryManager, OptionsMs, Log).GetQuery(name, npo, attach, parameters);

    #endregion

    #region Linking

    public IDataSourceLink CreateLink(IDataSourceLinkable source,
        NoParamOrder npo = default,
        string? inName = default,
        string? outName = default
    )
    {
        var link = source.GetLink();
        return inName != default || outName != default
            ? link.WithRename(outName: outName, inName: inName)
            : link;
    }

    public IDataSourceLink CombineLinks(params IDataSourceLinkable[] sources)
    {
        if (sources.Length == 0)
            throw new ArgumentException(@"At least one source must be provided", nameof(sources));

        var first = sources[0].GetLink();
        return sources.Length == 1
            ? first
            : first.WithMore(sources.Skip(1).ToArray());
    }

    #endregion

}