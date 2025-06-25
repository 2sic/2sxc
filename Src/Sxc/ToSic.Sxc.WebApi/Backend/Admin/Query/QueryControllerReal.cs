using ToSic.Eav.Apps.Internal;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Admin.Query;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.LookUp.Sys;

namespace ToSic.Sxc.Backend.Admin.Query;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class QueryControllerReal(
    QueryControllerBase<QueryControllerReal>.MyServices services,
    GenWorkPlus<WorkViews> workViews,
    ISxcCurrentContextService currentContextService,
    Generator<IAppDataConfigProvider> tokenEngineWithContext)
    : QueryControllerBase<QueryControllerReal>(services, "Api." + LogSuffix, connect: [workViews, currentContextService, tokenEngineWithContext])
{
    public const string LogSuffix = "Query";
    public const string LogGroup = EavWebApiConstants.HistoryNameWebApi + "-query";

    /// <summary>
    /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations.
    /// Stops if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
    /// </summary>
    public bool DeleteIfUnused(int appId, int id)
    {
        var l = Log.Fn<bool>($"{nameof(appId)}: {appId}; {nameof(id)}: {id}");

        // Stop if views still use this Query
        var viewUsingQuery = workViews.New(appId)
            .GetAll()
            .Where(t => t.Query?.Id == id)
            .Select(t => t.Id)
            .ToArray();

        if (viewUsingQuery.Any())
            throw l.Done(new Exception($"Query is used by Views and can't be deleted. Query ID: {id}. TemplateIds: {string.Join(", ", viewUsingQuery)}"));

        var queryMod = Services.WorkUnitQueryMod.New(appId: appId);
        return l.Return( queryMod.Delete(id));
    }
        


    public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25) 
        => DebugStream(appId, id, top, LookUpEngineWithBlockRequired(), from, @out);

    /// <summary>
    /// Query the Result of a Pipeline using Test-Parameters
    /// </summary>
    public QueryRunDto RunDev(int appId, int id, int top)
        => RunDevInternal(appId, id, LookUpEngineWithBlockRequired(), top, builtQuery => builtQuery.Main);

    private ILookUpEngine LookUpEngineWithBlockRequired()
    {
        var block = currentContextService.BlockRequired();
        var specs = new SxcAppDataConfigSpecs { BlockForLookupOrNull = block };
        var lookUps = tokenEngineWithContext.New()
            .GetDataConfiguration((SxcAppBase)block.App, specs)
            .Configuration;
        return lookUps;
    }

}