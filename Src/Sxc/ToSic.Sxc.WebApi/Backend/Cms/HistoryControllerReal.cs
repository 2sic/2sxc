using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Sys.Cms;

namespace ToSic.Sxc.Backend.Cms;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HistoryControllerReal(GenWorkDb<WorkEntityVersioning> versioning)
    : ServiceBase("Api.CmsHistoryRl", connect: [versioning]), IHistoryController
{
    public const string LogSuffix = "Hist";

    public List<ItemHistory> Get(int appId, ItemIdentifier item)
        => versioning.New(appId: appId).VersionHistory(item.EntityId);


    public bool Restore(int appId, int transactionId, ItemIdentifier item)
    {
        versioning.New(appId: appId).VersionRestore(item.EntityId, transactionId);
        return true;
    }
}