using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// This is a fallback work block for the publishing lookup, which is used when no other work block can handle the request.
/// It will stop the publishing process and return a draft forbidden result.
/// Normally it should never be run, since platform specific implementations should already provide an answer.
/// </summary>
/// <param name="_"></param>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlockPublishingLookupUnknown(WarnUseOfUnknown<WorkBlockPublishingLookupUnknown> _)
    : ServiceBase(LogScopes.NotImplemented + ".PubUnk"),
        IWorkBlockPublishingLookup
{

    public Task<Package<BlockPublishingSettings>> Handle(WorkContext _, Package<BlockPublishingSettings> package)
    {
        return Task.FromResult(BlockPublishingSettingsService.Stop(package, PublishingMode.DraftForbidden));
    }

    public int WorkSequenceOrder => (int)BlockPublishingLookupSequence.Fallback;
}