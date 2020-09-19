using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;

namespace ToSic.Sxc.WebApi.FieldList
{
    internal class FieldListBackend: BlockWebApiBackendBase<FieldListBackend>
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public FieldListBackend(IPagePublishing publishing) : base("Bck.FldLst") 
            => _publishing = publishing.Init(Log);

        #endregion

        public void ChangeOrder(Guid? parent, string fields, int index, int toIndex)
        {
            var wrapLog = Log.Call($"change order sort:{index}, dest:{toIndex}");
            var entMan = _cmsManager.Entities;
            FindTargetAndModifyList(parent, fields, index,
                (entity, fieldList, idx, versioning) => entMan.FieldListMove(entity, fieldList, idx, toIndex, versioning));
            wrapLog(null);
        }

        public bool Publish(string part, int index)
        {
            Log.Add($"try to publish #{index} on '{part}'");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            return BlockEditorBase.GetEditor(_block).Publish(part, index);
        }

        public void Remove(Guid? parent, string fields, int index)
        {
            var wrapLog = Log.Call($"remove from index:{index}");
            var entMan = _cmsManager.Entities;
            FindTargetAndModifyList(parent, fields, index,
                (entity, fieldList, idx, versioning) => entMan.FieldListRemove(entity, fieldList, idx, versioning));
            wrapLog(null);
        }


        private void FindTargetAndModifyList(Guid? parent, string fields, int index, Action<IEntity, string[], int, bool> action)
        {
            // use dnn versioning - items here are always part of list
            _publishing.DoInsidePublishing(_context, args =>
            {
                // find target
                var target = parent == null ? _block.Configuration.Entity : _block.App.Data.List.One(parent.Value);
                if (target == null) throw new Exception($"Can't find parent {parent}");
                // determine versioning
                var useVersioning = _block.Configuration.VersioningEnabled;
                // check field list (default to content-block fields)
                var fieldList = fields?.Split(',').Select(f => f.Trim()).ToArray() ?? ViewParts.ContentPair;
                action.Invoke(target, fieldList, index, useVersioning);
            });
        }

    }
}
