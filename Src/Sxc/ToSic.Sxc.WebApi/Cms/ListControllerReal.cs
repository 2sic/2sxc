using System;
using System.Linq;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Cms;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ListControllerReal: BlockWebApiBackendBase, IHasLog, IListController
    {
        public const string LogSuffix = "Lst";


        #region constructor / DI

        public ListControllerReal(
            Generator<MultiPermissionsApp> multiPermissionsApp,
            IPagePublishing publishing,
            LazySvc<CmsManager> cmsManagerLazy,
            IContextResolver ctxResolver,
            Generator<IPagePublishing> versioning
        ) : base(multiPermissionsApp, cmsManagerLazy, ctxResolver, "Api.LstRl")
            => ConnectServices(
                _publishing = publishing,
                _versioning = versioning
            );

        private readonly Generator<IPagePublishing> _versioning;
        private readonly IPagePublishing _publishing;

        private IContextOfBlock Context => _context ?? (_context = CtxResolver.BlockContextRequired());
        private IContextOfBlock _context;


        #endregion


        public void Move(Guid? parent, string fields, int index, int toIndex
        ) => Log.Do($"change order sort:{index}, dest:{toIndex}", () =>
        {
            var entMan = CmsManagerOfBlock.Entities;
            ModifyList(FindOrThrow(parent), fields,
                (entity, fieldList, versioning) => entMan.FieldListMove(entity, fieldList, index, toIndex, versioning));
        });


        public void Delete(Guid? parent, string fields, int index) => Log.Do($"remove from index:{index}", () =>
        {
            var entMan = CmsManagerOfBlock.Entities;
            ModifyList(FindOrThrow(parent), fields,
                (entity, fieldList, versioning) => entMan.FieldListRemove(entity, fieldList, index, versioning));
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
            return target;
        }
    }
}
