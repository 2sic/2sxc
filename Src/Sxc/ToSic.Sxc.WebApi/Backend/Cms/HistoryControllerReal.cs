using ToSic.Eav.Data.Versions.Sys;

namespace ToSic.Sxc.Backend.Cms;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HistoryControllerReal(AppWorkQuick<WorkEntityVersionRestore> versioning)
    : ServiceBase("Api.CmsHistoryRl", connect: [versioning])
{
    public const string LogSuffix = "Hist";


    public bool Restore(int appId, int transactionId, ItemIdentifier item)
    {
        versioning.New(appId).VersionRestore(item.EntityId, transactionId);
        return true;
    }
}