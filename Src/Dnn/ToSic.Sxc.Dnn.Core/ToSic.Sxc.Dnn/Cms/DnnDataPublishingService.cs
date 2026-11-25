using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Data.Publishing.Sys;
using ToSic.Eav.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Dnn.Cms;

internal class DnnDataPublishingService(IContextCompleteService ctxSvc, IUser user)
    : ServiceBase("Dnn.Publsh", connect: [ctxSvc, user]),
        IDataPublishingService
{

    public void DoInsidePublishing(Action<PublishingOptions> action)
    {
        var blockCtxOrNull = ctxSvc.BlockContextOrNull();
        var publishingEnabled = blockCtxOrNull?.Publishing.ForceDraft ?? false;
        var modId = blockCtxOrNull?.Module.Id ?? EavConstants.IdNotInitialized;

        var userId = user.Id;

        var l = Log.Fn($"{nameof(modId)}:{modId}, user:{userId}, {nameof(publishingEnabled)}:{publishingEnabled}");

        if (publishingEnabled)
        {
            var moduleVersionSettings = new DnnModuleVersioningHelper(modId, Log);

            // Get an new version number and submit it to DNN
            // The submission must be made every time something changes, because a "discard" could have happened
            // in the meantime.
            TabChangeTracker.Instance.TrackModuleModification(
                moduleVersionSettings.ModuleInfo,
                moduleVersionSettings.IncreaseLatestVersion(),
                userId
            );
        }

        var versioningActionInfo = new PublishingOptions { ForceDraft = publishingEnabled };
        action.Invoke(versioningActionInfo);
        l.Done();
    }

}