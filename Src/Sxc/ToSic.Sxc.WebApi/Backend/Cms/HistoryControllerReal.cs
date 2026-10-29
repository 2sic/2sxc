using ToSic.Eav.WebApi.Sys.Cms;

namespace ToSic.Sxc.Backend.Cms;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HistoryControllerReal(AppWorkQuick<WorkEntityVersioning> versioning)
    : ServiceBase("Api.CmsHistoryRl", connect: [versioning]), IHistoryController
{
    public const string LogSuffix = "Hist";

    // 2026-09-01 not in use any more, replaced with #SysData
    //public List<ToSic.Eav.Persistence.Versions.ItemHistory> Get(int appId, ItemIdentifier item)
    //    => versioning.New(appId: appId).VersionHistory(item.EntityId);


    public bool Restore(int appId, int transactionId, ItemIdentifier item)
    {
        versioning.New(appId: appId).VersionRestore(item.EntityId, transactionId);
        return true;
    }
}