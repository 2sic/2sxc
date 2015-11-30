using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.Serializers;
using ToSic.SexyContent.WebApi;
using System.Linq;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	public class EntitiesController : SxcApiController // DnnApiController
	{
	    private Eav.WebApi.EntitiesController entitiesController = new Eav.WebApi.EntitiesController();

		public EntitiesController(): base()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
		}

	    private void EnsureSerializerHasSxc()
	    {
            (entitiesController.Serializer as Serializer).Sxc = Sexy;
	    }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            EnsureSerializerHasSxc();
            return entitiesController.GetOne(contentType, id, appId, cultureCode);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic GetManyForEditing([FromBody]  List<ItemIdentifier> items, int appId)
        {
            // this will contain the list of the items we'll really return
            var newItems = new List<ItemIdentifier>();

            // go through all the groups, assign relevant info so that we can then do get-many
            foreach (var reqItem in items)
            {
                // only do special processing if it's a "group" item
                if (reqItem.Group == null)
                {
                    newItems.Add(reqItem);
                    continue;
                }
                
                var contentGroup = Sexy.ContentGroups.GetContentGroup(reqItem.Group.Guid);
                var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(reqItem.Group.Part);

                // if there is no content-type for this, then skip it (don't deliver anything)
                if (contentTypeStaticName == "")
                    continue;

                var part = contentGroup[reqItem.Group.Part];
                reqItem.ContentTypeName = contentTypeStaticName;
                if (!reqItem.Group.Add && // not in add-mode
                              part.Count > reqItem.Group.Index && // has as many items as desired
                              part[reqItem.Group.Index] != null) // and the slot has something
                    reqItem.EntityId = part[reqItem.Group.Index].EntityId;

                // tell the UI that it should not actually use this data yet, keep it locked
                if (reqItem.Group.Part.ToLower().Contains("presentation")) {
                    reqItem.Group.SlotCanBeEmpty = true;  // all presentations can always be locked
                    if (reqItem.EntityId == 0)
                    {
                        reqItem.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with
                        
                        reqItem.DuplicateEntity =
                            reqItem.Group.Part.ToLower() == "presentation"
                            ? contentGroup.Template.PresentationDemoEntity?.EntityId as int?
                            : contentGroup.Template.ListPresentationDemoEntity?.EntityId as int?;
                    }
                }
                
                newItems.Add(reqItem);
            }

            // Now get all
            return entitiesController.GetManyForEditing(appId, newItems);

            // todo: find out how to handle "Presentation" items
            

        }



        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        // todo: should refactor to save all items in 1 transaction
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<EntityWithHeader> items)
        {
            // first, save all to do it in 1 transaction
            // note that it won't save the SlotIsEmpty ones, as these won't be needed
            var postSaveIds = entitiesController.SaveMany(appId, items.Select(i => new EntityWithHeader { Header = i.Header, Entity = i.Entity}).ToList());

            // now assign all content-groups as needed
            var groupItems = items
                .Where(i => i.Header.Group != null)
                .GroupBy( i => i.Header.Group.Guid.ToString() + i.Header.Group.Index.ToString() + i.Header.Group.Add);
            foreach (var entitySets in groupItems)
            {
                var contItem = entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == "content") ??
                              entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == "listcontent");
                if(contItem == null)
                    throw new Exception("unexpected group-entity assigment, cannot figure it out");

                var presItem = entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == "presentation") ??
                              entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == "listpresentation");

                // Get group to assign to and parameters
                var contentGroup = Sexy.ContentGroups.GetContentGroup(contItem.Header.Group.Guid);
                var partName = contItem.Header.Group.Part;
                var part = contentGroup[partName];
                var index = contItem.Header.Group.Index;

                // Get saved entity (to get its ID)
                if (!postSaveIds.ContainsKey(contItem.Entity.Guid) )
                    throw new Exception("Saved entity not found - not able to update ContentGroup");

                int postSaveId = postSaveIds[contItem.Entity.Guid];

                int? presentationId = null;

                if (presItem != null)
                {
                    if (postSaveIds.ContainsKey(presItem.Entity.Guid))
                        presentationId = postSaveIds[presItem.Entity.Guid];

                    presentationId = presItem.Header.Group.SlotIsEmpty ? null : presentationId;
                        // use null if it shouldn't have one
                }
                
                if (contItem.Header.Group.Add) // this cannot be auto-detected, it must be specified
                {
                    contentGroup.AddContentAndPresentationEntity(partName, index, postSaveId, presentationId);
                }
                // otherwise it's an update 
                else // if (part.Count <= index || part[index] == null)
                    contentGroup.UpdateEntityIfChanged(partName, index, postSaveId, true, presentationId);

            }

            #region update-module-title
            // check the contentGroup as to what should be the module title, then try to set it
            // technically it could have multiple different groups to save in, 
            // ...but for now we'll just update the current modules title
            // note: it also correctly handles published/unpublished, but I'm not sure why :)
            var modContentGroup = Sexy.ContentGroups.GetContentGroupForModule(Dnn.Module.ModuleID);

            var titleItem = modContentGroup.ListContent.FirstOrDefault() ?? modContentGroup.Content.FirstOrDefault();

            if (titleItem != null)
                Dnn.Module.ModuleTitle = titleItem.GetBestValue("EntityTitle").ToString();
            #endregion

            return postSaveIds;
        }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId, string cultureCode = null)
		{
            EnsureSerializerHasSxc();
			return entitiesController.GetEntities(contentType, cultureCode, appId);
		}

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType)
	    {
	        EnsureSerializerHasSxc();
	        return entitiesController.GetAllOfTypeForAdmin(appId, contentType);
	    }


        //2015-09-12 deprecated this - should use the metadata-controller
        /// <summary>
        /// Get Entities with specified AssignmentObjectTypeId and Key
        /// </summary>
        //[HttpGet]
		//public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, Guid keyGuid, string contentType, int appId)
		//{
		//    var metadataController = new ToSic.Eav.WebApi.MetadataController();
  //          EnsureSerializerHasSxc();
  //          return metadataController.GetAssignedEntities(assignmentObjectTypeId, keyGuid, contentType, appId);
		//}

        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Delete(string contentType, int id, int appId)
        {
            EnsureSerializerHasSxc();
            entitiesController.Delete(contentType, id, App.AppId);
        }
        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Delete(string contentType, Guid guid, int appId)
        {
            EnsureSerializerHasSxc();
            entitiesController.Delete(contentType, guid, App.AppId);
        }


	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public Dictionary<string, object> CreateOrUpdate(string contentType, int id = 0)
	    {
	        throw new NotImplementedException();
	    }

        #region Content Types
        /// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IContentType GetContentType(string contentType, int appId)
		{
            EnsureSerializerHasSxc();
            // todo refactor-verify
            return new Eav.WebApi.ContentTypeController().GetSingle(appId, contentType, null);
            // return entitesController.GetContentType(contentType, appId);
		}

        #endregion
    }
}
