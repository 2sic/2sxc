using ToSic.Eav.DataSources.Query.Sys;

namespace ToSic.Sxc.Backend.Admin.Query;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkQueryDeleteIfUnused(AppWorkChain<WorkViewsGet> workViews, AppWorkChain<WorkQueryDelete> workUnitQueryDelete)
    : ServiceWithSetup<IAppWorkContext>("AWk.QryMod", connect: [workViews, workUnitQueryDelete])
{
    /// <summary>
    /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations.
    /// Stops if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
    /// </summary>
    public bool DeleteIfUnused(int id)
    {
        var l = Log.Fn<bool>($"appId: {MyOptions.AppReader.AppId}; {nameof(id)}: {id}");

        // Stop if views still use this Query
        var viewUsingQuery = workViews.New(MyOptions).AllViews
            .Where(t => t.Query?.Id == id)
            .Select(t => t.Id)
            .ToArray();

        if (viewUsingQuery.Any())
            throw l.Done(new Exception($"Query is used by Views and can't be deleted. Query ID: {id}. Views: {string.Join(", ", viewUsingQuery)}"));

        var queryMod = workUnitQueryDelete.New(MyOptions);
        return l.Return(queryMod.Delete(id));
    }
}