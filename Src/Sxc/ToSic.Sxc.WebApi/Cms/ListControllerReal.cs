using System;
using System.Linq;
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
    public partial class ListControllerReal: BlockWebApiBackendBase<ListControllerReal>, IHasLog, IListController
    {
        public const string LogSuffix = "Lst";


        #region constructor / DI

        public ListControllerReal(
            IServiceProvider sp,
            IPagePublishing publishing,
            LazyInitLog<CmsManager> cmsManagerLazy,
            IContextResolver ctxResolver,
            GeneratorLog<IPagePublishing> versioning
        ) : base(sp, cmsManagerLazy, ctxResolver, "Api.LstRl")
            => ConnectServices(
                _publishing = publishing,
                _versioning = versioning
            );

        private readonly GeneratorLog<IPagePublishing> _versioning;
        private readonly IPagePublishing _publishing;

        private IContextOfBlock Context => _context ?? (_context = CtxResolver.BlockRequired());
        private IContextOfBlock _context;


        #endregion


        public void Move(Guid? parent, string fields, int index, int toIndex)
        {
            var wrapLog = Log.Fn($"change order sort:{index}, dest:{toIndex}");
            var entMan = CmsManagerOfBlock.Entities;
            ModifyList(FindOrThrow(parent), fields, 
                (entity, fieldList, versioning) => entMan.FieldListMove(entity, fieldList, index, toIndex, versioning));
            wrapLog.Done();
        }
        

        public void Delete(Guid? parent, string fields, int index)
        {
            var wrapLog = Log.Fn($"remove from index:{index}");
            var entMan = CmsManagerOfBlock.Entities;
            ModifyList(FindOrThrow(parent), fields, 
                (entity, fieldList, versioning) => entMan.FieldListRemove(entity, fieldList, index, versioning));
            wrapLog.Done();
        }




        private void ModifyList(IEntity target, string fields, Action<IEntity, string[], bool> action)
        {
            // use dnn versioning - items here are always part of list
            _publishing.DoInsidePublishing(ContextOfBlock, args =>
            {
                // determine versioning
                var forceDraft = (ContextOfBlock as IContextOfBlock)?.Publishing.ForceDraft ?? false;
                // check field list (default to content-block fields)
                var fieldList = fields?.Split(',').Select(f => f.Trim()).ToArray() ?? ViewParts.ContentPair;
                action.Invoke(target, fieldList, forceDraft);
            });
        }

        private IEntity FindOrThrow(Guid? parent)
        {
            var target = parent == null ? CtxResolver.RealBlockRequired().Configuration.Entity : ContextOfBlock.AppState.List.One(parent.Value);
            if (target == null) throw new Exception($"Can't find parent {parent}");
            return target;
        }
    }
}
