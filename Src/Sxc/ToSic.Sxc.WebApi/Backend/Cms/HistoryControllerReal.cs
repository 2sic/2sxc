using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HistoryControllerReal : ServiceBase, IHistoryController
{
    public const string LogSuffix = "Hist";

    // #UnusedFeatureHistoryOfGroup 2022-07-05 2dm removed - probably clean up ca. Q4 2022
    public HistoryControllerReal(GenWorkDb<WorkEntityVersioning> versioner) : base("Api.CmsHistoryRl")
    {
        ConnectServices(
            _versioner = versioner
        );
    }

    private readonly GenWorkDb<WorkEntityVersioning> _versioner;


    public List<ItemHistory> Get(int appId, ItemIdentifier item)
        => _versioner.New(appId: appId).VersionHistory(item.EntityId);


    public bool Restore(int appId, int changeId, ItemIdentifier item)
    {
        _versioner.New(appId: appId).VersionRestore(item.EntityId, changeId);
        return true;
    }
}