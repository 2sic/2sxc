using ToSic.Sxc.Services;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlockPublishingLookupPreflightDisabled(IFeaturesService featuresService)
    : ServiceBase("Pub.Forb", connect: [featuresService]),
        IWorkBlockPublishingLookup
{
    /// <summary>
    /// Highest priority, must come first
    /// </summary>
    public int WorkSequenceOrder => (int)BlockPublishingLookupSequence.PreflightDisabled;
    
    /// <summary>
    /// Work
    /// </summary>
    public Task<Package<BlockPublishingSettings>> Handle(WorkContext _, Package<BlockPublishingSettings> package)
    {
        var forbidden = featuresService.IsEnabled(BuiltInFeatures.EditUiDisableDraft.NameId);
        var result = forbidden
            ? BlockPublishingSettingsService.Stop(package, PublishingMode.DraftForbidden)
            : package with { Decision = ResultState.Skip };
        
        return Task.FromResult(result);
    }

}