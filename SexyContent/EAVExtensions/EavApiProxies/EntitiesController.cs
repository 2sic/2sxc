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
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
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
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            EnsureSerializerHasSxc();
            return entitiesController.GetOne(contentType, id, appId, cultureCode);
        }


        [HttpPost]
        public dynamic GetManyForEditing([FromBody]  List<ItemIdentifier> items)
        {
            // this will contain the list of the items we'll really return
            var newItems = new List<ItemIdentifier>();

            // go through all the groups, assign relevant info so that we can then do get-many
            foreach (var s in items)
            {
                // only do special processing if it's a "group" item
                if (s.Group == null)
                {
                    newItems.Add(s);
                    continue;
                }
                
                var contentGroup = Sexy.ContentGroups.GetContentGroup(s.Group.Guid);
                var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(s.Group.Part);

                // if there is no content-type for this, then skip it (don't deliver anything)
                if (contentTypeStaticName == "")
                    continue;

                var part = contentGroup[s.Group.Part];
                s.ContentTypeName = contentTypeStaticName;
                if (!s.Group.Add && // not in add-mode
                              part.Count > s.Group.Index && // has as many items as desired
                              part[s.Group.Index] != null) // and the slot has something
                    s.EntityId = part[s.Group.Index].EntityId;

                // tell the UI that it should not actually use this data yet, keep it locked
                if (s.Group.Part.ToLower().Contains("presentation")) {
                    s.Group.SlotCanBeEmpty = true;  // all presentations can always be locked
                    if (s.EntityId == 0)
                        s.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with
                }
                
                newItems.Add(s);
            }

            // Now get all
            return entitiesController.GetManyForEditing(App.AppId, newItems);

            // todo: find out how to handle "Presentation" items
            

        }



        [HttpPost]
        // todo: should refactor to save all items in 1 transaction
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<EntityWithHeader> items)
        {
            // var success = true;

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

                // Get saved entity (to get its ID) - ToDo: Should get ID from Save method, would clean up this code
                //var dataSource = DataSource.GetInitialDataSource(App.ZoneId, App.AppId, false);
                //var contentEntity = dataSource.LightList.FirstOrDefault(p => p.EntityGuid == contItem.Entity.Guid);
                if (!postSaveIds.ContainsKey(contItem.Entity.Guid) )
                    throw new Exception("Saved entity not found - not able to update ContentGroup");

                int postSaveId = postSaveIds[contItem.Entity.Guid];

                int? presentationId = null;
                if (postSaveIds.ContainsKey(presItem.Entity.Guid))
                    presentationId = postSaveIds[presItem.Entity.Guid];

                presentationId = presItem.Header.Group.SlotIsEmpty ? null : presentationId; // use null if it shouldn't have one

                //if (presItem != null)
                //    presentationId =
                //        dataSource.LightList.FirstOrDefault(p => p.EntityGuid == presItem.Entity.Guid).EntityId;


                if (contItem.Header.Group.Add) // this cannot be auto-detected, it must be specified
                {
                    contentGroup.AddContentAndPresentationEntity(partName, index, postSaveId, presentationId);
                }
                // otherwise it's an update 
                else // if (part.Count <= index || part[index] == null)
                    contentGroup.UpdateEntityIfChanged(partName, index, postSaveId, true, presentationId);

            }
            return postSaveIds;
        }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId, string cultureCode = null)
		{
            EnsureSerializerHasSxc();
			return entitiesController.GetEntities(contentType, cultureCode, appId);
		}

	    [HttpGet]
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
	    public void Delete(string contentType, int id, int appId)
        {
            EnsureSerializerHasSxc();
            entitiesController.Delete(contentType, id, App.AppId);
        }
        [HttpDelete]
        public void Delete(string contentType, Guid guid, int appId)
        {
            EnsureSerializerHasSxc();
            entitiesController.Delete(contentType, guid, App.AppId);
        }


	    [HttpPost]
	    public Dictionary<string, object> CreateOrUpdate(string contentType, int id = 0)
	    {
	        throw new NotImplementedException();
	    }

        #region Content Types
        /// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
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
