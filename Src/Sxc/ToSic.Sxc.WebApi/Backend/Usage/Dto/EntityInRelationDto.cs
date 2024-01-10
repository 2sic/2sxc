using ToSic.Eav.Data;

namespace ToSic.Sxc.WebApi.Usage.Dto;

class EntityInRelationDto: EntityDto
{
    public string Relationship;
    public string Key;

    public EntityInRelationDto(IEntity entity, string relationship, string key) : base(entity)
    {
        Relationship = relationship;
        Key = key;
    }
}