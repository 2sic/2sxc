using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [SxcWebApiExceptionHandling]
	public class PipelineDesignerController : DnnApiControllerWithFixes
	{
		private readonly Eav.WebApi.PipelineDesignerController _controller;

		public PipelineDesignerController()
		{
			_controller = new Eav.WebApi.PipelineDesignerController(/*Environment.Dnn7.UserIdentity.CurrentUserIdentityToken, */"SiteSqlServer");
		}

		/// <summary>
		/// Get a Pipeline with DataSources
		/// </summary>
		[HttpGet]
		public Dictionary<string, object> GetPipeline(int appId, int? id = null) => _controller.GetPipeline(appId, id);

	    /// <summary>
		/// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
		/// </summary>
		[HttpGet]
		public IEnumerable<object> GetInstalledDataSources() => _controller.GetInstalledDataSources();

	    /// <summary>
	    /// Save Pipeline
	    /// </summary>
	    /// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
	    /// <param name="appId">AppId this Pipeline belogs to</param>
	    /// <param name="id">PipelineEntityId</param>
	    [HttpPost]
	    public Dictionary<string, object> SavePipeline([FromBody] dynamic data, int appId, int? id = null)
	        => _controller.SavePipeline(data, appId, id);


	    /// <summary>
	    /// Query the Result of a Pipline using Test-Parameters
	    /// </summary>
	    [HttpGet]
	    public dynamic QueryPipeline(int appId, int id)
	        => _controller.QueryPipeline(appId, id);

	    /// <summary>
	    /// Clone a Pipeline with all DataSources and their configurations
	    /// </summary>
	    [HttpGet]
	    public object ClonePipeline(int appId, int id)
	        => _controller.ClonePipeline(appId, id);
	

		/// <summary>
		/// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
		/// </summary>
		[HttpGet]
		public object DeletePipeline(int appId, int id)
		{
			// Stop if a Template uses this Pipeline
			//var sexy = new SxcInstance(0, appId);
            var app = new App(PortalSettings.Current, appId);
			var templatesUsingPipeline = app.TemplateManager.GetAllTemplates().Where(t => t.Pipeline != null && t.Pipeline.EntityId == id).Select(t => t.TemplateId).ToArray();
			if (templatesUsingPipeline.Any())
				throw new Exception(
				        $"Pipeline is used by Templates and cant be deleted. Pipeline EntityId: {id}. TemplateIds: {string.Join(", ", templatesUsingPipeline)}");

			return _controller.DeletePipeline(appId, id);
		}
	}
}