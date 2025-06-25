namespace ToSic.Sxc.Blocks.Sys;

/// <inheritdoc />
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockIdentifier(int zoneId, int appId, string? appDebugNameId, Guid guid, Guid viewOverride)
    : IBlockIdentifier
{
    /// <inheritdoc />
    public int ZoneId { get; } = zoneId;

    /// <inheritdoc />
    public int AppId { get; } = appId;

    public string? AppDebugNameId { get; } = appDebugNameId;

    /// <inheritdoc />
    public Guid Guid { get; } = guid;

    /// <inheritdoc />
    public Guid PreviewView { get; } = viewOverride;
}