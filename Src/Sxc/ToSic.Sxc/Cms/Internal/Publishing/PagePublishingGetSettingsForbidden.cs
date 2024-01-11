using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Cms.Internal.Publishing;

/// <summary>
/// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PagePublishingGetSettingsForbidden: ServiceBase, IPagePublishingGetSettings
{
    #region Constructors

    public PagePublishingGetSettingsForbidden(IFeaturesService featuresService) : base("Cms.PubForb")
    {
        ConnectServices(
            _featuresService = featuresService
        );
    }
    private readonly IFeaturesService _featuresService;

    #endregion

    public BlockPublishingSettings SettingsOfModule(int moduleId) => new()
    {
        AllowDraft = false,
        ForceDraft = false,
        Mode = PublishingMode.DraftForbidden
    };

    public string NameId => "DraftForbidden";

    public bool IsViable() => _featuresService.IsEnabled(BuiltInFeatures.EditUiDisableDraft.NameId);

    public int Priority => (int)PagePublishingPriorities.DraftForbidden;
}