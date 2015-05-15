using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	public class OLDEntitiesController : DnnApiController
	{
	    private readonly Eav.WebApi.WebApi eavWebApi = new Eav.WebApi.WebApi();//  new Eav.ManagementUI.API.EntitiesController();

		public OLDEntitiesController()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
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
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int appId, int assignmentObjectTypeId, Guid keyGuid, string contentTypeName)
		{
			return eavWebApi.GetAssignedEntities(appId, assignmentObjectTypeId, keyGuid, contentTypeName);
		}

		/// <summary>
		/// Get Entities with specified AssignmentObjectTypeId and Key
		/// </summary>
		public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int appId, int assignmentObjectTypeId, string keyString, string contentTypeName)
		{
			return eavWebApi.GetAssignedEntities(appId, assignmentObjectTypeId, keyString, contentTypeName);
		}

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
