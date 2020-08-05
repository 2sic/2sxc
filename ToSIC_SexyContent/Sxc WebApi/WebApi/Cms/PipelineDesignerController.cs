using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.WebApi.Cms
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [SxcWebApiExceptionHandling]
    [ValidateAntiForgeryToken]
	public class PipelineDesignerController : DnnApiControllerWithFixes, IPipelineDesignerController
    {
        protected override string HistoryLogName => "Api.SxcQry";

		private QueryApi QryCtc => _eavCont ?? (_eavCont = new QueryApi(Log));
		private QueryApi _eavCont;

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
		public QueryDefinitionDto GetPipeline(int appId, int? id = null) => QryCtc.Definition(appId, id);

        /// <summary>
        /// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
        /// </summary>
        [HttpGet]
        public IEnumerable<QueryRuntime.DataSourceInfo> GetInstalledDataSources() =>
            QueryRuntime.GetInstalledDataSources();

		/// <summary>
		/// Save Pipeline
		/// </summary>
		/// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
		/// <param name="appId">AppId this Pipeline belongs to</param>
		/// <param name="id">PipelineEntityId</param>
		[HttpPost]
	    public QueryDefinitionDto SavePipeline([FromBody] QueryDefinitionDto data, int appId, int id)
	        => QryCtc.Save(data, appId, id);


	    /// <summary>
	    /// Query the Result of a Pipeline using Test-Parameters
	    /// </summary>
	    [HttpGet]
	    public QueryRunDto QueryPipeline(int appId, int id)
	    {
	        var modId = ActiveModule?.ModuleID ?? 0;
	        var wrapLog = Log.Call($"app:{appId}, id:{id}", message: $"mid:{modId}");
	        var dnnConfigProvider = new GetDnnEngine().GetEngine(modId, Log);
            var result = QryCtc.Run(appId, id, dnnConfigProvider);
            wrapLog(null);
            return result;
        }

        /// <summary>
	    /// Clone a Pipeline with all DataSources and their configurations
	    /// </summary>
	    [HttpGet]
	    public void ClonePipeline(int appId, int id) => QryCtc.Clone(appId, id);
	

		/// <summary>
		/// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
		/// </summary>
		[HttpGet]
		public object DeletePipeline(int appId, int id)
		{
            Log.Add($"delete pipe:{id} on app:{appId}");
			// Stop if a Template uses this Pipeline
            var cms = new CmsRuntime(appId, Log, true );
			var templatesUsingPipeline = cms.Views.GetAll()
                .Where(t => t.Query?.Id == id)
                .Select(t => t.Id)
                .ToArray();
			if (templatesUsingPipeline.Any())
				throw new Exception(
				        $"Pipeline is used by Views and cant be deleted. Pipeline EntityId: {id}. TemplateIds: {string.Join(", ", templatesUsingPipeline)}");

			return QryCtc.Delete(appId, id);
		}

	    [HttpPost]
	    public bool ImportQuery(EntityImportDto args) => QryCtc.Import(args);
	}
}