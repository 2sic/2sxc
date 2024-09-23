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
public class AppQueryControllerReal(
    ISxcContextResolver ctxResolver,
    IConvertToEavLight dataConverter,
    Generator<AppPermissionCheck> appPermissionCheck,
    LazySvc<QueryManager> queryManager,
    LazySvc<ILookUpEngineResolver> lookupResolver)
    : ServiceBase("Sxc.ApiApQ",
        connect: [lookupResolver, ctxResolver, dataConverter, appPermissionCheck, queryManager]), IAppQueryController
{
    public const string LogSuffix = "AppQry";

    private const string AllStreams = "*";

    #region In-Container-Context Queries

    public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, int? appId, string stream = null,
        bool includeGuid = false)
        => QueryPost(name, null, appId, stream, includeGuid);

    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(string name, QueryParameters more, int? appId, string stream = null, bool includeGuid = false)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
        var appCtx = appId != null ? ctxResolver.GetBlockOrSetApp(appId.Value) : ctxResolver.BlockContextRequired();

        // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
        var maybeBlock = appId == null || appId == appCtx.AppReader.AppId ? ctxResolver.BlockOrNull() : null;

        // If no app available from context, check if an app-id was supplied in url
        // Note that it may only be an app from the current portal
        // and security checks will run internally
        //var app = _app.New().InitWithOptionalBlock(appCtx.AppStateReader.AppId, maybeBlock);

        var blockLookupOrNull = maybeBlock?.Data?.Configuration?.LookUpEngine;

        var result = BuildQueryAndRun(appCtx.AppReader, name, stream, includeGuid, appCtx, more, blockLookupOrNull);
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

        var appCtx = ctxResolver.SetAppOrGetBlock(appPath);

        var blockLookupOrNull = ctxResolver.BlockOrNull()?.Data?.Configuration?.LookUpEngine;
        //var queryApp = _app.New().Init(appCtx.AppState, _appConfigDelegate.New().Build());

        // now just run the default query check and serializer
        var result = BuildQueryAndRun(appCtx.AppReader, name, stream, false, appCtx, more, blockLookupOrNull);
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

        var lookups = preparedLookup ?? lookupResolver.Value.GetLookUpEngine(modId);
        var query = queryManager.Value.GetQuery(app, name, lookups, recurseParents: 3);

        if (query == null)
        {
            var msg = $"query '{name}' not found";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found"));
        }

        var permissionChecker = appPermissionCheck.New()
            .ForItem(context, app, query.Definition.Entity);
        var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething);

        var isAdmin = context.User.IsContentAdmin;

        // Only return query if permissions ok
        if (!(readExplicitlyAllowed || isAdmin))
        {
            var msg = $"Request not allowed. User does not have read permissions for query '{name}'";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed"));
        }

        dataConverter.WithGuid = includeGuid;
        if (dataConverter is ConvertToEavLightWithCmsInfo serializerWithEdit)
            serializerWithEdit.WithEdit = context.Permissions.IsContentAdmin;
        if (stream == AllStreams) stream = null;


        // New v17 experimental with special fields
        var extraParams = new QueryODataParams(query.Configuration);
        if (dataConverter is ConvertToEavLight serializerWithOData)
            serializerWithOData.AddSelectFields(extraParams.SelectFields);

        var result = dataConverter.Convert(query, stream?.Split(','), more?.Guids);
        return l.Return(result);
    }
}