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
        public dynamic GetPackage([FromBody]  EditPackageRequest packageRequest)
        {
            if (packageRequest.Type == "entities")
                return entitiesController.GetPackage(App.AppId, packageRequest);
            
            if(packageRequest.Type != "group")
                throw new NotSupportedException("Package type " + packageRequest.Type + " is not supported.");

            var contentGroup = Sexy.ContentGroups.GetContentGroup(packageRequest.GroupGuid);

            return new
            {
                entities = packageRequest.GroupSet.Select(s => {
                    var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(s);
                    if (contentTypeStaticName == "")
                        return null;
                    return new
                    {
                        packageInfo = new
                        {
                            type = "group",
                            groupGuid = packageRequest.GroupGuid,
                            groupSet = s,
                            groupIndex = packageRequest.GroupIndex,
                            contentTypeName = contentTypeStaticName // UI needs to know which content type to load...
                        },
                        entity = contentGroup[s].Count > packageRequest.GroupIndex && contentGroup[s][packageRequest.GroupIndex] != null ?
                            entitiesController.GetOne(App.AppId, contentTypeStaticName, contentGroup[s][packageRequest.GroupIndex].EntityId) :
                            null
                    };
                }).Where(c => c != null)
            };
        }

        public class EditPackageRequest : ToSic.Eav.WebApi.EntitiesController.EditPackageRequestEntities {
            public Guid GroupGuid { get; set; }
            public string[] GroupSet { get; set; }
            public int GroupIndex { get; set; }
        }

        public bool SavePackage([FromUri] int appId, [FromBody] Eav.WebApi.EntitiesController.EditPackage editPackage)
        {
            var success = true;
            foreach (var entity in editPackage.Entities)
            {
                // Save entity in EAV
                success = success && entitiesController.Save(entity.Entity, appId);

                // Get saved entity (to get its ID) - ToDo: Should get ID from Save method, would clean up this code
                var dataSource = DataSource.GetInitialDataSource(App.ZoneId, App.AppId, false);
                dataSource = DataSource.GetDataSource<EntityTypeFilter>(App.ZoneId, App.AppId, dataSource);
                ((EntityTypeFilter)dataSource).TypeName = entity.Entity.Type.StaticName;
                var savedEntity = dataSource.List.Where(p => p.Value.EntityGuid == entity.Entity.Guid).Select(p => p.Value).FirstOrDefault();

                if (savedEntity == null)
                    throw new Exception("Saved entity not found - not able to save contentgroup");

                // ... then update contentgroup info (if defined)
                if (entity.PackageInfo.type == "group")
                {
                    var contentGroup = Sexy.ContentGroups.GetContentGroup((Guid)entity.PackageInfo.groupGuid);
                    var groupSet = (string)entity.PackageInfo.groupSet;
                    var groupIndex = (int)entity.PackageInfo.groupIndex;
                    if (contentGroup[groupSet].Count <= groupIndex || contentGroup[groupSet][groupIndex] == null || contentGroup[groupSet][groupIndex].EntityId != savedEntity.EntityId)
                        contentGroup.UpdateEntity(groupSet, groupIndex, savedEntity.EntityId);
                }
            }
            return success;
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
