using ToSic.Eav.WebApi.Sys.Cms;

namespace ToSic.Sxc.Backend.Cms;

public interface IContentGroupController
{
    EntityInListDto? Header(Guid guid);
    List<EntityInListDto> ItemList(Guid parent, string part);
    bool ItemList(Guid parent, List<EntityInListDto> list, string? part = null);
}