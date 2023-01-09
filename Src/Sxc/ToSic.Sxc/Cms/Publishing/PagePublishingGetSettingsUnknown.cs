using ToSic.Eav;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public class PagePublishingGetSettingsUnknown : PagePublishingGetSettingsBase
    {
        public PagePublishingGetSettingsUnknown(WarnUseOfUnknown<PagePublishingGetSettingsUnknown> warn) : base(LogScopes.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int moduleId) 
            => PublishingMode.DraftOptional;
    }
}
