using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ContentGroupController
    {
        // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Replace(Guid guid, string part, int index, int entityId)
        {
            var wrapLog = Log.Call($"target:{guid}, part:{part}, index:{index}, id:{entityId}");
            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                var cms = new CmsManager(BlockBuilder.App, Log);
                var entity = cms.AppState.List.One(guid);
                if (entity == null) throw new Exception($"Can't find item '{guid}'");

                // correct casing of content / listcontent for now - TODO should already happen in JS-Call
                if (entity.Type.StaticName == BlocksRuntime.BlockTypeName)
                {
                    if (string.Equals(part, ViewParts.Content, OrdinalIgnoreCase)) part = ViewParts.Content;
                    if (string.Equals(part, ViewParts.ListContent, OrdinalIgnoreCase)) part = ViewParts.ListContent;
                }

                cms.Entities.FieldListReplaceIfModified(entity, new[] { part }, index, new int?[] { entityId }, cms.Read.WithPublishing);
            }

            // use dnn versioning - this is always part of page
            var context = DnnDynamicCode.Create(BlockBuilder, Log);
            versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, InternalSave);
            wrapLog(null);
        }
        // TODO: WIP changing this from ContentGroup editing to any list editing
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic Replace(Guid guid, string part, int index)
        {
            var wrapLog = Log.Call<dynamic>($"target:{guid}, part:{part}, index:{index}");
            part = part.ToLower();

            var itemList = FindContentGroupAndTypeName(guid, part, out var attributeSetName)
                           ?? FindItemAndFieldTypeName(guid, part, out attributeSetName);

            // if no type was defined in this set, then return an empty list as there is nothing to choose from
            if (string.IsNullOrEmpty(attributeSetName))
                return null;

            var appState = Eav.Apps.State.Get(BlockBuilder.App);
            var ct = appState.GetContentType(attributeSetName);

            var dataSource = BlockBuilder.App.Data[ct.Name];
            var results = dataSource.List.ToDictionary(p => p.EntityId,
                p => p.GetBestTitle() ?? "");

            var selectedId = itemList.Count == 0 ? null : itemList[index]?.EntityId;

            var result = new
            {
                SelectedId = selectedId,
                Items = results,
                ContentTypeName = ct.StaticName
            };
            return wrapLog(null, result);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<SortedEntityItem> ItemList(Guid guid, string part)
        {
            Log.Add($"item list for:{guid}");
            var cg = BlockBuilder.App.Data.List.One(guid);
            var itemList = cg.Children(part);

            var list = itemList.Select((c, index) => new SortedEntityItem
            {
                Index = index,
                Id = c?.EntityId ?? 0,
                Guid = c?.EntityGuid ?? Guid.Empty,
                Title = c?.GetBestTitle() ?? "",
                //Type = c?.Type.StaticName ?? cg.View.ContentType
            }).ToList();

            return list;
        }


        // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool ItemList([FromUri] Guid guid, List<SortedEntityItem> list, [FromUri] string part = null)
        {
            Log.Add($"list for:{guid}, items:{list?.Count}");
            if (list == null) throw new ArgumentNullException(nameof(list));

            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                var cms = new CmsManager(BlockBuilder.App, Log);
                var entity = cms.Read.AppState.List.One(guid);

                var sequence = list.Select(i => i.Index).ToArray();
                var fields = part == ViewParts.ContentLower ? ViewParts.ContentPair : new[] {part};
                cms.Entities.FieldListReorder(entity, fields, sequence, cms.EnablePublishing);
            }

            // use dnn versioning - items here are always part of list
            var context = DnnDynamicCode.Create(BlockBuilder, Log);
            versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, InternalSave);

            return true;
        }
    }
}
