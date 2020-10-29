using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    /// <summary>
    /// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
    /// </summary>
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class QueryController : SxcStatefulControllerBase, IQueryController
    {
        protected override string HistoryLogName => "Api.Query";

        public QueryController(StatefulControllerDependencies dependencies) : base(dependencies)
        { }

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
        public QueryDefinitionDto Get(int appId, int? id = null) => new QueryApi(Log).Definition(appId, id);

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
            => new QueryApi(Log).Save(data, appId, id);


        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        [HttpGet]
        public QueryRunDto Run(int appId, int id)
        {
            var block = GetBlock();
            var instanceId = GetContext().Container.Id; // ?? 0;
            var config = ConfigurationProvider.GetConfigProviderForModule(instanceId, block?.App, block);
            return new QueryApi(Log).Run(appId, id, instanceId, config);
        }

        /// <summary>
	    /// Clone a Pipeline with all DataSources and their configurations
	    /// </summary>
	    [HttpGet]
        public void Clone(int appId, int id) => new QueryApi(Log).Clone(appId, id);


        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        [HttpDelete]
        public bool Delete(int appId, int id)
            => new CmsManager(State.Identity(null, appId), true, false, Log)
                .DeleteQueryIfNotUsedByView(id, Log);

        [HttpPost]
        public bool Import(EntityImportDto args) => new QueryApi(Log).Import(args);

    }
}