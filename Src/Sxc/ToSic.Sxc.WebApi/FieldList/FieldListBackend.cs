using System;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.FieldList
{
    public class FieldListBackend: BlockWebApiBackendBase<FieldListBackend>
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public FieldListBackend(IServiceProvider sp, IPagePublishing publishing, Lazy<CmsManager> cmsManagerLazy, IContextResolver ctxResolver) : base(sp, cmsManagerLazy, ctxResolver, "Bck.FldLst") 
            => _publishing = publishing.Init(Log);

        #endregion

        public void ChangeOrder(Guid? parent, string fields, int index, int toIndex)
        {
            var wrapLog = Log.Call($"change order sort:{index}, dest:{toIndex}");
            var entMan = CmsManagerOfBlock.Entities;
            ModifyList(FindOrThrow(parent), fields, 
                (entity, fieldList, versioning) => entMan.FieldListMove(entity, fieldList, index, toIndex, versioning));
            wrapLog(null);
        }


        public void Remove(Guid? parent, string fields, int index)
        {
            var wrapLog = Log.Call($"remove from index:{index}");
            var entMan = CmsManagerOfBlock.Entities;
            ModifyList(FindOrThrow(parent), fields, 
                (entity, fieldList, versioning) => entMan.FieldListRemove(entity, fieldList, index, versioning));
            wrapLog(null);
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
