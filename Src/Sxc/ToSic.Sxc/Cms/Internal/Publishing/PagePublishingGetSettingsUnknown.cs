using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Cms.Internal.Publishing;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PagePublishingGetSettingsUnknown : PagePublishingGetSettingsBase
{
    public PagePublishingGetSettingsUnknown(WarnUseOfUnknown<PagePublishingGetSettingsUnknown> _) : base(LogScopes.NotImplemented) { }

    protected override PublishingMode LookupRequirements(int moduleId) 
        => PublishingMode.DraftOptional;
}