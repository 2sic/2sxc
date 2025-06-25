using ToSic.Eav.Cms.Internal;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Cms.Internal.Publishing;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class PagePublishingGetSettingsUnknown : PagePublishingGetSettingsBase
{
    public PagePublishingGetSettingsUnknown(WarnUseOfUnknown<PagePublishingGetSettingsUnknown> _) : base(LogScopes.NotImplemented) { }

    protected override PublishingMode LookupRequirements(int moduleId) 
        => PublishingMode.DraftOptional;
}