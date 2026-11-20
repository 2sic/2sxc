using ToSic.Eav.Apps.Sys.State;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Get all the items in a list.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityGetItems() : ServiceWithSetup<IAppDataContext>("Act.LstGet")
{
    public List<ListSlotContentsInfo> ItemList(Guid parent, string part)
    {
        var l = Log.Fn<List<ListSlotContentsInfo>>($"item list for:{parent}");
        var appReader = MyOptions.AppReader;
        var cg = appReader.GetDraftOrPublished(parent)!;
        var itemList = cg.Children(part);
        var list = itemList
            .Select(appReader.GetDraftOrKeep)
            .Select((e, index) => new ListSlotContentsInfo(e, index))
            .ToList();

        return l.Return(list);
    }
}
