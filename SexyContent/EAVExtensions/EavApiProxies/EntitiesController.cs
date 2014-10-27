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
	public class EntitiesController : DnnApiController
	{
		private readonly Eav.ManagementUI.API.EntitiesController _controller = new Eav.ManagementUI.API.EntitiesController();

		public EntitiesController()
		{
			Eav.Configuration.SetConnectionString("SiteSqlServer");
		}

		/// <summary>
		/// Get all Entities of specified Type
		/// </summary>
		[HttpGet]
		public IEnumerable<Dictionary<string, object>> GetEntities(int appId, string typeName, string cultureCode = null)
		{
			return _controller.GetEntities(appId, typeName, cultureCode);
		}

		/// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
		public IContentType GetContentType(int appId, string name)
		{
			return _controller.GetContentType(appId, name);
		}
	}
}
