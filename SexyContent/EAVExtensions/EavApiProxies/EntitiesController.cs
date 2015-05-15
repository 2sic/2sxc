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

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	public class EntitiesController : SxcApiController // DnnApiController
	{
	    private Eav.WebApi.WebApi eavWebApi = new Eav.WebApi.WebApi();

		public EntitiesController(): base()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");

            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
//		    (eavWebApi.Serializer as Serializer).Sxc = Sexy;
		}

	    private void EnsureSerializerHasSxc()
	    {
            (eavWebApi.Serializer as Serializer).Sxc = Sexy;	        
	    }

        [HttpGet]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            EnsureSerializerHasSxc();
            return eavWebApi.GetOne(contentType, id, appId, cultureCode);
        }

        
        /// <summary>
		/// Get all Entities of specified Type
		/// </summary>
		[HttpGet]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId, string cultureCode = null)
		{
            EnsureSerializerHasSxc();
			return eavWebApi.GetEntities(contentType, cultureCode, appId);
		}


		/// <summary>
		/// Get Entities with specified AssignmentObjectTypeId and Key
		/// </summary>
		[HttpGet]
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, Guid keyGuid, string contentType, int appId)
		{
            EnsureSerializerHasSxc();
		    return eavWebApi.GetAssignedEntities(assignmentObjectTypeId, keyGuid, contentType, appId);
		}

        [HttpDelete]
	    public void Delete(string contentType, int id, int appId)
        {
            EnsureSerializerHasSxc();
            eavWebApi.Delete(contentType, id, App.AppId);
        }
        [HttpDelete]
        public void Delete(string contentType, Guid guid, int appId)
        {
            EnsureSerializerHasSxc();
            eavWebApi.Delete(contentType, guid, App.AppId);
        }


	    [HttpPost]
	    public Dictionary<string, object> CreateOrUpdate(string contentType, int id = 0)
	    {
	        return null;
	    }

        #region Content Types
        /// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
		public IContentType GetContentType(string contentType, int appId)
		{
            EnsureSerializerHasSxc();
			return eavWebApi.GetContentType(contentType, appId);
        }

        #endregion
    }
}
