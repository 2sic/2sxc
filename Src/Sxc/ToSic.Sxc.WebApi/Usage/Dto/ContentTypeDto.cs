using ToSic.Eav.Data;

namespace ToSic.Sxc.WebApi.Usage.Dto
{
    public class ContentTypeDto: IdentifierBase
    {
        public string Name;
        public string StaticName;

        public ContentTypeDto(IContentType type)
        {
            Id = type.ContentTypeId;
            Name = type.Name;
            StaticName = type.StaticName;
        }
    }
}
