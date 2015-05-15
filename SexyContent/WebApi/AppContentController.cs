using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent.WebApi
{
	/// <summary>
	/// Direct access to app-content items, simple manipulations etc.
	/// Should check for security at each standard call - to see if the current user may do this
	/// Then we can reduce security access level to anonymous, because each method will do the security check
	/// todo: security
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	public class AppContentController : SxcApiController
	{
	    private Eav.WebApi.WebApi _eavWebApi;

		public AppContentController()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            //		    (eavWebApi.Serializer as Serializer).Sxc = Sexy;
		}

	    private void InitEavAndSerializer()
	    {
            // Improve the serializer so it's aware of the 2sxc-context (module, portal etc.)
            _eavWebApi = new Eav.WebApi.WebApi(App.AppId);
            ((Serializer)_eavWebApi.Serializer).Sxc = Sexy;	        
	    }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string cultureCode = null)
        {
            InitEavAndSerializer();
            return _eavWebApi.GetEntities(contentType, cultureCode);
        }

		/// <summary>
		/// Get Entities with specified AssignmentObjectTypeId and Key
		/// </summary>
		[HttpGet]
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, Guid keyGuid, string contentType)
		{
            InitEavAndSerializer();
		    return _eavWebApi.GetAssignedEntities(assignmentObjectTypeId, keyGuid, contentType);
		}

        [HttpDelete]
	    public void Delete(string contentType, int id)
        {
            InitEavAndSerializer();
             _eavWebApi.Delete(contentType, id);
        }
        [HttpDelete]
        public void Delete(string contentType, Guid guid)
        {
            InitEavAndSerializer();
             _eavWebApi.Delete(contentType, guid);
        }


	    [HttpGet]
	    public Dictionary<string, object> GetOne(string contentType, int id)
	    {
            InitEavAndSerializer();
	        return _eavWebApi.GetOne(contentType, id);
	    }

	    [HttpPost]
	    public Dictionary<string, object> CreateOrUpdate(string contentType, int id = 0)
	    {
	        return null;
	    }

    }
}
