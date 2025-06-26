using ToSic.Lib.Services;
using ToSic.Sxc.Services;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PagePublishingGetSettingsForbidden(IFeaturesService featuresService)
    : ServiceBase("Cms.PubForb", connect: [featuresService]), IPagePublishingGetSettings
{
    public BlockPublishingSettings SettingsOfModule(int moduleId) => new()
    {
        AllowDraft = false,
        ForceDraft = false,
        Mode = PublishingMode.DraftForbidden
    };

    public string NameId => "DraftForbidden";

    public bool IsViable() => featuresService.IsEnabled(BuiltInFeatures.EditUiDisableDraft.NameId);

    public int Priority => (int)PagePublishingPriorities.DraftForbidden;
}