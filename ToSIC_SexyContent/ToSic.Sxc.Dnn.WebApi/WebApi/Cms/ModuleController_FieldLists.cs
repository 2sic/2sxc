using System;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ModuleController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? index = null)
        {
            Log.Add($"add order:{index}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args) =>
                CmsManager.Blocks.AddEmptyItem(BlockBuilder.Block.Configuration, index);

            // use dnn versioning - this is always part of page
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void ChangeOrder(Guid? parent, string fields, int index, int toIndex)
        {
            var wrapLog = Log.Call($"change order sort:{index}, dest:{toIndex}");
            var entMan = CmsManager.Entities;
            FindTargetAndModifyList(parent, fields, index,
                (entity, fieldList, idx, versioning) => entMan.FieldListMove(entity, fieldList, idx, toIndex, versioning));
            wrapLog(null);
        }



        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index)
        {
            Log.Add($"try to publish #{index} on '{part}'");
            if (!new MultiPermissionsApp(BlockBuilder, BlockBuilder.Context, App.AppId, Log).EnsureAll(GrantSets.WritePublished, out var error))
                throw HttpException.PermissionDenied(error);
            return GetEditor().Publish(part, index);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList(Guid? parent, string fields, int index)
        {
            var wrapLog = Log.Call($"remove from index:{index}");
            var entMan = CmsManager.Entities;
            FindTargetAndModifyList(parent, fields, index,
                (entity, fieldList, idx, versioning) => entMan.FieldListRemove(entity, fieldList, idx, versioning));
            wrapLog(null);
        }

        private void FindTargetAndModifyList(Guid? parent, string fields, int index, Action<IEntity, string[], int, bool> action)
        {
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                // find target
                var target = parent == null ? BlockBuilder.Block.Configuration.Entity : App.Data.List.One(parent.Value);
                if (target == null) throw new Exception($"Can't find parent {parent}");
                // determine versioning
                var useVersioning = BlockBuilder.Block.Configuration.VersioningEnabled;
                // check field list (default to content-block fields)
                var fieldList = fields?.Split(',').Select(f => f.Trim()).ToArray() ?? ViewParts.ContentPair;
                action.Invoke(target, fieldList, index, useVersioning);
            }

            // use dnn versioning - items here are always part of list
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
        }
    }
}
