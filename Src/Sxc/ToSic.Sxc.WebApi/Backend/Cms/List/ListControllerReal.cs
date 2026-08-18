using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListControllerReal(
    GenWorkDb<WorkFieldList> workFieldList,
    ISxcCurrentContextService ctxService,
    LazySvc<AppWorkContextService> appCtxSvc,
    Generator<IPagePublishing> publishing,
    LazySvc<ListActivityReplace> actReplace,
    Generator<ListActivityReplaceOptions, IAppWorkCtxForDiWip> actReplaceOptions,
    LazySvc<ListActivityGetItems> actGetItems,
    LazySvc<ListActivitySave> actSave,
    Generator<ListActivityGetBlockHeader, IAppWorkCtxForDiWip> actGetBlockHeader
        )
    : ServiceBase("Api.LstRl", connect: [workFieldList, publishing, ctxService, appCtxSvc, actReplace, actReplaceOptions, actGetItems, actSave, actGetBlockHeader]),
        IListController
{
    public const string LogSuffix = "Lst";

    private IAppWorkCtxForDiWip GetCtx() => appCtxSvc.Value.ContextNew(ctxService.BlockContextRequired().AppReaderRequired);

    public void Replace(Guid parent, string part, int index, int entityId, bool add = false)
        => actReplace.Value.Replace(new(parent, part, index, entityId, add));

    public ReplacementListDto ReplaceOptions(Guid parent, string part, int index, string? contentType = null)
        => actReplaceOptions.New(GetCtx()).ReplaceOptions(new(parent, part, index, contentType));

    public List<EntityInListDto> Items(Guid parent, string part)
        => actGetItems.Value.ItemList(parent, part);
    
    public bool Items(Guid parent, List<EntityInListDto> list, string part)
        => actSave.Value.ItemList(parent, list, part);

    public List<EntityInListDto> ContentBlockHeader(Guid parent)
        => actGetBlockHeader.New(GetCtx()).ContentBlockHeader(parent);


    public void Move(Guid? parent, string fields, int index, int toIndex) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}, toIndex:{toIndex}");
        var fList = workFieldList.New(ctxService.BlockContextRequired().AppReaderRequired);
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.FieldListMove(entity, fieldList, index, toIndex, versioning));
        l.Done();
    }


    public void Delete(Guid? parent, string part, int index) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{part}, index:{index}");
        var fList = workFieldList.New(ctxService.BlockContextRequired().AppReaderRequired);
        ModifyList(FindOrThrow(parent), part,
            (entity, fieldList, versioning) => fList.FieldListRemove(entity, fieldList, index, versioning));
        l.Done();
    }

    private void ModifyList(IEntity target, string fields, Action<IEntity, string[], bool> action)
    {
        // use dnn versioning - items here are always part of list
        var context = ctxService.BlockContextRequired();
        publishing.New().DoInsidePublishing(context, _ =>
        {
            // determine versioning
            var forceDraft = context.Publishing.ForceDraft;
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