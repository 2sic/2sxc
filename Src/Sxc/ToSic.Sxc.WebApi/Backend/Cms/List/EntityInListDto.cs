namespace ToSic.Sxc.Backend.Cms;

public record EntityInListDto
{
    public int Index { get; init; }
    public int Id { get; init; }
    public Guid Guid { get; init; }
    public string? Title { get; init; }
    public string? Type { get; init; }
    // 2026-06-22 2dm - seems unused
    //public JsonType? TypeWip;

    public EntityInListDto(IEntity? c, int index)
    {
        Index = index;
        Id = c?.EntityId ?? 0;
        Guid = c?.EntityGuid ?? Guid.Empty;
        Title = c?.GetBestTitle() ?? "";
        Type = c?.Type.NameId;
        // 2026-06-22 2dm - seems unused
        //TypeWip = c?.Type.NameId == null ? null : new JsonType(c)
    }
}