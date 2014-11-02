using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	public class PipelineDesignerController : DnnApiController
	{
		private readonly Eav.ManagementUI.API.PipelineDesignerController _controller;

		public PipelineDesignerController()
		{
			var userName = UserController.Instance.GetCurrentUserInfo().Username;
			_controller = new Eav.ManagementUI.API.PipelineDesignerController(userName, "SiteSqlServer");
		}

		/// <summary>
		/// Get a Pipeline with DataSources
		/// </summary>
		[HttpGet]
		public Dictionary<string, object> GetPipeline(int appId, int? id = null)
		{
			return _controller.GetPipeline(appId, id);
		}

		/// <summary>
		/// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
		/// </summary>
		[HttpGet]
		public IEnumerable<object> GetInstalledDataSources()
		{
			return _controller.GetInstalledDataSources();
		}

		/// <summary>
		/// Save Pipeline
		/// </summary>
		/// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
		/// <param name="appId">AppId this Pipeline belogs to</param>
		/// <param name="id">PipelineEntityId</param>
		[HttpPost]
		public Dictionary<string, object> SavePipeline([FromBody] dynamic data, int appId, int? id = null)
		{
			return _controller.SavePipeline(data, appId, id);
		}

		/// <summary>
		/// Query the Result of a Pipline using Test-Parameters
		/// </summary>
		[HttpGet]
		public Dictionary<string, IEnumerable<IEntity>> QueryPipeline(int appId, int id)
		{
			return _controller.QueryPipeline(appId, id);
		}

		/// <summary>
		/// Clone a Pipeline with all DataSources and their configurations
		/// </summary>
		[HttpGet]
		public object ClonePipeline(int appId, int id)
		{
			return _controller.ClonePipeline(appId, id);
		}
	}
}