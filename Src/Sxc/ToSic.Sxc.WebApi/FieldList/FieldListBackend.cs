using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.WebApi.FieldList
{
    public class FieldListBackend: BlockWebApiBackendBase<FieldListBackend>
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public FieldListBackend(IPagePublishing publishing, Lazy<CmsManager> cmsManagerLazy) : base(cmsManagerLazy, "Bck.FldLst") 
            => _publishing = publishing.Init(Log);

        #endregion

        public void ChangeOrder(Guid? parent, string fields, int index, int toIndex)
        {
            var wrapLog = Log.Call($"change order sort:{index}, dest:{toIndex}");
            var entMan = CmsManager.Entities;
            ModifyList(FindOrThrow(parent), fields, 
                (entity, fieldList, versioning) => entMan.FieldListMove(entity, fieldList, index, toIndex, versioning));
            wrapLog(null);
        }


        public void Remove(Guid? parent, string fields, int index)
        {
            var wrapLog = Log.Call($"remove from index:{index}");
            var entMan = CmsManager.Entities;
            ModifyList(FindOrThrow(parent), fields, 
                (entity, fieldList, versioning) => entMan.FieldListRemove(entity, fieldList, index, versioning));
            wrapLog(null);
        }


        private void ModifyList(IEntity target, string fields, Action<IEntity, string[], bool> action)
        {
            // use dnn versioning - items here are always part of list
            _publishing.DoInsidePublishing(_context, args =>
            {
                // determine versioning
                var forceDraft = _block.Context.Publishing.ForceDraft; //.Configuration.VersioningEnabled;
                // check field list (default to content-block fields)
                var fieldList = fields?.Split(',').Select(f => f.Trim()).ToArray() ?? ViewParts.ContentPair;
                action.Invoke(target, fieldList, forceDraft);
            });
        }

        private IEntity FindOrThrow(Guid? parent)
        {
            var target = parent == null ? _block.Configuration.Entity : _block.App.Data.Immutable.One(parent.Value);
            if (target == null) throw new Exception($"Can't find parent {parent}");
            return target;
        }
    }
}
