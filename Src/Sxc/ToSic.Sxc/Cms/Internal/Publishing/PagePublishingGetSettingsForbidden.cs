using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Cms.Internal.Publishing;

/// <summary>
/// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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