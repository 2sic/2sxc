using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class PagePublishingGetSettingsUnknown : PagePublishingGetSettingsBase
    {
        public PagePublishingGetSettingsUnknown(WarnUseOfUnknown<PagePublishingGetSettingsUnknown> _) : base(LogScopes.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int moduleId) 
            => PublishingMode.DraftOptional;
    }
}
