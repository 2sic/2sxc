namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// Defines the publishing mode for content in the CMS.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public enum PublishingMode
{
    DraftOptional,
    DraftRequired,
    DraftForbidden,
}