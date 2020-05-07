using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;
using static System.StringComparison;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class ContentGroupController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sCoGr");
        }

        private BlockConfiguration GetContentGroup(Guid contentGroupGuid)
        {
            Log.Add($"get group:{contentGroupGuid}");
            var cms = new CmsRuntime(BlockBuilder.App, Log, true, false);
            var contentGroup = cms.Blocks.GetBlockConfig(contentGroupGuid);

            if (contentGroup == null)
                throw new Exception("BlockConfiguration with Guid " + contentGroupGuid + " does not exist.");
            return contentGroup;
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

        private List<IEntity> FindItemAndFieldTypeName(Guid guid, string part, out string attributeSetName)
        {
            var parent = BlockBuilder.App.Data.List.One(guid);
            if (parent == null) throw new Exception($"No item found for {guid}");
            if (!parent.Attributes.ContainsKey(part)) throw new Exception($"Could not find field {part} in item {guid}");
            var itemList = parent.Children(part);
            
            // find attribute-type-name
            var attribute = parent.Type.Attributes.FirstOrDefault(a => string.Equals(a.Name, part, OrdinalIgnoreCase));
            if (attribute == null) throw new Exception($"Attribute definition for '{part}' not found on the item {guid}");
            var itemTypeName = attribute.Metadata.GetBestValue<string>(Eav.Constants.EntityFieldType) ?? "";
            attributeSetName = itemTypeName.Split(',').First().Trim();
            return itemList;
        }

        private List<IEntity> FindContentGroupAndTypeName(Guid guid, string part, out string attributeSetName)
        {
            var wrapLog = Log.Call<List<IEntity>>($"{guid}, {part}");
            var contentGroup = GetContentGroup(guid);
            attributeSetName = null;
            var partIsContent = string.Equals(part, ViewParts.ContentLower, OrdinalIgnoreCase);
            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
            var itemList = partIsContent ? contentGroup.Content : contentGroup.Header;

            if (itemList == null) return wrapLog(null, null);

            // not sure what this check is for, just leaving it in for now (2015-09-19 2dm)
            if (contentGroup.View == null)
            {
                Log.Add("Something found, but doesn't seem to be a content-group. Cancel.");
                return wrapLog(null, null);
                //throw new Exception($"Found Content-Group but has no View: {guid}");
            }

            attributeSetName = partIsContent ? contentGroup.View.ContentType : contentGroup.View.HeaderType;
            return wrapLog(null, itemList);
        }

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
                if(entity == null) throw new Exception($"Can't find item '{guid}'");

                // correct casing of content / listcontent for now - TODO should already happen in JS-Call
                if (entity.Type.StaticName == BlocksRuntime.BlockTypeName)
                {
                    if (string.Equals(part, ViewParts.Content, OrdinalIgnoreCase)) part = ViewParts.Content;
                    if (string.Equals(part, ViewParts.ListContent, OrdinalIgnoreCase)) part = ViewParts.ListContent;
                }

                cms.Entities.FieldListReplaceIfModified(entity, new[] { part }, index, new int? [] {entityId}, cms.Read.WithPublishing);
            }

            // use dnn versioning - this is always part of page
            var context = DnnDynamicCode.Create(BlockBuilder, Log);
            versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, InternalSave);
            wrapLog(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<SortedEntityItem> ItemList(Guid guid)
        {
            Log.Add($"item list for:{guid}");
            var cg = GetContentGroup(guid);

            var list = cg.Content.Select((c, index) => new SortedEntityItem
            {
                Index = index,
                Id = c?.EntityId ?? 0,
                Guid = c?.EntityGuid ?? Guid.Empty,
                Title = c?.GetBestTitle() ?? "",
                Type = c?.Type.StaticName ?? cg.View.ContentType
            }).ToList();

            return list;
        }
         

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool ItemList([FromUri] Guid guid, List<SortedEntityItem> list)
        {
            Log.Add($"list for:{guid}, items:{list?.Count}");
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var versioning = BlockBuilder.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                var cg = GetContentGroup(guid);

                var sequence = list.Select(i => i.Index).ToArray();
                new CmsManager(BlockBuilder.App, Log).Blocks
                    .ReorderAllAndSave(cg, sequence);
            }

            // use dnn versioning - items here are always part of list
            var context = DnnDynamicCode.Create(BlockBuilder, Log);
            versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, InternalSave);

            return true;

        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public SortedEntityItem Header(Guid guid)
        {
            Log.Add($"header for:{guid}");
            var cg = GetContentGroup(guid);

            var header = cg.Header.FirstOrDefault();

            return new SortedEntityItem
            {
                Index = 0,
                Id = header?.EntityId ?? 0,
                Guid = header?.EntityGuid ?? Guid.Empty,
                Title = header?.GetBestTitle() ?? "",
                Type = header?.Type.StaticName?? cg.View.HeaderType
            };
        }


        #region helper classes for data transport / json interface

        public class SortedEntityItem
        {
            public int Index;
            public int Id;
            public Guid Guid;
            public string Title;
            public string Type;
        }

        #endregion
    }
}