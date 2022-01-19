using ToSic.Eav.Data;
using ToSic.Sxc.WebApi.SharedDto;

namespace ToSic.Sxc.WebApi.Usage.Dto
{
    public class ContentTypeDto: IdentifierDto
    {
        public string Name;
        public string StaticName;

        public ContentTypeDto(IContentType type)
        {
            Id = type.ContentTypeId;
            Name = type.Name;
            StaticName = type.NameId;
        }
    }
}
