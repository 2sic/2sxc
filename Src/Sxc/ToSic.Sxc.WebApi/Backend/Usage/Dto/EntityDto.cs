namespace ToSic.Sxc.Backend.Usage.Dto;

public class EntityDto: IdentifierDto
{
    public string Title;
    public ContentTypeDto Type;

    public EntityDto(IEntity entity)
    {
        Id = entity.EntityId;
        Guid = entity.EntityGuid;
        Title = entity.GetBestTitle();
        Type = new(entity.Type);
    }
}