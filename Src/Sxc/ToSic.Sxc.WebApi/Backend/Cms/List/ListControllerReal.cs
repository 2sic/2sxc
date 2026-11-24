using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.ListSlots.Sys;
using ToSic.Eav.Data.Publishing.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListControllerReal(
    IContextCompleteService ctxService,
    LazySvc<AppWorkContextService> appCtxSvc,
    Generator<IDataPublishingService> publishing,
    AppWorkChain<WorkListSlots> workFieldList,
    AppWorkChain<ListActivityReplace> actReplace,
    AppDataChain<ListActivityReplaceOptions> actReplaceOptions,
    AppDataChain<ListActivityGetItems> actGetItems,
    AppWorkChain<ListActivitySave> actSave,
    AppDataChain<ListActivityGetBlockHeader> actGetBlockHeader
        )
    : ServiceBase("Api.LstRl", connect: [workFieldList, publishing, ctxService, appCtxSvc, actReplace, actReplaceOptions, actGetItems, actSave, actGetBlockHeader]),
        IListController
{
    public const string LogSuffix = "Lst";

    private IAppWorkContext GetCtx() => appCtxSvc.Value.ContextNew(ctxService.BlockContextRequired().AppReaderRequired);

    public void Replace(Guid parent, string part, int index, int entityId, bool add = false)
        => actReplace.New(GetCtx()).Replace(new(parent, ChangePartNameToFields(part), index, entityId, add));

    public ListSlotSwapInfo ReplaceOptions(Guid parent, string part, int index, string? contentType = null)
        => actReplaceOptions.New(GetCtx()).ReplaceOptions(new(parent, part, index, contentType));

    public List<ListSlotContentsInfo> Items(Guid parent, string part)
        => actGetItems.New(GetCtx()).ItemList(parent, part);

    public bool Items(Guid parent, List<ListSlotContentsInfo> list, string part)
        => actSave.New(GetCtx()).ItemList(parent, list, ChangePartNameToFields(part));

    public List<ListSlotContentsInfo> ContentBlockHeader(Guid parent)
        => actGetBlockHeader.New(GetCtx()).ContentBlockHeader(parent);

    /// <summary>
    /// 2026-11-19 2dm This is a leftover of old times, not sure if it's still needed.
    /// Previously web requests simply asked for the part (Content, Header, etc.)
    /// and the backend had to figure out which fields.
    /// 
    /// This is because a long time ago, lists were explicitly used for content blocks.
    /// It's not sure if the UI already lists the exact names, in which case this would be obsolete.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private string[] ChangePartNameToFields(string part)
    {
        var isContentPair = ViewParts.ContentLower.EqualsInsensitive(part);
        var l = Log.Fn<string[]>($"part:{part}");
        var fields = isContentPair
            ? ViewParts.ContentPair
            : [part ?? throw new ArgumentException(@"Part name cannot be null", nameof(part))];
        return l.Return(fields);
    }

    public void Move(Guid? parent, string fields, int index, int toIndex) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}, toIndex:{toIndex}");
        var fList = workFieldList.New(GetCtx());
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.MoveSlot(entity, fieldList, index, toIndex, versioning));
        l.Done();
    }


    public void Delete(Guid? parent, string fields, int index)
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}");
        var fList = workFieldList.New(GetCtx());
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.RemoveSlot(entity, fieldList, index, versioning));
        l.Done();
    }

    private void ModifyList(IEntity target, string fields, Action<IEntity, string[], bool> action)
    {
        // use dnn versioning - items here are always part of list
        publishing.New().DoInsidePublishing(options =>
        {
            // determine versioning
            var forceDraft = options.ForceDraft;
            // check field list (default to content-block fields)
            var fieldList = fields is null or ViewParts.ContentLower
                ? ViewParts.ContentPair
                : fields.CsvToArrayWithoutEmpty();
            action.Invoke(target, fieldList, forceDraft);
        });
    }

    private IEntity FindOrThrow(Guid? parent)
    {
        var block = ctxService.BlockRequired();
        var target = parent == null
            ? (block.Configuration as ICanBeEntity)?.Entity
            : block.Context.AppReaderRequired.List.GetOne(parent.Value);

        return target == null
            ? throw new($"Can't find parent {parent}")
            : block.Context.AppReaderRequired.GetDraftOrKeep(target);
    }
}
