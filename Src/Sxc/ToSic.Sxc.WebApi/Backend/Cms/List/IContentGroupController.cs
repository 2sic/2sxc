namespace ToSic.Sxc.Backend.Cms;

public interface IContentGroupController
{
    EntityInListDto? Header(Guid guid);
    void Replace(Guid parent, string part, int index, int entityId, bool add = false);
    ReplacementListDto? Replace(Guid parent, string part, int index);
    List<EntityInListDto> ItemList(Guid parent, string part);
    bool ItemList(Guid parent, List<EntityInListDto> list, string? part = null);
}