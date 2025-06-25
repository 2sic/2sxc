namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// Tell the save operations if saving should trigger change-detection at page level to start work flows
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockPublishingSettings
{
    public bool AllowDraft = true;

    public bool ForceDraft = false;

    public PublishingMode Mode = PublishingMode.DraftOptional;
}