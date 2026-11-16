using System.Net;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSource.Sys.Convert;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.WebApi.Sys.Admin.Query;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Services.DataService.Sys;
using ToSic.Sys.OData;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// In charge of delivering Pipeline-Queries on the fly
/// They will only be delivered if the security is confirmed - it must be publicly available
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppQueryControllerReal(
    LazySvc<AppQueryODataHelper> oDataHelper,
    ISxcCurrentContextService ctxService,
    IContextBuilderService ctxBuilder,
    Generator<IConvertToEavLight> dataConverter,
    LazySvc<PermissionCheckFactory> permCheckFactory,
    LazySvc<QueryBuilder<Query>> queryManager,
    LazySvc<ILookUpEngineResolver> lookupResolver)
    : ServiceBase("Sxc.ApiApQ",
        connect: [oDataHelper, lookupResolver, ctxService, ctxBuilder, dataConverter, permCheckFactory, queryManager])
{
    public const string LogSuffix = "AppQry";


    /// <summary>
    /// Run Queries which are in a container context
    /// </summary>
    /// <param name="name">query name</param>
    /// <param name="more">additional url parameters for odata</param>
    /// <param name="appId"></param>
    /// <param name="stream"></param>
    /// <param name="includeGuid"></param>
    /// <returns></returns>
    public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, QueryParametersDtoFromClient? more, int? appId, string? stream = null, bool? includeGuid = false)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
        
        if (string.IsNullOrEmpty(name))
            throw l.Ex(HttpException.MissingParam(nameof(name)));

        var appCtx = appId != null
            ? ctxService.ShareAppStateOptimized(appId.Value)
            : ctxService.BlockContextRequired();

        // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
        var maybeBlock = appId == null || appId == appCtx.AppReaderRequired.AppId
            ? ctxService.BlockOrNull
            : null;

        var parameters = PrepareParameters(appCtx, maybeBlock, name, stream, includeGuid, more);

        var result = BuildQueryAndRun(parameters);
        return l.Return(result);
    }


    /// <summary>
    /// Public Queries
    /// </summary>
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(string appPath, string name, QueryParametersDtoFromClient? more, string? stream) 
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"path:{appPath}, name:{name}, stream: {stream}");
        
        if (string.IsNullOrEmpty(name))
            throw l.Ex(HttpException.MissingParam(nameof(name)));

        // 2026-10-20 2dm - previously used ctxService.SetAppFromPathOrGetExistingBlock
        var appCtx = ctxBuilder.TryFindAppFromPath(appPath)
                     ?? throw new ArgumentException($"App not found for folder '{appPath}'. Can't run query.");

        var parameters = PrepareParameters(appCtx, ctxService.BlockOrNull, name, stream, false, more);

        // now just run the default query check and serializer
        var result = BuildQueryAndRun(parameters);
        return l.Return(result);
    }



    private Parameters PrepareParameters(IContextOfApp context, IBlock? maybeBlock, string queryName, string? streams, bool? includeGuid, QueryParametersDtoFromClient? odata)
    {
        var modId = (context as IContextOfModule)?.Module.Id ?? -1;

        var l = Log.Fn<Parameters>($"name:{queryName}, stream:{streams}, withModule:{modId}");

        // If no app available from context, check if an app-id was supplied in url
        // Note that it may only be an app from the current portal
        // and security checks will run internally
        var blockLookupOrNull = maybeBlock is { DataIsReady: true }
            ? maybeBlock.Data.Configuration.LookUpEngine
            : null;

        streams = streams == DataSourceConstants.AllStreams ? null : streams;
        var lookups = blockLookupOrNull ?? lookupResolver.Value.GetLookUpEngine(modId);

        var query = GetQueryAndDoPermissionChecks(queryName, lookups, context);

        var streamNames = DataSourceConvertHelper.GetBestStreamNames(query, streams);

        return l.Return(new(
            Context: context,
            AppReader: context.AppReaderRequired,
            Lookups: lookups,
            Query: query,
            QueryName: queryName,
            Streams: streams,
            StreamNames: streamNames,
            IncludeGuids: includeGuid ?? false,
            OData: odata?.Guids
        ));
    }

    public record Parameters(
        IContextOfApp Context,
        IAppReader AppReader,
        ILookUpEngine Lookups,
        Query Query,
        string QueryName,
        string? Streams,
        string[] StreamNames,
        bool IncludeGuids,
        string[]? OData
    );


    private Query GetQueryAndDoPermissionChecks(string queryName, ILookUpEngine lookups, IContextOfApp context)
    {
        var app = context.AppReaderRequired;
        var l = Log.Fn<Query?>($"name:{queryName}, app:{app.AppId}");
        var query = queryManager.Value.TryBuild(app, queryName, lookups, recurseParents: 3);

        if (query == null)
        {
            var msg = $"query '{queryName}' not found";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.NotFound, msg, "query not found"));
        }

        l.A($"Check permission on query {query.Definition.Id}");
        var permissionChecker = permCheckFactory.Value.ForItem(context, app, (query.Definition as ICanBeEntity).Entity);
        var readExplicitlyAllowed = permissionChecker.UserMay(GrantSets.ReadSomething).Allowed;

        // Only return query if permissions ok
        if (!(readExplicitlyAllowed || context.User.IsContentAdmin))
        {
            var msg = $"Request not allowed. User does not have read permissions for query '{queryName}'";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, msg, "Request not allowed"));
        }

        return l.Return(query);
    }


    public IDictionary<string, IEnumerable<EavLightEntity>> BuildQueryAndRun(Parameters parameters)
    {
        var (context, _, lookups, query, _, streams, streamNames, includeGuid, guidList) = parameters;

        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>();


        // Pass the originally requested stream so QueryODataParams can map bare OData options
        // to that stream when exactly one stream was explicitly selected.
        var streamOptions = QueryODataParams.CreateMany(d => lookups.LookUp(d), streamNames, streams);

        // v21 support OData filtering, sorting...
        var mustUseOData = streamOptions.Any(so => !so.Value.IsEmptyExceptForSelect());
        if (mustUseOData)
        {
            var oDataResult = oDataHelper.Value.ApplyOData(query, streamOptions, guidList, includeGuid, context.Permissions.IsContentAdmin);
            return l.Return(oDataResult, "processed with OData");
        }

        // New v17 experimental with special fields
        var systemQueryOptions = QueryODataParams.Create(d => lookups.LookUp(d));

        // Classic, lightweight conversion
        var selectFields = streamOptions.ToDictionary(
            pair => pair.Key,
            ICollection<string> (pair) => pair.Value.Select.ToListOpt(),
            StringComparer.OrdinalIgnoreCase
        );
        var dc = dataConverter.PrepareDataConverter(systemQueryOptions, includeGuid, context.Permissions.IsContentAdmin);
        var result = dc.Convert(query, streamNames, guidList, selectFields);
        return l.Return(result, "classic convert");
    }
}
