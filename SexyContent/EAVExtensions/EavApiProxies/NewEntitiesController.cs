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
	public class NewEntitiesController : SxcApiController // DnnApiController
	{
		private readonly Eav.WebApi.WebApi eavWebApi = new Eav.WebApi.WebApi();

		public NewEntitiesController()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
		    (eavWebApi.Serializer as ToSic.SexyContent.Serializers.Serializer).Sxc = Sexy;
		}

		/// <summary>
		/// Get all Entities of specified Type
		/// </summary>
		[HttpGet]
		public IEnumerable<Dictionary<string, object>> GetEntities(int appId, string typeName, string cultureCode = null)
		{
			return eavWebApi.GetEntities(appId, typeName, cultureCode);
		}

		/// <summary>
		/// Get Entities with specified AssignmentObjectTypeId and Key
		/// </summary>
		[HttpGet]
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int appId, int assignmentObjectTypeId, Guid keyGuid, string contentTypeName)
		{
            // todo: get AppId if not delivered automatically
		    var ser = new Serializers.Serializer();
		    ser.Sxc = this.Sexy;
		    IEnumerable<Dictionary<string, object>> list = eavWebApi.GetAssignedEntities(appId, assignmentObjectTypeId, keyGuid,
		        contentTypeName);
			return ser.Prepare(list);
		}

        [HttpDelete]
	    public bool Delete(string contentType, int id)
        {
            var found = App.Data.List[id];
            if (found.Type.Name == contentType)
            {
                App.Data.Delete(id);
                return true;
            }
            throw new KeyNotFoundException("Can't find " + id + "of type '" + contentType + "'");
        }

	    [HttpGet]
	    public Dictionary<string, object> GetOne(string contentType, int id)
	    {
            var found = App.Data.List[id];
	        if (found.Type.Name == contentType)
	        {
	            var ser = new Serializer();
	            ser.Sxc = this.Sexy;
	            return ser.Prepare(found);
	        }
	        throw new KeyNotFoundException("Can't find " + id + "of type '" + contentType + "'");
	    }

	    [HttpPost]
	    public Dictionary<string, object> CreateOrUpdate(string contentType, int id = 0)
	    {
	        return null;
	    }


            /// <summary>
		/// Get Entities with specified AssignmentObjectTypeId and Key
		/// </summary>
        //public IEnumerable<IEntity> GetAssignedEntities(int appId, int assignmentObjectTypeId, string keyString, string contentTypeName)
        //{
        //    return _controller.GetAssignedEntities(appId, assignmentObjectTypeId, keyString, contentTypeName);
        //}

		/// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
		public IContentType GetContentType(int appId, string name)
		{
			return eavWebApi.GetContentType(appId, name);
		}
	}
}
