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
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [DnnLogExceptions]
    [ValidateAntiForgeryToken]
	public class QueryController : SxcApiControllerBase, IQueryController
    {
        protected override string HistoryLogName => "Api.Query";

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
		public QueryDefinitionDto Get(int appId, int? id = null) => Eav.Factory.Resolve<QueryApi>().Init(appId, Log).Definition(appId, id);

        /// <summary>
        /// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
        /// </summary>
        [HttpGet]
        public IEnumerable<QueryRuntime.DataSourceInfo> DataSources() => QueryRuntime.QueryDataSources();

		/// <summary>
		/// Save Pipeline
		/// </summary>
		/// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
		/// <param name="appId">AppId this Pipeline belongs to</param>
		/// <param name="id">PipelineEntityId</param>
		[HttpPost]
	    public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id)
	        => Eav.Factory.Resolve<QueryApi>().Init(appId, Log).Save(data, appId, id);


	    /// <summary>
	    /// Query the Result of a Pipeline using Test-Parameters
	    /// </summary>
	    [HttpGet]
	    public QueryRunDto Run(int appId, int id)
        {
            var block = GetBlock();
            var instanceId = ActiveModule?.ModuleID ?? 0;
            var config = ConfigurationProvider.GetConfigProviderForModule(instanceId, block?.App, block);
            return Eav.Factory.Resolve<QueryApi>().Init(appId, Log).Run(appId, id, instanceId, config);
        }

        /// <summary>
	    /// Clone a Pipeline with all DataSources and their configurations
	    /// </summary>
	    [HttpGet]
	    public void Clone(int appId, int id) => Eav.Factory.Resolve<QueryApi>().Init(appId, Log).Clone(appId, id);


        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        [HttpDelete]
        public bool Delete(int appId, int id)
            => Eav.Factory.Resolve<CmsManager>().Init(State.Identity(null, appId), true, false, Log)
                .DeleteQueryIfNotUsedByView(id, Log);

        [HttpPost]
	    public bool Import(EntityImportDto args) => Eav.Factory.Resolve<QueryApi>().Init(args.AppId, Log).Import(args);
	}
}