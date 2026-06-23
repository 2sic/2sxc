using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Cms.Publishing.Sys;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivitySave(
    GenWorkDb<WorkFieldList> workFieldList,
    LazySvc<IPagePublishing> publishing,
    ISxcCurrentContextService ctxService
    ) : ServiceBase("Api.CntGrpRl", connect: [workFieldList, ctxService, publishing])
{
    public bool ItemList(Guid parent, List<EntityInListDto>? list,  string part)
    {
        var l = Log.Fn<bool>($"list for:{parent}, items:{list?.Count}");
        if (list == null)
            throw l.Done(new ArgumentNullException(nameof(list)));

        if (!list.Any())
            return l.ReturnTrue();

        var context = ctxService.BlockContextRequired();
        publishing.Value.DoInsidePublishing(context, _ =>
        {
            var entity = context.AppReaderRequired.GetDraftOrPublished(parent);
            var sequence = list
                .Select(i => i.Index)
                .ToArray();
            var fields = part == ViewParts.ContentLower
                ? ViewParts.ContentPair
                : [part ?? throw new ArgumentException(@"Part name cannot be null", nameof(part))];
            workFieldList.New(context.AppReaderRequired)
                .FieldListReorder(entity!, fields, sequence, context.Publishing.ForceDraft);
        });

        return l.ReturnTrue();
    }
}