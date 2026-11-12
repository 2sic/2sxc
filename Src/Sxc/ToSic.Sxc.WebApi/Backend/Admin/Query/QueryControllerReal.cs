using ToSic.Eav.DataSources.ImportExport.Sys;
using ToSic.Eav.DataSources.Query.Dev.Sys;
using ToSic.Eav.DataSources.Query.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.LookUp.Sys;

namespace ToSic.Sxc.Backend.Admin.Query;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class QueryControllerReal(
    AppWorkQuick<WorkViewsGet> workViews,
    AppWorkQuick<WorkQueryCopy> workUnitQueryCopy,
    AppWorkQuick<WorkQueryDelete> workUnitQueryDelete,
    IContextCompleteService ctxSvc,
    AppWorkQuick<WorkQueryImport> queryImport,
    AppWorkQuick<WorkQueryDeleteIfUnused> queryDeleteIfUnused,
    AppWorkQuick<WorkQuerySave> querySave,
    QueryDevSvc queryDev,
    QueryDevDebugSvc queryDevDebug,
    Generator<IAppDataConfigProvider> tokenEngineWithContext)
    : ServiceBase("Api." + LogSuffix, connect: [workViews, workUnitQueryCopy, workUnitQueryDelete, queryImport, queryDeleteIfUnused, ctxSvc, querySave, queryDev, queryDevDebug, tokenEngineWithContext])
{
    public const string LogSuffix = "Query";
    public const string LogGroup = EavWebApiConstants.HistoryNameWebApi + "-query";

    public bool DeleteIfUnused(int appId, int id) => queryDeleteIfUnused.New(appId).DeleteIfUnused(id);


    public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25)
        => queryDevDebug.DebugStream(appId, id, top, LookUpEngineWithBlockRequired(), from, @out);

    /// <summary>
    /// Query the Result of a Pipeline using Test-Parameters
    /// </summary>
    public QueryRunDto RunDev(int appId, int id, int top)
        => queryDev.RunDev(appId, id, LookUpEngineWithBlockRequired(), top, builtQuery => builtQuery.Main);

    private ILookUpEngine LookUpEngineWithBlockRequired()
    {
        var block = ctxSvc.BlockRequired();
        var specs = new SxcAppDataConfigSpecs { BlockForLookupOrNull = block };
        var lookUps = tokenEngineWithContext.New()
            .GetDataConfiguration(block.App, specs)
            .LookUpEngine;
        return lookUps;
    }

    /// <summary>
    /// Save Pipeline
    /// </summary>
    /// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
    /// <param name="appId">AppId this Pipeline belongs to</param>
    /// <param name="id">PipelineEntityId</param>
    public QueryDefinitionDto Save(QueryDefinitionDto data, int appId, int id)
        => querySave.New(appId).Save(data, id);

    /// <summary>
    /// Clone a Pipeline with all DataSources and their configurations
    /// </summary>
    public void Clone(int appId, int id) => workUnitQueryCopy.New(appId: appId).SaveCopy(id);

    public bool Import(EntityImportDto args)
    {
        var l = Log.Fn<bool>(args.DebugInfo);
        return l.Return(queryImport.New(args.AppId).Import(args.AppId, args.GetContentString()));
    }
}