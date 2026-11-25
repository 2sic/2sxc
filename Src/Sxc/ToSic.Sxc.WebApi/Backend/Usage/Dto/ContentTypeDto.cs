namespace ToSic.Sxc.Backend.Usage.Dto;

public record ContentTypeDto : IIdentifierDto
{
    public int Id { get; init; }
    public Guid Guid { get; init; }
    public string Name;
    public string StaticName;

    public ContentTypeDto(IContentType type)
    {
        Id = type.Id;
        Name = type.Name;
        StaticName = type.NameId;
    }
}