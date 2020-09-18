using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;

namespace ToSic.Sxc.WebApi.FieldList
{
    internal class FieldListBackend: HasLog
    {
        private readonly IPagePublishing _publishing;

        #region constructor / DI

        public FieldListBackend(IPagePublishing publishing) : base("Bck.FldLst")
        {
            _publishing = publishing;
        }

        public FieldListBackend Init(IInstanceContext context, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _block = block;
            _context = context;
            _cmsManager = new CmsManager(_block.App, Log);
            return this;
        }
        private IInstanceContext _context;
        private IBlock _block;
        private CmsManager _cmsManager;
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
            if (!new MultiPermissionsApp().Init(_context, _block.App, Log)
                .EnsureAll(GrantSets.WritePublished, out var error))
                throw HttpException.PermissionDenied(error);
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
