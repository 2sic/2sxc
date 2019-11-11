using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Environment;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.SexyContent.WebApi
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
            var cms = new CmsRuntime(CmsBlock.App, Log, true, false);
            var contentGroup = cms.Blocks.GetBlockConfig(contentGroupGuid);

            if (contentGroup == null)
                throw new Exception("BlockConfiguration with Guid " + contentGroupGuid + " does not exist.");
            return contentGroup;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic Replace(Guid guid, string part, int index)
        {
            Log.Add($"replace target:{guid}, part:{part}, index:{index}");
            part = part.ToLower();
            var contentGroup = GetContentGroup(guid);

            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
            var set = part == ViewParts.ContentLower ? contentGroup.Content : contentGroup.ListContent;

            // not sure what this check is for, just leaving it in for now (2015-09-19 2dm)
            if (set == null || contentGroup.View == null)
                throw new Exception("Cannot find content group");


            var attributeSetName = part == ViewParts.ContentLower
                ? contentGroup.View.ContentType
                : contentGroup.View.HeaderType;

            // if no type was defined in this set, then return an empty list as there is nothing to choose from
            if (string.IsNullOrEmpty(attributeSetName))
                return null;

            var context = GetContext(CmsBlock, Log);

            var cache = context.App.Data.Cache; 
            var ct = cache.GetContentType(attributeSetName);

            var dataSource = context.App.Data[ct.Name]; 
            var results = dataSource.List.ToDictionary(p => p.EntityId,
                p => p.GetBestTitle() ?? "");

            var selectedId = set.Count == 0 ? null : set[index]?.EntityId;

            return new /*ReplaceSet*/
            {
                SelectedId = selectedId,
                Items = results,
                ContentTypeName = ct.StaticName
            };
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Replace(Guid guid, string part, int index, int entityId)
        {
            Log.Add($"replace target:{guid}, part:{part}, index:{index}, id:{entityId}");
            var versioning = CmsBlock.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                var cms = new CmsManager(CmsBlock.App, Log);
                var contentGroup = cms.Read.Blocks.GetBlockConfig(guid);
                cms.Blocks.UpdateEntityIfChanged(contentGroup, part, index, entityId, false, null);
                //contentGroup.UpdateEntityIfChanged(part, index, entityId, false, null);
            }

            // use dnn versioning - this is always part of page
            var context = GetContext(CmsBlock, Log);
            versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, InternalSave);
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

            var versioning = CmsBlock.Environment.PagePublishing;

            void InternalSave(VersioningActionInfo args)
            {
                var cg = GetContentGroup(guid);

                var sequence = list.Select(i => i.Index).ToArray();
                new CmsManager(CmsBlock.App, Log).Blocks.ReorderAll(cg, sequence);
                //cg.ReorderAll(sequence);
            }

            // use dnn versioning - items here are always part of list
            var context = GetContext(CmsBlock, Log);
            versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, InternalSave);

            return true;

        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public SortedEntityItem Header(Guid guid)
        {
            Log.Add($"header for:{guid}");
            var cg = GetContentGroup(guid);

            var header = cg.ListContent.FirstOrDefault();

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