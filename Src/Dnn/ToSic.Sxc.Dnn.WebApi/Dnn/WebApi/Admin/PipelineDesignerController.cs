using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi.Cms
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [DnnLogExceptions]
    [ValidateAntiForgeryToken]
	public class PipelineDesignerController : DnnApiControllerWithFixes, IPipelineDesignerController
    {
        protected override string HistoryLogName => "Api.SxcQry";

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
		public QueryDefinitionDto GetPipeline(int appId, int? id = null) 
            => new QueryApi(Log).Definition(appId, id);

        /// <summary>
        /// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
        /// </summary>
        [HttpGet]
        public IEnumerable<QueryRuntime.DataSourceInfo> GetInstalledDataSources() 
            => QueryRuntime.GetInstalledDataSources();

		/// <summary>
		/// Save Pipeline
		/// </summary>
		/// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
		/// <param name="appId">AppId this Pipeline belongs to</param>
		/// <param name="id">PipelineEntityId</param>
		[HttpPost]
	    public QueryDefinitionDto SavePipeline([FromBody] QueryDefinitionDto data, int appId, int id)
	        => new QueryApi(Log).Save(data, appId, id);


	    /// <summary>
	    /// Query the Result of a Pipeline using Test-Parameters
	    /// </summary>
	    [HttpGet]
	    public QueryRunDto QueryPipeline(int appId, int id) 
            => new QueryApi(Log).Run(appId, id, ActiveModule?.ModuleID ?? 0);

        /// <summary>
	    /// Clone a Pipeline with all DataSources and their configurations
	    /// </summary>
	    [HttpGet]
	    public void ClonePipeline(int appId, int id) 
            => new QueryApi(Log).Clone(appId, id);


        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        [HttpGet]
        public bool DeletePipeline(int appId, int id)
            => new CmsManager(State.Identity(null, appId), true, false, Log)
                .DeleteQueryIfNotUsedByView(id, Log);

        [HttpPost]
	    public bool ImportQuery(EntityImportDto args) 
            => new QueryApi(Log).Import(args);
	}
}