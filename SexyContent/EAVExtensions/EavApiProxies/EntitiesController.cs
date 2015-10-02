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
        public dynamic GetManyForEditing([FromBody]  List<EntityWithHeader> items)
        {
            // go through all the groups, assign relevant info so that we can then do get-many
            var entities = items.Where(i => i.Group != null).ToList();
            foreach (var s in entities)
            {
                var contentGroup = Sexy.ContentGroups.GetContentGroup(s.Group.Guid);
                var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(s.Group.Set);
                if (contentTypeStaticName == "")
                    continue;

                // Header should be null, so we have to create one first
                s.Header = new Eav.WebApi.EntitiesController.ItemIdentifier();

                s.Header.ContentTypeName = contentTypeStaticName;
                s.Header.EntityId = (contentGroup[s.Group.Set].Count > s.Group.Index &&
                                     contentGroup[s.Group.Set][s.Group.Index] != null)
                    ? contentGroup[s.Group.Set][s.Group.Index].EntityId
                    : 0;
            }

            // Now get all
            return entitiesController.GetManyForEditing(App.AppId, items.Select(i =>  i.Header).ToList());

            // todo: find out how to handle "Presentation" items

            //.
            //Select(s => {
            //        return new
            //        {
            //            packageInfo = new
            //            {
            //                type = "group",
            //                groupGuid = s.Group.Guid,
            //                groupSet = s,
            //                groupIndex = s.Group.Index,
            //                contentTypeName = contentTypeStaticName // UI needs to know which content type to load...
            //            },
            //            entity = contentGroup[s.Group.Set].Count > s.Group.Index && contentGroup[s.Group.Set][s.Group.Index] != null ?
            //                entitiesController.GetOne(App.AppId, contentTypeStaticName, contentGroup[s.Group.Set][s.Group.Index].EntityId) :
            //                null
            //        };
            //    }).Where(c => c != null)

        }


        public class EntityWithHeader: Eav.WebApi.EntitiesController.EntityWithHeader
        {
            public GroupAssignment Group { get; set; }
        }

	    public class GroupAssignment
	    {
            public Guid Guid { get; set; }
            public string Set { get; set; }
            public int Index { get; set; }
	    }


        [HttpPost]
        // todo: should refactor to save all items in 1 transaction
        public bool SaveMany([FromUri] int appId, [FromBody] List<EntityWithHeader> items)
        {
            // var success = true;

            // first, save all to do it in 1 transaction
            entitiesController.SaveMany(appId, items.Select(i => new Eav.WebApi.EntitiesController.EntityWithHeader() { Header = i.Header, Entity = i.Entity}).ToList());

            // now assign all content-groups as needed
            foreach (var entity in items)
            {
                // Get saved entity (to get its ID) - ToDo: Should get ID from Save method, would clean up this code
                var dataSource = DataSource.GetInitialDataSource(App.ZoneId, App.AppId, false);
                dataSource = DataSource.GetDataSource<EntityTypeFilter>(App.ZoneId, App.AppId, dataSource);
                ((EntityTypeFilter)dataSource).TypeName = entity.Entity.Type.StaticName;
                var savedEntity = dataSource.List.Where(p => p.Value.EntityGuid == entity.Entity.Guid).Select(p => p.Value).FirstOrDefault();

                if (savedEntity == null)
                    throw new Exception("Saved entity not found - not able to save contentgroup");

                // ... then update contentgroup info (if defined)
                if (entity.Group != null)
                {
                    var contentGroup = Sexy.ContentGroups.GetContentGroup(entity.Group.Guid);
                    var groupSet = entity.Group.Set;
                    var groupIndex = entity.Group.Index;
                    if (contentGroup[groupSet].Count <= groupIndex || contentGroup[groupSet][groupIndex] == null || contentGroup[groupSet][groupIndex].EntityId != savedEntity.EntityId)
                        contentGroup.UpdateEntity(groupSet, groupIndex, savedEntity.EntityId);
                }
            }
            return true;
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
