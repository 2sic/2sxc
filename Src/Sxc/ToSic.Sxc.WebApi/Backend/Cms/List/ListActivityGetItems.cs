using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.WebApi.Sys.Cms;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityGetItems() : ServiceWithSetup<IAppDataContext>("Act.LstGet")
{

    public List<EntityInListDto> ItemList(Guid parent, string part)
    {
        var l = Log.Fn<List<EntityInListDto>>($"item list for:{parent}");
        var appReader = MyOptions.AppReader;
        var cg = appReader.GetDraftOrPublished(parent)!;
        var itemList = cg.Children(part);
        var list = itemList
            .Select(appReader.GetDraftOrKeep)
            .Select((e, index) => new EntityInListDto(e, index))
            .ToList();

        return l.Return(list);
    }

}