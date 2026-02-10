using System.Net;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSources;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Eav.WebApi.Sys.Admin.App;
using ToSic.Eav.WebApi.Sys.Admin.Query;
using ToSic.Sxc.Data.Sys.Convert;
using ToSic.Sys.OData;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// In charge of delivering Pipeline-Queries on the fly
/// They will only be delivered if the security is confirmed - it must be publicly available
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppQueryControllerReal(
    ISxcCurrentContextService ctxService,
    IConvertToEavLight dataConverter,
    Generator<AppPermissionCheck> appPermissionCheck,
    LazySvc<QueryManager> queryManager,
    LazySvc<ILookUpEngineResolver> lookupResolver,
    IDataSourcesService dataSourcesService)
    : ServiceBase("Sxc.ApiApQ",
        connect: [lookupResolver, ctxService, dataConverter, appPermissionCheck, queryManager]), IAppQueryController
{
    public const string LogSuffix = "AppQry";

    private const string AllStreams = "*";

    #region In-Container-Context Queries

    public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, int? appId, string? stream = null, bool includeGuid = false)
        => QueryPost(name, null, appId, stream, includeGuid);

    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(string name, QueryParametersDtoFromClient? more, int? appId, string? stream = null, bool includeGuid = false)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
        var appCtx = appId != null
            ? ctxService.GetExistingAppOrSet(appId.Value)
            : ctxService.BlockContextRequired();

        // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
        var maybeBlock = appId == null || appId == appCtx.AppReaderRequired.AppId
            ? ctxService.BlockOrNull()
            : null;

        // If no app available from context, check if an app-id was supplied in url
        // Note that it may only be an app from the current portal
        // and security checks will run internally
        var blockLookupOrNull = maybeBlock is { DataIsReady: true }
            ? maybeBlock.Data.Configuration.LookUpEngine
            : null;

        var result = BuildQueryAndRun(appCtx.AppReaderRequired, name, stream, includeGuid, appCtx, more, blockLookupOrNull);
        return l.Return(result);
    }

    #endregion

    #region Public Queries

    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(string appPath, string name, string? stream)
        => PublicQueryPost(appPath, name, null, stream);


    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(string appPath, string name, QueryParametersDtoFromClient? more, string? stream) 
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"path:{appPath}, name:{name}, stream: {stream}");
        if (string.IsNullOrEmpty(name))
            throw l.Ex(HttpException.MissingParam(nameof(name)));

        var appCtx = ctxService.SetAppOrGetBlock(appPath);

        var blockLookupOrNull = ctxService.BlockOrNull()?.Data?.Configuration?.LookUpEngine;

        // now just run the default query check and serializer
        var result = BuildQueryAndRun(appCtx.AppReaderRequired, name, stream, false, appCtx, more, blockLookupOrNull);
        return l.Return(result);
    }


    #endregion


    private IDictionary<string, IEnumerable<EavLightEntity>> BuildQueryAndRun(
        IAppIdentity app,
        string name,
        string? stream,
        bool includeGuid,
        IContextOfApp context,
        QueryParametersDtoFromClient? more,
        ILookUpEngine? preparedLookup = null)
    {
        var modId = (context as IContextOfBlock)?.Module.Id ?? -1;

        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"name:{name}, stream:{stream}, withModule:{(context as IContextOfBlock)?.Module.Id}");

        var lookups = preparedLookup ?? lookupResolver.Value.GetLookUpEngine(modId);
        var query = queryManager.Value.TryGetQuery(app, name, lookups, recurseParents: 3);

        if (query == null)
        {
            var msg = $"query '{name}' not found";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found"));
        }

        l.A($"Check permission on query {query.Definition.Id}");
        var permissionChecker = appPermissionCheck.New()
            .ForItem(context, app, (query.Definition as ICanBeEntity).Entity);
        var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething).Allowed;

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

        if (stream == AllStreams)
            stream = null;

        // New v17 experimental with special fields
        var systemQueryOptions = new QueryODataParams(query.Configuration).SystemQueryOptions;
        if (dataConverter is ConvertToEavLight serializerWithOData)
            serializerWithOData.AddSelectFields(systemQueryOptions.Select.ToListOpt());

        // v20 support OData filtering, sorting...
        var result = systemQueryOptions.RawAllSystem.Any()
            ? ApplyOData(query, systemQueryOptions, stream, more?.Guids)
            : dataConverter.Convert(query, stream?.Split(','), more?.Guids);
        return l.Return(result);
    }

    private IDictionary<string, IEnumerable<EavLightEntity>> ApplyOData(IDataSource query, SystemQueryOptions systemQueryOptions, string? stream, string[]? filterGuids)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>();
        var oDataQuery = UriQueryParser.Parse(systemQueryOptions);
        var engine = new ODataQueryEngine(dataSourcesService);

        var streams = stream?.Split(',')
                          .Select(s => s.Trim())
                          .Where(s => !string.IsNullOrWhiteSpace(s))
                          .ToArray()
                      ?? query.Out.Select(p => p.Key).ToArray();

        var guidFilter = filterGuids?
            .Select(g => Guid.TryParse(g, out var guid) ? guid : (Guid?)null)
            .Where(g => g.HasValue)
            .Select(g => g!.Value)
            .ToHashSet()
            ?? new HashSet<Guid>();

        var results = new Dictionary<string, IEnumerable<EavLightEntity>>(StringComparer.OrdinalIgnoreCase);

        foreach (var streamName in streams)
        {
            var sourceStream = query.GetStream(streamName, nullIfNotFound: true);
            if (sourceStream == null)
            {
                l.A($"Stream '{streamName}' not found, skip OData.");
                continue;
            }

            var wrapper = dataSourcesService.Create<PassThrough>(sourceStream);
            var execution = engine.Execute(wrapper, oDataQuery);
            var entities = guidFilter.Any()
                ? execution.Items.Where(e => guidFilter.Contains(e.EntityGuid))
                : execution.Items;

            results[streamName] = dataConverter.Convert(entities);
        }

        return l.Return(results);
    }
}
