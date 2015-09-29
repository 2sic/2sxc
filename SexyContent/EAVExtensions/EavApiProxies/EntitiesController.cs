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
	    private Eav.WebApi.EntitiesController entitesController = new Eav.WebApi.EntitiesController();

		public EntitiesController(): base()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
		}

	    private void EnsureSerializerHasSxc()
	    {
            (entitesController.Serializer as Serializer).Sxc = Sexy;
	    }

        [HttpGet]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            EnsureSerializerHasSxc();
            return entitesController.GetOne(contentType, id, appId, cultureCode);
        }


        [HttpPost]
        public dynamic GetPackage([FromBody]  EditPackageRequest packageRequest)
        {
            if (packageRequest.Type == "entities")
                return entitesController.GetPackage(App.AppId, packageRequest);
            
            if(packageRequest.Type != "group")
                throw new NotSupportedException("Package type " + packageRequest.Type + " is not supported.");

            var contentGroup = Sexy.ContentGroups.GetContentGroup(packageRequest.GroupGuid);

            return new
            {
                entities = packageRequest.GroupSet.Select(s => new {
                    packageInfo = new {
                        type = "group",
                        groupGuid = packageRequest.GroupGuid,
                        groupSet = s,
                        groupIndex = packageRequest.GroupIndex,
                        contentTypeName = contentGroup.Template.ContentTypeStaticName // UI needs to know which content type to load...
                    },
                    entity = contentGroup[s].Count > packageRequest.GroupIndex && contentGroup[s][packageRequest.GroupIndex] != null ?
                        entitesController.GetOne(App.AppId, contentGroup.Template.ContentTypeStaticName, contentGroup[s][packageRequest.GroupIndex].EntityId) :
                        null
                })
            };
        }

        public class EditPackageRequest : ToSic.Eav.WebApi.EntitiesController.EditPackageRequestEntities {
            public Guid GroupGuid { get; set; }
            public string[] GroupSet { get; set; }
            public int GroupIndex { get; set; }
        }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId, string cultureCode = null)
		{
            EnsureSerializerHasSxc();
			return entitesController.GetEntities(contentType, cultureCode, appId);
		}

	    [HttpGet]
	    public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType)
	    {
	        EnsureSerializerHasSxc();
	        return entitesController.GetAllOfTypeForAdmin(appId, contentType);
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
            entitesController.Delete(contentType, id, App.AppId);
        }
        [HttpDelete]
        public void Delete(string contentType, Guid guid, int appId)
        {
            EnsureSerializerHasSxc();
            entitesController.Delete(contentType, guid, App.AppId);
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
