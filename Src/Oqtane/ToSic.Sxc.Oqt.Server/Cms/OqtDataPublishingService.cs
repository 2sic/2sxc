using ToSic.Eav.Data.Publishing.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms;
internal class OqtDataPublishingService(IContextCompleteService ctxSvc)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Publsh", connect: [ctxSvc]),
        IDataPublishingService
{
    public void DoInsidePublishing(Action<PublishingOptions> action)
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

        var versioningActionInfo = new PublishingOptions { ForceDraft = enabled };
        action.Invoke(versioningActionInfo);
        Log.A("/DoInsidePublishing");
    }
}
