namespace ToSic.Sxc.Backend.Usage.Dto;

class EntityInRelationDto(IEntity entity, string relationship, string key) : EntityDto(entity)
{
    public string Relationship = relationship;
    public string Key = key;
}