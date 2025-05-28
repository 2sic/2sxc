namespace ToSic.Eav.Cms.Internal;

/// <inheritdoc />
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockIdentifier(int zoneId, int appId, string appNameId, Guid guid, Guid viewOverride)
    : IBlockIdentifier
{
    /// <inheritdoc />
    public int ZoneId { get; } = zoneId;

    /// <inheritdoc />
    public int AppId { get; } = appId;

    public string AppNameId { get; } = appNameId;

    /// <inheritdoc />
    public Guid Guid { get; } = guid;

    /// <inheritdoc />
    public Guid PreviewView { get; } = viewOverride;
}