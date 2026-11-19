using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Publishing.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityReplace(AppWorkChain<WorkFieldList> workFieldList, Generator<IDataPublishingService> publishing)
    : ServiceWithSetup<IAppWorkContext>("Act.LsRpOp", connect: [workFieldList, publishing])
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
        publishing.New().DoInsidePublishing(opts => InternalSave(options, opts.ForceDraft));

        l.Done();
    }

    private bool InternalSave(Options options, bool forceDraft)
    {
        var (guid, part, index, entityId, add) = options;
        var isContentPair = ViewParts.ContentLower.EqualsInsensitive(part);
        var l = Log.Fn<bool>($"target:{guid}, {nameof(part)}:{part}, {nameof(isContentPair)}: {isContentPair}, {nameof(index)}:{index}, {nameof(entityId)}:{entityId}");

        var entity = MyOptions.AppReader.GetDraftOrPublished(guid)
                     ?? throw l.Done(new Exception($"Can't find item '{guid}'"));

        // Make sure we have the correct casing for the field names
        part = entity.Type[part]!.Name;

        var fList = workFieldList.New(MyOptions);

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

        return l.ReturnTrue();
    }
}