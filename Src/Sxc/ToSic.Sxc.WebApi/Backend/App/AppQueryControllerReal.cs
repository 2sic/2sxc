using System.Net;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSource.Sys.Convert;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.WebApi.OData.Sys;
using ToSic.Eav.WebApi.Sys.App;
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
    Generator<IConvertToEavLight> dataConverter,
    LazySvc<PermissionCheckFactory> permCheckFactory,
    LazySvc<QueryBuilder<Query>> queryManager)
    : ServiceBase("Sxc.ApiApQ", connect: [oDataHelper, dataConverter, permCheckFactory, queryManager])
{
    public const string LogSuffix = "AppQry";

    public record Result(IContextOfApp AppCtx, ILookUpEngine Lookups);

    /// <summary>
    /// Run Queries which are in a container context
    /// </summary>
    /// <param name="appInfo"></param>
    /// <param name="name">query name</param>
    /// <param name="more">additional url parameters for odata</param>
    /// <param name="stream">optional stream name(s)</param>
    /// <param name="includeGuid">Include the guid in the result</param>
    /// <returns></returns>
    public IDictionary<string, IEnumerable<EavLightEntity>> QueryNew(Result appInfo, string name, QueryParametersDtoFromClient? more, string? stream = null, bool? includeGuid = false)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>($"'{name}', inclGuid: {includeGuid}, stream: {stream}");
        
        var parameters = PrepareParameters(appInfo.AppCtx, appInfo.Lookups, name, stream, includeGuid, more);

        var result = BuildQueryAndRun(parameters);
        return l.Return(result);
    }




    private Parameters PrepareParameters(IContextOfApp context, ILookUpEngine lookUp, string queryName, string? streams, bool? includeGuid, QueryParametersDtoFromClient? odata)
    {
        var l = Log.Fn<Parameters>($"name:{queryName}, stream:{streams}");

        if (string.IsNullOrEmpty(queryName))
            throw l.Ex(HttpException.MissingParam(nameof(queryName)));

        var query = GetQueryAndDoPermissionChecks(queryName, lookUp, context);

        streams = streams == DataSourceConstants.AllStreams ? null : streams;
        var streamNames = DataSourceConvertHelper.GetBestStreamNames(query, streams);

        return l.Return(new(
            //Context: context,
            AppReader: context.AppReaderRequired,
            Lookups: lookUp,
            Query: query,
            QueryName: queryName,
            Streams: streams,
            StreamNames: streamNames,
            IncludeGuids: includeGuid ?? false,
            OData: odata?.Guids
        ));
    }

    public record Parameters(
        //IContextOfApp Context,
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
        var (/*context, #DropWithEdit */ _, lookups, query, _, streams, streamNames, includeGuid, guidList) = parameters;

        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>();


        // Pass the originally requested stream so QueryODataParams can map bare OData options
        // to that stream when exactly one stream was explicitly selected.
        var streamOptions = QueryODataParams.CreateMany(d => lookups.LookUp(d), streamNames, streams);

        // v21 support OData filtering, sorting...
        var mustUseOData = streamOptions.Any(so => !so.Value.IsEmptyExceptForSelect());
        if (mustUseOData)
        {
            var oDataResult = oDataHelper.Value.ApplyOData(query, streamOptions, guidList, includeGuid/*, context.Permissions.IsContentAdmin #DropWithEdit*/);
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
        var dc = dataConverter.PrepareDataConverter(systemQueryOptions, includeGuid/*, context.Permissions.IsContentAdmin #DropWithEdit*/);
        var result = dc.Convert(query, streamNames, guidList, selectFields);
        return l.Return(result, "classic convert");
    }
}
