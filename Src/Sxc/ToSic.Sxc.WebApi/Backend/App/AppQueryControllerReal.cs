using System.Net;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSource.Sys.Convert;
using ToSic.Eav.DataSources.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Services;
using ToSic.Eav.WebApi.Sys.Admin.App;
using ToSic.Eav.WebApi.Sys.Admin.Query;
using ToSic.Sxc.Data.Sys.Convert;
using ToSic.Sys.OData;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// In charge of delivering Pipeline-Queries on the fly
/// They will only be delivered if the security is confirmed - it must be publicly available
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppQueryControllerReal(
    LazySvc<AppQueryODataHelper> oDataHelper,
    ISxcCurrentContextService ctxService,
    LazySvc<IDataSourcesService> dataSourcesService,
    Generator<IConvertToEavLight> dataConverter,
    Generator<AppPermissionCheck> appPermissionCheck,
    LazySvc<QueryManager> queryManager,
    LazySvc<ILookUpEngineResolver> lookupResolver)
    : ServiceBase("Sxc.ApiApQ",
        connect: [oDataHelper, lookupResolver, ctxService, dataSourcesService, dataConverter, appPermissionCheck, queryManager]), IAppQueryController
{
    public const string LogSuffix = "AppQry";
    private const string SystemDataQuery = "System.SysData";

    //private const string AllStreams = "*";

    #region In-Container-Context Queries

    public IDictionary<string, IEnumerable<EavLightEntity>> Query(string name, int? appId, string? stream = null, bool? includeGuid = false)
        => QueryPost(name, null, appId, stream, includeGuid);

    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(string name, QueryParametersDtoFromClient? more, int? appId, string? stream = null, bool? includeGuid = false)
    {
        if (appId == KnownAppsConstants.AppIdEmpty && string.Equals(name, SystemDataQuery, StringComparison.OrdinalIgnoreCase))
            return QuerySystemDataForCurrentSite(more, stream, includeGuid ?? false);

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

        var result = BuildQueryAndRun(appCtx.AppReaderRequired, name, stream, includeGuid ?? false, appCtx, more, blockLookupOrNull);
        return l.Return(result);
    }

    #endregion

    private IDictionary<string, IEnumerable<EavLightEntity>> QuerySystemDataForCurrentSite(
        QueryParametersDtoFromClient? more,
        string? stream,
        bool includeGuid)
    {
        var blockContext = ctxService.BlockContextRequired();
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"stream:{stream}, withModule:{blockContext.Module.Id}");

        if (!blockContext.User.IsContentAdmin)
        {
            const string message = "Request not allowed";
            throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, message, message));
        }

        var lookUpEngine = lookupResolver.Value.GetLookUpEngine(blockContext.Module.Id);
        var dataSourceName = lookUpEngine.FindSource("QueryString")?.Get(nameof(SysData.SysDataSource)) ?? "";
        var options = new DataSourceOptions
        {
            AppIdentityOrReader = new AppIdentityPure(blockContext.Site.ZoneId, KnownAppsConstants.AppIdEmpty),
            LookUp = lookUpEngine,
            MyConfigValues = new Dictionary<string, string>
            {
                [nameof(SysData.SysDataSource)] = dataSourceName,
            }.ToImmutableInvIgnoreCase(),
        };
        var systemData = dataSourcesService.Value.Create<SysData>(options);

        var result = ConvertDataSource(systemData, stream, includeGuid, false, more);
        return l.Return(result);
    }

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

        var result = ConvertDataSource(query, stream, includeGuid, context.Permissions.IsContentAdmin, more);
        return l.Return(result);
    }

    private IDictionary<string, IEnumerable<EavLightEntity>> ConvertDataSource(
        IDataSource source,
        string? stream,
        bool includeGuid,
        bool withEdit,
        QueryParametersDtoFromClient? more)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"stream:{stream}");

        if (stream == DataSourceConstants.AllStreams)
            stream = null;

        var streamNames = DataSourceConvertHelper.GetBestStreamNames(source, stream);

        // Pass the originally requested stream so QueryODataParams can map bare OData options
        // to that stream when exactly one stream was explicitly selected.
        var streamOptions = QueryODataParams.CreateMany(source.Configuration.Parse, streamNames, stream);

        // New v17 experimental with special fields
        var systemQueryOptions = QueryODataParams.Create(source.Configuration.Parse);

        // v21 support OData filtering, sorting...
        var mustUseOData = streamOptions.Any(so => !so.Value.IsEmptyExceptForSelect());
        if (mustUseOData)
        {
            var oDataResult = oDataHelper.Value.ApplyOData(source, streamOptions, more?.Guids, includeGuid, withEdit);
            return l.Return(oDataResult, "processed with OData");
        }

        // Classic, lightweight conversion
        var selectFields = streamOptions.ToDictionary(
            pair => pair.Key,
            ICollection<string> (pair) => pair.Value.Select.ToListOpt(),
            StringComparer.OrdinalIgnoreCase
        );
        var dc = PrepareDataConverter(includeGuid, withEdit, systemQueryOptions);
        var result = dc.Convert(source, streamNames, more?.Guids, selectFields);
        return l.Return(result, "classic convert");
    }

    private IConvertToEavLight PrepareDataConverter(bool withGuid, bool isEditor, ODataOptions options)
    {
        var dc = dataConverter.New();
        dc.WithGuid = withGuid;
        if (dc is ConvertToEavLightWithCmsInfo serializerWithEdit)
            serializerWithEdit.WithEdit = isEditor;
        if (dc is ConvertToEavLight serializerWithOData)
            serializerWithOData.AddSelectFields(options.Select.ToListOpt());
        return dc;
    }
}
