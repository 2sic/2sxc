using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.WebApi.FieldList;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ModuleController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? index = null)
        {
            Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log).AddItem(index);
            //Log.Add($"add order:{index}");
            //var versioning = Factory.Resolve<IPagePublishing>().Init(Log);

            //void InternalSave(VersioningActionInfo args) =>
            //    CmsManager.Blocks.AddEmptyItem(GetBlock().Configuration, index);

            //// use dnn versioning - this is always part of page
            //versioning.DoInsidePublishing(GetContext(), InternalSave);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void ChangeOrder(Guid? parent, string fields, int index, int toIndex)
        {
            Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log).ChangeOrder(parent, fields, index, toIndex);
            //var wrapLog = Log.Call($"change order sort:{index}, dest:{toIndex}");
            //var entMan = CmsManager.Entities;
            //FindTargetAndModifyList(parent, fields, index,
            //    (entity, fieldList, idx, versioning) => entMan.FieldListMove(entity, fieldList, idx, toIndex, versioning));
            //wrapLog(null);
        }



        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index)
        {
            return Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log).Publish(part, index);

            //Log.Add($"try to publish #{index} on '{part}'");
            //if (!new MultiPermissionsApp().Init(GetContext(), App, Log)
            //    .EnsureAll(GrantSets.WritePublished, out var error))
            //    throw HttpException.PermissionDenied(error);
            //return GetEditor().Publish(part, index);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList(Guid? parent, string fields, int index)
        {
            Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log)
                .RemoveFromList(parent, fields, index);
            //var wrapLog = Log.Call($"remove from index:{index}");
            //var entMan = CmsManager.Entities;
            //FindTargetAndModifyList(parent, fields, index,
            //    (entity, fieldList, idx, versioning) => entMan.FieldListRemove(entity, fieldList, idx, versioning));
            //wrapLog(null);
        }

        //private void FindTargetAndModifyList(Guid? parent, string fields, int index, Action<IEntity, string[], int, bool> action)
        //{

        //    void InternalSave(VersioningActionInfo args)
        //    {
        //        var block = GetBlock();
        //        // find target
        //        var target = parent == null ? block.Configuration.Entity : App.Data.List.One(parent.Value);
        //        if (target == null) throw new Exception($"Can't find parent {parent}");
        //        // determine versioning
        //        var useVersioning = block.Configuration.VersioningEnabled;
        //        // check field list (default to content-block fields)
        //        var fieldList = fields?.Split(',').Select(f => f.Trim()).ToArray() ?? ViewParts.ContentPair;
        //        action.Invoke(target, fieldList, index, useVersioning);
        //    }

        //    // use dnn versioning - items here are always part of list
        //    var publishing = Factory.Resolve<IPagePublishing>().Init(Log);
        //    publishing.DoInsidePublishing(GetContext(), InternalSave);
        //}
    }
}
