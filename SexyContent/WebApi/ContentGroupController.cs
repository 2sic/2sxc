using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.WebApi
{
	[SupportedModules("2sxc,2sxc-app")]
	public class ContentGroupController : SxcApiController
	{

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
            var contentGroup = Sexy.ContentGroups.GetContentGroup(contentGroupGuid);

            if (contentGroup == null)
                throw new Exception("ContentGroup with Guid " + contentGroupGuid + " does not exist.");
            return contentGroup;
        }

        [HttpGet]
	    public ReplaceSet Replace(Guid guid, string part, int index)
	    {
	        part = part.ToLower();
            var contentGroup = GetContentGroup(guid);

            // try to get the entityId. Sometimes it will try to get #0 which doesn't exist yet, that's why it has these checks
            var set = part == "content" ? contentGroup.Content : contentGroup.ListContent;

            // not sure what this check is for, just leaving it in for now (2015-09-19 2dm)
            if (set == null || contentGroup.Template == null)
                throw new Exception("Cannot find content group");


            var attributeSetName = part == "content" ? contentGroup.Template.ContentTypeStaticName : contentGroup.Template.ListContentTypeStaticName;

            // if no type was defined in this set, then return an empty list as there is nothing to choose from
	        if (String.IsNullOrEmpty(attributeSetName))
	            return null;

	        var cache = App.Data.Cache;// DataSource.GetCache()
	        var ct = cache.GetContentType(attributeSetName);


	        var dataSource = App.Data[ct.Name];// attributeSetName];
            var results = dataSource.List.ToDictionary(p => p.Value.EntityId, p => (p.Value.GetBestValue("EntityTitle")??"").ToString());

            var selectedId = set[index]?.EntityId;

            return new ReplaceSet
	        {
	            SelectedId = selectedId,
                Items = results
	        };
	    }

	    public class ReplaceSet
	    {
	        public int? SelectedId { get; set; }
            public Dictionary<int, string> Items { get; set; }  
	    }

	    [HttpPost]
	    public void Replace(Guid guid, string part, int index, int entityId)
	    {
            var contentGroup = Sexy.ContentGroups.GetContentGroup(guid);
            contentGroup.UpdateEntityIfChanged(part, index, entityId, false, null);
        }

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<SortedEntityItem> ItemList(Guid guid)
	    {
	        var cg = GetContentGroup(guid);

	        var list = cg.Content.Select((c, index) => new SortedEntityItem
	        {
                Index = index,
	            Id = c.EntityId,
	            Guid = c.EntityGuid,
	            Title = (c.GetBestValue("EntityTitle")??"").ToString()
	        }).ToList();

	        return list;
	    }

	    public class SortedEntityItem
	    {
	        public int Index;
	        public int Id;
	        public Guid Guid;
	        public string Title;
	    }

	    [HttpPost]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
	    public bool ItemList([FromUri] Guid guid, List<SortedEntityItem> List)
	    {
            var cg = GetContentGroup(guid);

	        var sequence = List.Select(i => i.Index).ToArray();

	        cg.ReorderAll(sequence);
            return true;
	    }
    }
    }