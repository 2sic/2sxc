using ToSic.Eav.Apps.Enums;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Cms.Publishing
{
    /// <summary>
    /// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
    /// </summary>
    public class PagePublishingGetSettingsOptional : ServiceBase, IPagePublishingGetSettings
    {
        #region Constructors

        public PagePublishingGetSettingsOptional() : base("Cms.PubNone") { }

        #endregion

        public BlockPublishingSettings SettingsOfModule(int moduleId) => new BlockPublishingSettings
        {
            AllowDraft = true,
            ForceDraft = false,
            Mode = PublishingMode.DraftOptional
        };

        public string NameId => "NoPublishing";

        public bool IsViable() => true;

        public int Priority => (int)PagePublishingPriorities.Unknown;
    }
}
