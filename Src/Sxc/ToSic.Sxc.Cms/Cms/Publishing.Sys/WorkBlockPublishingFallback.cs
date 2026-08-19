using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// This is the fallback page publishing strategy, which basically says that page publishing is optional.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlockPublishingFallback() : ServiceBase("Pub.Opt"), IWorkBlockPublishingLookup
{
    public Task<Package<BlockPublishingSettings>> Handle(WorkContext _, Package<BlockPublishingSettings> package)
    {
        return Task.FromResult(BlockPublishingSettingsService.Stop(package, PublishingMode.DraftOptional));
    }

    public int WorkSequenceOrder => (int)BlockPublishingLookupSequence.Fallback;
}