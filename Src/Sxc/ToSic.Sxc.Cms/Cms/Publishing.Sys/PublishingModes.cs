namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// Defines the publishing mode for content in the CMS.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public enum PublishingModes
{
    /// <summary>
    /// Draft is allowed / supported.
    /// </summary>
    DraftOptional,

    /// <summary>
    /// Draft is required, meaning the current user cannot directly publish.
    /// </summary>
    DraftRequired,
    
    /// <summary>
    /// Draft is forbidden, meaning the platform doesn't support it or the current user can't use drafts.
    /// </summary>
    DraftForbidden,
}