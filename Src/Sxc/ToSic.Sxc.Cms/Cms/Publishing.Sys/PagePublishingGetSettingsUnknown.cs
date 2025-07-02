namespace ToSic.Sxc.Cms.Publishing.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class PagePublishingGetSettingsUnknown : PagePublishingGetSettingsBase
{
    public PagePublishingGetSettingsUnknown(WarnUseOfUnknown<PagePublishingGetSettingsUnknown> _) : base(LogScopes.NotImplemented) { }

    protected override PublishingMode LookupRequirements(int moduleId) 
        => PublishingMode.DraftOptional;
}