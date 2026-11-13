namespace ToSic.Sxc.Backend.Usage.Dto;

public record EntityDto : IIdentifierDto
{
    public int Id { get; init; }
    public Guid Guid { get; init; }
    public string? Title;
    public ContentTypeDto Type;

    public EntityDto(IEntity entity)
    {
        Id = entity.EntityId;
        Guid = entity.EntityGuid;
        Title = entity.GetBestTitle();
        Type = new(entity.Type);
    }
}