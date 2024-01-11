namespace ToSic.Sxc.Backend.Cms;

public interface IContentGroupController
{
    EntityInListDto Header(Guid guid);
    void Replace(Guid guid, string part, int index, int entityId, bool add = false);
    ReplacementListDto Replace(Guid guid, string part, int index);
    List<EntityInListDto> ItemList(Guid guid, string part);
    bool ItemList(Guid guid, List<EntityInListDto> list, string part = null);
}