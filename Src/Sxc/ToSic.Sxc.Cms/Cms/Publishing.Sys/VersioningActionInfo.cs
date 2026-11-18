namespace ToSic.Sxc.Cms.Publishing.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record VersioningActionInfo
{
    public required bool ForceDraft { get; init; }

    // TODO: Add properties required (like the latest version)
}