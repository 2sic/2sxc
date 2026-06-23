using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sxc.Cms.Publishing.Sys;
using static System.StringComparison;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentGroupControllerReal(
    GenWorkDb<WorkFieldList> workFieldList,
    GenWorkPlus<WorkBlocks> appBlocks,
    LazySvc<IPagePublishing> publishing,
    ISxcCurrentContextService ctxService
    )
    : ServiceBase("Api.CntGrpRl", connect: [workFieldList, appBlocks, ctxService, publishing]),
        IContentGroupController
{
    #region Constructor / di

    public const string LogSuffix = "CntGrp";

    [field: AllowNull, MaybeNull]
    private IContextOfBlock Context => field ??= ctxService.BlockContextRequired();

    private IAppWorkCtxPlus AppCtx => field ??= appBlocks.CtxSvc.ContextPlus(Context.AppReaderRequired);

    #endregion

    public EntityInListDto? Header(Guid guid)
    {
        var l = Log.Fn<EntityInListDto?>($"header for:{guid}");
        var cg = appBlocks.New(AppCtx).GetBlockConfig(guid);

        // new in v11 - this call might be run on a non-content-block, in which case we return null
        var ent = (cg as ICanBeEntity)?.Entity;
        if (ent == null!)
            return l.ReturnNull("No entity found");
        if (ent.Type.Name != WorkBlocks.BlockTypeName)
            return l.ReturnNull("Entity type mismatch");

        var header = cg.Header.FirstOrDefault();

        return l.Return(new(header, 0)
        {
            Type = header?.Type.NameId ?? cg.View!.HeaderType
        });
    }
        

    //public void Replace(Guid parent, string part, int index, int entityId, bool add = false) 
    //    => listController.Value.Replace(parent, part, index, entityId, add);


    ///// <summary>
    ///// Special Replace just like list-replace, but with content type name coming from View definition
    ///// </summary>
    //public ReplacementListDto? Replace(Guid parent, string part, int index)
    //{
    //    var l = Log.Fn<ReplacementListDto?>($"target:{parent}, part:{part}, index:{index}");
    //    var typeNameOfField = FindTypeNameOnContentGroup(parent, part);
    //    var result = listController.Value.GetListToReorder(parent, part, index, typeNameOfField);
    //    return l.Return(result);
    //}




    public List<EntityInListDto> ItemList(Guid parent, string part)
    {
        var l = Log.Fn<List<EntityInListDto>>($"item list for:{parent}");
        var cg = Context.AppReaderRequired.GetDraftOrPublished(parent)!;
        var itemList = cg.Children(part);
        var list = itemList
            .Select(Context.AppReaderRequired.GetDraftOrKeep)
            .Select((e, index) => new EntityInListDto(e, index))
            .ToList();

        return l.Return(list);
    }


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    public bool ItemList(Guid parent, List<EntityInListDto>? list,  string? part = null)
    {
        var l = Log.Fn<bool>($"list for:{parent}, items:{list?.Count}");
        if (list == null)
            throw l.Done(new ArgumentNullException(nameof(list)));

        publishing.Value.DoInsidePublishing(Context, _ =>
        {
            var entity = Context.AppReaderRequired.GetDraftOrPublished(parent);
            var sequence = list
                .Select(i => i.Index)
                .ToArray();
            var fields = part == ViewParts.ContentLower
                ? ViewParts.ContentPair
                : [part ?? throw new ArgumentException(@"Part name cannot be null", nameof(part))];
            workFieldList.New(Context.AppReaderRequired)
                .FieldListReorder(entity!, fields, sequence, Context.Publishing.ForceDraft);
        });

        return l.ReturnTrue();
    }
}