using ToSic.Eav.Apps.Enums;

namespace ToSic.Sxc.Cms.Publishing
{
    /// <summary>
    /// Tell the save operations if saving should trigger change-detection at page level to start work flows
    /// </summary>
    public class BlockPublishingSettings
    {
        public bool ForceDraft = false;

        public PublishingMode Mode = PublishingMode.DraftOptional;
    }
}
