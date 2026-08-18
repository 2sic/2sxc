using ToSic.Eav.Apps.Sys.State;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityReplace(
    AppWorkQuick<WorkFieldList> workFieldList,
    Generator<IPagePublishing> publishing,
    ISxcCurrentContextService ctxService
) : ServiceBase("Act.LsRpOp", connect: [workFieldList, ctxService, publishing])
{
    public record Options(
        Guid Guid,
        string Part,
        int Index,
        int EntityId,
        bool Add
    );

    public void Replace(Options options)
    {
        var l = Log.Fn($"options:{options}");

        // use dnn versioning - this is always part of page
        var context = ctxService.BlockContextRequired();
        publishing.New().DoInsidePublishing(context, _ => InternalSave(context, options));

        l.Done();
    }

    private void InternalSave(IContextOfBlock context, Options options)
    {
        var (guid, part, index, entityId, add) = options;
        var isContentPair = ViewParts.ContentLower.EqualsInsensitive(part);
        var l = Log.Fn($"target:{guid}, {nameof(part)}:{part}, {nameof(isContentPair)}: {isContentPair}, {nameof(index)}:{index}, {nameof(entityId)}:{entityId}");

        var entity = context.AppReaderRequired.GetDraftOrPublished(guid)
                     ?? throw l.Done(new Exception($"Can't find item '{guid}'"));

        // Make sure we have the correct casing for the field names
        part = entity.Type[part]!.Name;

        var fList = workFieldList.New(context.AppReaderRequired);

        var forceDraft = context.Publishing.ForceDraft;
        if (add)
        {
            var fields = isContentPair
                ? ViewParts.ContentPair
                : [part];
            var values = isContentPair
                ? [entityId, null]
                : new int?[] { entityId };
            fList.FieldListAdd(entity, fields, index, values, forceDraft, false);
        }
        else
            fList.FieldListReplaceIfModified(entity, [part], index, [entityId], forceDraft);
        l.Done();
    }
}