using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.Backend.Cms;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HistoryControllerReal(GenWorkDb<WorkEntityVersioning> versioning)
    : ServiceBase("Api.CmsHistoryRl", connect: [versioning]), IHistoryController
{
    public const string LogSuffix = "Hist";

    public List<ItemHistory> Get(int appId, ItemIdentifier item)
        => versioning.New(appId: appId).VersionHistory(item.EntityId);


    public bool Restore(int appId, int changeId, ItemIdentifier item)
    {
        versioning.New(appId: appId).VersionRestore(item.EntityId, changeId);
        return true;
    }
}