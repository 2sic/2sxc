using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Cms;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Eav.Apps.Work;

namespace ToSic.Sxc.WebApi.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class ListControllerReal: BlockWebApiBackendBase, IHasLog, IListController
{
    public const string LogSuffix = "Lst";

    #region constructor / DI

    public ListControllerReal(
        Generator<MultiPermissionsApp> multiPermissionsApp,
        GenWorkPlus<WorkEntities> workEntities,
        GenWorkDb<WorkFieldList> workFieldList,
        IPagePublishing publishing,
        IContextResolver ctxResolver,
        AppWorkContextService appWorkCtxService,
        Generator<IPagePublishing> versioning
    ) : base(multiPermissionsApp, appWorkCtxService, ctxResolver, "Api.LstRl")
    {
        ConnectServices(
            _workFieldList = workFieldList,
            _workEntities = workEntities,
            _publishing = publishing,
            _versioning = versioning
        );
    }



    private readonly GenWorkDb<WorkFieldList> _workFieldList;
    private readonly GenWorkPlus<WorkEntities> _workEntities;
    private readonly Generator<IPagePublishing> _versioning;
    private readonly IPagePublishing _publishing;

    private IContextOfBlock Context => _context ??= CtxResolver.BlockContextRequired();
    private IContextOfBlock _context;


    #endregion


    public void Move(Guid? parent, string fields, int index, int toIndex
    ) => Log.Do($"change order sort:{index}, dest:{toIndex}", () =>
    {
        var fList = _workFieldList.New(Context.AppState);
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.FieldListMove(entity, fieldList, index, toIndex, versioning));
    });


    public void Delete(Guid? parent, string fields, int index) => Log.Do($"remove from index:{index}", () =>
    {
        var fList = _workFieldList.New(Context.AppState);
        ModifyList(FindOrThrow(parent), fields,
            (entity, fieldList, versioning) => fList.FieldListRemove(entity, fieldList, index, versioning));
    });




    private void ModifyList(IEntity target, string fields, Action<IEntity, string[], bool> action)
    {
        // use dnn versioning - items here are always part of list
        _publishing.DoInsidePublishing(ContextOfBlock, args =>
        {
            // determine versioning
            var forceDraft = (ContextOfBlock as IContextOfBlock)?.Publishing.ForceDraft ?? false;
            // check field list (default to content-block fields)
            var fieldList = fields == null || fields == ViewParts.ContentLower
                ? ViewParts.ContentPair
                : fields.Split(',').Select(f => f.Trim()).ToArray();
            action.Invoke(target, fieldList, forceDraft);
        });
    }

    private IEntity FindOrThrow(Guid? parent)
    {
        var target = parent == null ? CtxResolver.BlockRequired().Configuration.Entity : ContextOfBlock.AppState.List.One(parent.Value); 
        if (target == null) throw new Exception($"Can't find parent {parent}");
        return ContextOfBlock.AppState.GetDraftOrKeep(target);
    }
}