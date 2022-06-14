using ToSic.Eav;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Cms.Publishing
{
    public class PagePublishingGetSettingsUnknown : PagePublishingGetSettingsBase
    {
        public PagePublishingGetSettingsUnknown(WarnUseOfUnknown<PagePublishingGetSettingsUnknown> warn) : base(LogNames.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int moduleId) 
            => PublishingMode.DraftOptional;
    }
}
