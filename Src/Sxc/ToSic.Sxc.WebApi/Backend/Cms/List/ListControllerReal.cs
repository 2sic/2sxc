using ToSic.Eav.Apps.State;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Cms;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Internal.Publishing;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class ListControllerReal(
    Generator<MultiPermissionsApp> multiPermissionsApp,
    GenWorkPlus<WorkEntities> workEntities,
    GenWorkDb<WorkFieldList> workFieldList,
    ISxcContextResolver ctxResolver,
    AppWorkContextService appWorkCtxService,
    Generator<IPagePublishing> publishing)
    : BlockWebApiBackendBase(multiPermissionsApp, appWorkCtxService, ctxResolver, "Api.LstRl",
        connect: [workFieldList, workEntities, publishing]), IListController
{
    public const string LogSuffix = "Lst";

    private IContextOfBlock Context => _context ??= CtxResolver.BlockContextRequired();
    private IContextOfBlock _context;


    public void Move(Guid? parent, string fields, int index, int toIndex) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}, toIndex:{toIndex}");
        var fList = workFieldList.New(Context.AppReader);
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.FieldListMove(entity, fieldList, index, toIndex, versioning));
        l.Done();
    }


    public void Delete(Guid? parent, string fields, int index) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}");
        var fList = workFieldList.New(Context.AppReader);
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.FieldListRemove(entity, fieldList, index, versioning));
        l.Done();
    }




    private void ModifyList(IEntity target, string fields, Action<IEntity, string[], bool> action)
    {
        // use dnn versioning - items here are always part of list
        publishing.New().DoInsidePublishing(ContextOfBlock, args =>
        {
            // determine versioning
            var forceDraft = (ContextOfBlock as IContextOfBlock)?.Publishing.ForceDraft ?? false;
            // check field list (default to content-block fields)
            var fieldList = fields is null or ViewParts.ContentLower
                ? ViewParts.ContentPair
                : fields.CsvToArrayWithoutEmpty();
            action.Invoke(target, fieldList, forceDraft);
        });
    }

    private IEntity FindOrThrow(Guid? parent)
    {
        var target = parent == null
            ? CtxResolver.BlockRequired().Configuration.Entity
            : ContextOfBlock.AppReader.List.One(parent.Value);

        return target == null
            ? throw new($"Can't find parent {parent}")
            : ContextOfBlock.AppReader.GetDraftOrKeep(target);
    }
}