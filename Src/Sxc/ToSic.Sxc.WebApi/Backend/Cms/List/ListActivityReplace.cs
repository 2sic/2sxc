using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.ListSlots.Sys;
using ToSic.Eav.Data.Publishing.Sys;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityReplace(AppWorkChain<WorkListSlots> workFieldList, Generator<IDataPublishingService> publishing)
    : ServiceWithSetup<IAppWorkContext>("Act.LsRpOp", connect: [workFieldList, publishing])
{
    public record Options(
        Guid Guid,
        string[] Fields,
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
        var (guid, fields, index, entityId, add) = options;
        var l = Log.Fn<bool>($"target:{guid}, {nameof(index)}:{index}, {nameof(entityId)}:{entityId}");

        var entity = MyOptions.AppReader.GetDraftOrPublished(guid)
                     ?? throw l.Done(new Exception($"Can't find item '{guid}'"));

        // Make sure we have the correct casing for the field names
        fields = [.. fields.Select(f => entity.Type[f]!.Name)];

        var fList = workFieldList.New(MyOptions);

        if (add)
        {
            var values = fields.Length > 1
                ? [entityId, null]
                : new int?[] { entityId };
            fList.AddSlot(entity, fields, index, values, forceDraft, false);
        }
        else
            fList.ReplaceSlot(entity, [fields.First()], index, [entityId], forceDraft);

        return l.ReturnTrue();
    }
}