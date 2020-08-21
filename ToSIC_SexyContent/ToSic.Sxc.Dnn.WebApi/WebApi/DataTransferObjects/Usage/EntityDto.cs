using ToSic.Eav.Data;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class EntityDto: IdentifierBase
    {
        public string Title;
        public ContentTypeDto Type;

        public EntityDto(IEntity entity)
        {
            Id = entity.EntityId;
            Guid = entity.EntityGuid;
            Title = entity.GetBestTitle();
            Type = new ContentTypeDto(entity.Type);
        }
    }
}
