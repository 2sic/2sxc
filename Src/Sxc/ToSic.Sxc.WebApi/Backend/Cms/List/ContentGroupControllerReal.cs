using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Context.Sys;
using static System.StringComparison;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentGroupControllerReal(
    GenWorkDb<WorkFieldList> workFieldList,
    GenWorkPlus<WorkBlocks> appBlocks,
    LazySvc<IPagePublishing> publishing,
    ISxcCurrentContextService ctxService,
    LazySvc<ListControllerReal> listController)
    : ServiceBase("Api.CntGrpRl", connect: [workFieldList, appBlocks, ctxService, publishing, listController]),
        IContentGroupController
{
    #region Constructor / di

    public const string LogSuffix = "CntGrp";


    public ISxcCurrentContextService CtxService { get; } = ctxService;


    [field: AllowNull, MaybeNull]
    private IContextOfBlock Context => field ??= CtxService.BlockContextRequired();

    private IAppWorkCtxPlus AppCtx => _appCtx.Get(() => appBlocks.CtxSvc.ContextPlus(Context.AppReaderRequired))!;
    private readonly GetOnce<IAppWorkCtxPlus> _appCtx = new();
    #endregion

    public EntityInListDto? Header(Guid guid)
    {
        Log.A($"header for:{guid}");
        //var cg = CmsManager.Read.Blocks.GetBlockConfig(guid);
        var cg = appBlocks.New(AppCtx).GetBlockConfig(guid);

        // new in v11 - this call might be run on a non-content-block, in which case we return null
        if (cg.Entity == null!)
            return null;
        if (cg.Entity.Type.Name != WorkBlocks.BlockTypeName)
            return null;

        var header = cg.Header.FirstOrDefault();

        return new()
        {
            Index = 0,
            Id = header?.EntityId ?? 0,
            Guid = header?.EntityGuid ?? Guid.Empty,
            Title = header?.GetBestTitle() ?? "",
            Type = header?.Type.NameId ?? cg.View!.HeaderType
        };
    }
        

    public void Replace(Guid guid, string part, int index, int entityId, bool add = false) 
        => listController.Value.Replace(guid, part, index, entityId, add);


    /// <summary>
    /// Special Replace just like list-replace, but with content type name coming from View definition
    /// </summary>
    public ReplacementListDto? Replace(Guid guid, string part, int index)
    {
        var l = Log.Fn<ReplacementListDto?>($"target:{guid}, part:{part}, index:{index}");
        var typeNameOfField = FindTypeNameOnContentGroup(guid, part);
        var result = listController.Value.GetListToReorder(guid, part, index, typeNameOfField);
        return l.Return(result);
    }


    private string? FindTypeNameOnContentGroup(Guid guid, string part)
    {
        var l = Log.Fn<string>($"{guid}, {part}");

        //var contentGroup = CmsManager.Read.Blocks.GetBlockConfig(guid);
        var contentGroup = appBlocks.New(AppCtx).GetBlockConfig(guid);
        if (contentGroup?.Entity == null || contentGroup.View == null)
            return l.ReturnNull("Doesn't seem to be a content-group. Cancel.");

        var typeNameForField = string.Equals(part, ViewParts.ContentLower, OrdinalIgnoreCase)
            ? contentGroup.View.ContentType
            : contentGroup.View.HeaderType;

        return l.Return(typeNameForField);
    }




    public List<EntityInListDto> ItemList(Guid guid, string part)
    {
        Log.A($"item list for:{guid}");
        var cg = Context.AppReaderRequired.GetDraftOrPublished(guid)!;
        var itemList = cg.Children(part);
        var list = itemList
            .Select(Context.AppReaderRequired.GetDraftOrKeep)
            .Select((c, index) => new EntityInListDto
            {
                Index = index,
                Id = c?.EntityId ?? 0,
                Guid = c?.EntityGuid ?? Guid.Empty,
                Title = c?.GetBestTitle() ?? "",
                Type = c?.Type.NameId,
                TypeWip = c?.Type.NameId == null ? null : new JsonType(c)
            })
            .ToList();

        return list;
    }


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    public bool ItemList(Guid guid, List<EntityInListDto>? list,  string? part = null)
    {
        Log.A($"list for:{guid}, items:{list?.Count}");
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        publishing.Value.DoInsidePublishing(Context, _ =>
        {
            var entity = Context.AppReaderRequired.GetDraftOrPublished(guid);
            var sequence = list
                .Select(i => i.Index)
                .ToArray();
            var fields = part == ViewParts.ContentLower
                ? ViewParts.ContentPair
                : [part ?? throw new ArgumentException(@"Part name cannot be null", nameof(part))];
            workFieldList.New(Context.AppReaderRequired)
                .FieldListReorder(entity!, fields, sequence, Context.Publishing.ForceDraft);
        });

        return true;
    }


}