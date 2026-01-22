using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class ListControllerReal(
    GenWorkDb<WorkFieldList> workFieldList,
    GenWorkPlus<WorkEntities> workEntities,
    ISxcCurrentContextService ctxService,
    Generator<IPagePublishing> publishing)
    : ServiceBase("Api.LstRl",
        connect: [workFieldList, workEntities, publishing, ctxService]), IListController
{
    public const string LogSuffix = "Lst";

    private IContextOfBlock Context => field ??= ctxService.BlockContextRequired();


    public void Move(Guid? parent, string fields, int index, int toIndex) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}, toIndex:{toIndex}");
        var fList = workFieldList.New(Context.AppReaderRequired);
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.FieldListMove(entity, fieldList, index, toIndex, versioning));
        l.Done();
    }


    public void Delete(Guid? parent, string fields, int index) 
    {
        var l = Log.Fn($"parent:{parent}, fields:{fields}, index:{index}");
        var fList = workFieldList.New(Context.AppReaderRequired);
        ModifyList(FindOrThrow(parent), fields,
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
            : block.Context.AppReaderRequired.List.One(parent.Value);

        return target == null
            ? throw new($"Can't find parent {parent}")
            : block.Context.AppReaderRequired.GetDraftOrKeep(target);
    }
}