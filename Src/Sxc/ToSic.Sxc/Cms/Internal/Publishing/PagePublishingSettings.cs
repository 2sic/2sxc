using ToSic.Eav.Cms.Internal;

namespace ToSic.Sxc.Cms.Internal.Publishing;

/// <summary>
/// Tell the save operations if saving should trigger change-detection at page level to start work flows
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockPublishingSettings
{
    public bool AllowDraft = true;

    public bool ForceDraft = false;

    public PublishingMode Mode = PublishingMode.DraftOptional;
}