using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Cms.Publishing
{
    /// <summary>
    /// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
    /// </summary>
    public class PagePublishingGetSettingsForbidden: HasLog, IPagePublishingGetSettings
    {
        #region Constructors

        public PagePublishingGetSettingsForbidden(IFeaturesService featuresService) : base("Cms.PubForb")
        {
            _featuresService = featuresService;
        }
        private readonly IFeaturesService _featuresService;

        #endregion

        public BlockPublishingSettings SettingsOfModule(int moduleId) => new BlockPublishingSettings
        {
            AllowDraft = false,
            ForceDraft = false,
            Mode = PublishingMode.DraftForbidden
        };

        public string NameId => "DraftForbidden";

        public bool IsViable() => _featuresService.IsEnabled(Eav.Configuration.BuiltInFeatures.EditUiDisableDraft.NameId);

        public int Priority => (int)PagePublishingPriorities.DraftForbidden;
    }
}
