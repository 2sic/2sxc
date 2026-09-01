namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// Information about the current block/module having drafts allowed / disabled.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record BlockPublishingSettings
{
    /// <summary>
    /// The ModuleId is required, as it's used to detect the settings.
    /// </summary>
    public required int ModuleId { get; init; }
    
    public bool AllowDraft = true;

    public bool ForceDraft = false;

    public PublishingModes Mode = PublishingModes.DraftOptional;

    /// <summary>
    /// Helper to repackage the result.
    /// </summary>
    public static BlockPublishingSettings Update(BlockPublishingSettings options, PublishingModes mode)
        => options with
        {
            AllowDraft = mode != PublishingModes.DraftForbidden,
            ForceDraft = mode == PublishingModes.DraftRequired,
            Mode = mode
        };
}