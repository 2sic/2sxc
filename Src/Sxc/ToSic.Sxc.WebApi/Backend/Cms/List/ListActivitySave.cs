using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.ListSlots.Sys;
using ToSic.Eav.Data.Publishing.Sys;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivitySave(AppWorkChain<WorkListSlots> workFieldList, LazySvc<IDataPublishingService> publishing)
    : ServiceWithSetup<IAppWorkContext>("Api.CntGrpRl", connect: [workFieldList, publishing])
{
    public bool ItemList(Guid parent, List<ListSlotContentsInfo>? list,  string[] fields)
    {
        var l = Log.Fn<bool>($"list for:{parent}, items:{list?.Count}");
        if (list == null)
            throw l.Done(new ArgumentNullException(nameof(list)));

        if (!list.Any())
            return l.ReturnTrue();

        publishing.Value.DoInsidePublishing(options =>
        {
            var entity = MyOptions.AppReader.GetDraftOrPublished(parent);
            var sequence = list
                .Select(i => i.Index)
                .ToArray();
            workFieldList.New(MyOptions)
                .ReorderSlots(entity!, fields, sequence, options.ForceDraft);
        });

        return l.ReturnTrue();
    }


}