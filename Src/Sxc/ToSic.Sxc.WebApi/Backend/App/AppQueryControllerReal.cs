using System.Net;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.LookUp;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Admin.App;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Data.Internal.Convert;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// In charge of delivering Pipeline-Queries on the fly
/// They will only be delivered if the security is confirmed - it must be publicly available
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppQueryControllerReal: ServiceBase , IAppQueryController
{
    private readonly LazySvc<ILookUpEngineResolver> _lookupResolver;
    private readonly LazySvc<QueryManager> _queryManager;
    public const string LogSuffix = "AppQry";

    private const string AllStreams = "*";

    #region Constructor / DI

    public AppQueryControllerReal(ISxcContextResolver ctxResolver, 
        IConvertToEavLight dataConverter, 
        Generator<AppPermissionCheck> appPermissionCheck,
        LazySvc<QueryManager> queryManager,
        LazySvc<ILookUpEngineResolver> lookupResolver) : base("Sxc.ApiApQ")
    {
        _lookupResolver = lookupResolver;
        ConnectServices(
            _ctxResolver = ctxResolver,
            _dataConverter = dataConverter,
            _appPermissionCheck = appPermissionCheck,
            _queryManager = queryManager
        );
    }
        
    private readonly ISxcContextResolver _ctxResolver;
    private readonly IConvertToEavLight _dataConverter;
    private readonly Generator<AppPermissionCheck> _appPermissionCheck;

    #endregion

    #region In-Container-Context Queries

    public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, int? appId, string stream = null,
        bool includeGuid = false)
        => QueryPost(name, null, appId, stream, includeGuid);

    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(string name, QueryParameters more, int? appId, string stream = null, bool includeGuid = false)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
        var appCtx = appId != null ? _ctxResolver.GetBlockOrSetApp(appId.Value) : _ctxResolver.BlockContextRequired();

        // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
        var maybeBlock = appId == null || appId == appCtx.AppState.AppId ? _ctxResolver.BlockOrNull() : null;

        // If no app available from context, check if an app-id was supplied in url
        // Note that it may only be an app from the current portal
        // and security checks will run internally
        //var app = _app.New().InitWithOptionalBlock(appCtx.AppStateReader.AppId, maybeBlock);

        var blockLookupOrNull = maybeBlock?.Data?.Configuration?.LookUpEngine;

        var result = BuildQueryAndRun(appCtx.AppState, name, stream, includeGuid, appCtx, more, blockLookupOrNull);
        return l.Return(result);
    }

    #endregion

    #region Public Queries

    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(string appPath, string name, string stream)
        => PublicQueryPost(appPath, name, null, stream);


    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(string appPath, string name, QueryParameters more, string stream) 
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"path:{appPath}, name:{name}, stream: {stream}");
        if (string.IsNullOrEmpty(name))
            throw l.Ex(HttpException.MissingParam(nameof(name)));

        var appCtx = _ctxResolver.SetAppOrGetBlock(appPath);

        var blockLookupOrNull = _ctxResolver.BlockOrNull()?.Data?.Configuration?.LookUpEngine;
        //var queryApp = _app.New().Init(appCtx.AppState, _appConfigDelegate.New().Build());

        // now just run the default query check and serializer
        var result = BuildQueryAndRun(appCtx.AppState, name, stream, false, appCtx, more, blockLookupOrNull);
        return l.Return(result);
    }


    #endregion


    private IDictionary<string, IEnumerable<EavLightEntity>> BuildQueryAndRun(
        IAppIdentity app,
        string name,
        string stream,
        bool includeGuid,
        IContextOfApp context,
        QueryParameters more,
        ILookUpEngine preparedLookup = null)
    {
        var modId = (context as IContextOfBlock)?.Module.Id ?? -1;
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"name:{name}, stream:{stream}, withModule:{(context as IContextOfBlock)?.Module.Id}");
        var lookups = preparedLookup ?? _lookupResolver.Value.GetLookUpEngine(modId);
        var query = _queryManager.Value.GetQuery(app, name, lookups, recurseParents: 3);
        // var query = app.GetQuery(name);

        if (query == null)
        {
            var msg = $"query '{name}' not found";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found"));
        }

        var permissionChecker = _appPermissionCheck.New()
            .ForItem(context, app, query.Definition.Entity);
        var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

        var isAdmin = context.User.IsContentAdmin;

        // Only return query if permissions ok
        if (!(readExplicitlyAllowed || isAdmin))
        {
            var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed"));
        }

        _dataConverter.WithGuid = includeGuid;
        if (_dataConverter is ConvertToEavLightWithCmsInfo serializerWithEdit)
            serializerWithEdit.WithEdit = context.UserMayEdit;
        if (stream == AllStreams) stream = null;


        // New v17 experimental with special fields
        var extraParams = new QueryODataParams(query.Configuration);
        if (_dataConverter is ConvertToEavLight serializerWithOData)
            serializerWithOData.SelectFields = extraParams.SelectFields;

        var result = _dataConverter.Convert(query, stream?.Split(','), more?.Guids);
        return l.Return(result);
    }
}