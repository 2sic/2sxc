namespace ToSic.Sxc.Adam.Sys;

/// <summary>
/// Dummy interface to mark a object which is used as a file identifier.
/// This is to avoid specifying the underlying type of the real ID.
/// </summary>
public record AdamAssetIdentifier
{
    public static AdamAssetId<T> Create<T>(T sysId) => new() { SysId = sysId };
}


public record AdamAssetId<TFileId> : AdamAssetIdentifier
{
    public TFileId SysId { get; init; } = default!;
}
