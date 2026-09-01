using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.WebApi.Sys.Cms;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityGetItems(
    ISxcCurrentContextService ctxService
    )
    : ServiceBase("Act.LstGet", connect: [ctxService])
{

    public List<EntityInListDto> ItemList(Guid parent, string part)
    {
        var l = Log.Fn<List<EntityInListDto>>($"item list for:{parent}");
        var context = ctxService.BlockContextRequired();
        var cg = context.AppReaderRequired.GetDraftOrPublished(parent)!;
        var itemList = cg.Children(part);
        var list = itemList
            .Select(context.AppReaderRequired.GetDraftOrKeep)
            .Select((e, index) => new EntityInListDto(e, index))
            .ToList();

        return l.Return(list);
    }

}