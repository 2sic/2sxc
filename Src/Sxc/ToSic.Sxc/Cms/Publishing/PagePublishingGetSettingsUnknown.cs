using ToSic.Eav;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Cms.Publishing
{
    public class BasicPagePublishingSettings : PagePublishingSettingsBase
    {
        public BasicPagePublishingSettings(WarnUseOfUnknown<BasicPagePublishingSettings> warn) : base(LogNames.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int moduleId) 
            => PublishingMode.DraftOptional;
    }
}
