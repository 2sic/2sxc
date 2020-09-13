using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Cms
{
    // TODO: these methods were once for ContentGroups only, now they work on every entity
    // Some day they should be moved to an own controller or to EntitiesController
    public partial class ContentGroupController
    {
        // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
        {
            var wrapLog = Log.Call($"target:{guid}, part:{part}, index:{index}, id:{entityId}");
            var versioning = Factory.Resolve<IPagePublishing>().Init(Log);

            void InternalSave(VersioningActionInfo args)
            {
                var cms = new CmsManager(GetBlock().App, Log);
                var entity = cms.AppState.List.One(guid);
                if (entity == null) throw new Exception($"Can't find item '{guid}'");

                // correct casing of content / listcontent for now - TODO should already happen in JS-Call
                if (entity.Type.Name == BlocksRuntime.BlockTypeName)
                {
                    if (string.Equals(part, ViewParts.Content, OrdinalIgnoreCase)) part = ViewParts.Content;
                    if (string.Equals(part, ViewParts.ListContent, OrdinalIgnoreCase)) part = ViewParts.ListContent;
                }

                if (add)
                    cms.Entities.FieldListAdd(entity, new[] {part}, index, new int?[] {entityId}, cms.EnablePublishing);
                else
                    cms.Entities.FieldListReplaceIfModified(entity, new[] {part}, index, new int?[] {entityId},
                        cms.EnablePublishing);
            }

            // use dnn versioning - this is always part of page
            var block = GetBlock();
            var dnnDynCode = new DnnDynamicCode().Init(block, Log);
            versioning.DoInsidePublishing(block.Context, InternalSave);
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

            var blockApp = GetBlock().App;
            var appState = State.Get(blockApp);
            var ct = appState.GetContentType(attributeSetName);

            var dataSource = blockApp.Data[ct.Name];
            var results = dataSource.List.ToDictionary(p => p.EntityId,
                p => p.GetBestTitle() ?? "");

            // if list is empty or shorter than index (would happen in an add-to-end-request) return null
            var selectedId = itemList.Count > index 
                ? itemList[index]?.EntityId
                : null;

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
            var cg = GetBlock().App.Data.List.One(guid);
            var itemList = cg.Children(part);

            var list = itemList.Select((c, index) => new SortedEntityItem
            {
                Index = index,
                Id = c?.EntityId ?? 0,
                Guid = c?.EntityGuid ?? Guid.Empty,
                Title = c?.GetBestTitle() ?? "",
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

            var versioning = Factory.Resolve<IPagePublishing>().Init(Log);

            var block = GetBlock();
            void InternalSave(VersioningActionInfo args)
            {
                var cms = new CmsManager(block.App, Log);
                var entity = cms.Read.AppState.List.One(guid);

                var sequence = list.Select(i => i.Index).ToArray();
                var fields = part == ViewParts.ContentLower ? ViewParts.ContentPair : new[] {part};
                cms.Entities.FieldListReorder(entity, fields, sequence, cms.EnablePublishing);
            }

            // use dnn versioning - items here are always part of list
            var dnnDynCode = new DnnDynamicCode().Init(block, Log);
            versioning.DoInsidePublishing(block.Context, InternalSave);

            return true;
        }
    }
}
