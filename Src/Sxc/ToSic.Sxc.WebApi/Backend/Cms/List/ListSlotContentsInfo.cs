namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Important: Empty constructor is needed for JSON serialization.
/// </summary>
public record ListSlotContentsInfo()
{
    /// <summary>
    /// Position in the list.
    /// </summary>
    public int Index { get; init; }
    public int Id { get; init; }
    public Guid Guid { get; init; }
    public string? Title { get; init; }
    public string? Type { get; init; }

    /// <summary>
    /// Quick constructor to create from an entity.
    /// </summary>
    public ListSlotContentsInfo(IEntity? entity, int index): this()
    {
        Index = index;
        Id = entity?.EntityId ?? 0;
        Guid = entity?.EntityGuid ?? Guid.Empty;
        Title = entity?.GetBestTitle() ?? "";
        Type = entity?.Type.NameId;
    }
}
