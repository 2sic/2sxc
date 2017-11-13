using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ContentGroupController : SxcApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sCoGr");
        }

        // ToDo 2rm: Check if this is needed somewhere...
        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        //public object Get([FromUri] Guid contentGroupGuid)
        //{
        //    var contentGroup = GetContentGroup(contentGroupGuid);

        //    return new
        //	{
        //		Guid = contentGroup.ContentGroupGuid,
        //		Content = contentGroup.Content.Select(e => e == null ? new int?() : e.EntityId).ToArray(),
        //		Presentation = contentGroup.Presentation.Select(e => e == null ? new int?() : e.EntityId).ToArray(),
        //		ListContent = contentGroup.ListContent.Select(e => e.EntityId).ToArray(),
        //		ListPresentation = contentGroup.ListPresentation.Select(e => e.EntityId).ToArray(),
        //		Template = contentGroup.Template == null ? null : new {
        //			contentGroup.Template.Name,
        //			contentGroup.Template.ContentTypeStaticName,
        //			contentGroup.Template.PresentationTypeStaticName,
        //			contentGroup.Template.ListContentTypeStaticName,
        //			contentGroup.Template.ListPresentationTypeStaticName
        //		}
        //	};
        //}

        private ContentGroup GetContentGroup(Guid contentGroupGuid)
        {
            Log.Add($"get group:{contentGroupGuid}");
            var contentGroup = SxcContext.App.ContentGroupManager.GetContentGroup(contentGroupGuid);

            if (contentGroup == null)
                throw new Exception("ContentGroup with Guid " + contentGroupGuid + " does not exist.");
            return contentGroup;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ReplaceSet Replace(Guid guid, string part, int index)
        {
            Log.Add($"replace target:{guid}, part:{part}, index:{index}");
            part = part.ToLower();
            var contentGroup = GetContentGroup(guid);

            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
            var set = part == AppConstants.ContentLower ? contentGroup.Content : contentGroup.ListContent;

            // not sure what this check is for, just leaving it in for now (2015-09-19 2dm)
            if (set == null || contentGroup.Template == null)
                throw new Exception("Cannot find content group");


            var attributeSetName = part == AppConstants.ContentLower
                ? contentGroup.Template.ContentTypeStaticName
                : contentGroup.Template.ListContentTypeStaticName;

            // if no type was defined in this set, then return an empty list as there is nothing to choose from
            if (string.IsNullOrEmpty(attributeSetName))
                return null;

            var cache = App.Data.Cache; 
            var ct = cache.GetContentType(attributeSetName);

            var dataSource = App.Data[ct.Name]; 
            var results = dataSource.List.ToDictionary(p => p.EntityId,
                p => p.GetBestTitle() ?? "");

            var selectedId = set.Count == 0 ? null : set[index]?.EntityId;

            return new ReplaceSet
            {
                SelectedId = selectedId,
                Items = results
            };
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Replace(Guid guid, string part, int index, int entityId)
        {
            Log.Add($"replace target:{guid}, part:{part}, index:{index}, id:{entityId}");
            var versioning = SxcContext.Environment.PagePublishing;// new PagePublishing(Log);

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                var contentGroup = SxcContext.App.ContentGroupManager.GetContentGroup(guid);
                contentGroup.UpdateEntityIfChanged(part, index, entityId, false, null);
            };

            // use dnn versioning - this is always part of page
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
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
                Type = c?.Type.StaticName ?? cg.Template.ContentTypeStaticName
            }).ToList();

            return list;
        }
         

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool ItemList([FromUri] Guid guid, List<SortedEntityItem> list)
        {
            Log.Add($"list for:{guid}, items:{list?.Count}");
            var versioning = SxcContext.Environment.PagePublishing;// new PagePublishing(Log);

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                var cg = GetContentGroup(guid);

                var sequence = list.Select(i => i.Index).ToArray();

                cg.ReorderAll(sequence);
                
            };

            // use dnn versioning - items here are always part of list
            versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);

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
                Type = header?.Type.StaticName?? cg.Template.ListContentTypeStaticName
            };
        }


        #region helper classes for data transport / json interface
        public class ReplaceSet
        {
            public int? SelectedId { get; set; }
            public Dictionary<int, string> Items { get; set; }
        }

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