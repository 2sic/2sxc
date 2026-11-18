using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms;

internal class OqtPagePublishing(IContextCompleteService ctxSvc)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Publsh", connect: [ctxSvc]),
    IPagePublishing
{
    public void DoInsidePublishing(Action<VersioningActionInfo> action)
    {
        var blockCtxOrNull = ctxSvc.BlockContextOrNull();
        var containerId = blockCtxOrNull?.Module.Id ?? Eav.Sys.EavConstants.IdNotInitialized;
        var userId = 0;
        var enabled = false;
        Log.A($"DoInsidePublishing(module:{containerId}, user:{userId}, enabled:{enabled})");
        if (enabled)
        {
            /* ignore */
        }

        var versioningActionInfo = new VersioningActionInfo { ForceDraft = enabled };
        action.Invoke(versioningActionInfo);
        Log.A("/DoInsidePublishing");
    }



    public int GetLatestVersion(int instanceId) => 0;

    public int GetPublishedVersion(int instanceId) => 0;


    public void Publish(int instanceId, int version) 
        => Log.A($"Publish(m:{instanceId}, v:{version}) - not supported in Oqtane, publish never happened ");
}