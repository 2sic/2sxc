using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi;
using WebApiConstants = ToSic.Sxc.Oqt.Shared.WebApiConstants;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
    /// </summary>
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]

    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Admin}")]
    public class QueryController : OqtStatefulControllerBase, IQueryController
    {
        private readonly QueryBackend _queryLazy;
        protected override string HistoryLogName => "Api.Query";

        public QueryController(QueryBackend queryLazy) => _queryLazy = queryLazy;

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
        public QueryDefinitionDto Get(int appId, int? id = null) 
            => _queryLazy.Init(appId, Log).Definition(appId, id);

        /// <summary>
        /// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
        /// </summary>
        [HttpGet]
        public IEnumerable<DataSourceDto> DataSources()
            => _queryLazy.Init(0, Log).DataSources();

        /// <summary>
        /// Save Pipeline
        /// </summary>
        /// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
        /// <param name="appId">AppId this Pipeline belongs to</param>
        /// <param name="id">PipelineEntityId</param>
        [HttpPost]
        public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id)
            => _queryLazy.Init(appId, Log).Save(data, appId, id);


        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        [HttpGet]
        public QueryRunDto Run(int appId, int id, int top = 0) 
            => _queryLazy.Init(appId, Log).RunDev(appId, id, top);

        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        [HttpGet]
        public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25) 
            => _queryLazy.Init(appId, Log).DebugStream(appId, id, @from, @out, top);

        /// <summary>
        /// Clone a Pipeline with all DataSources and their configurations
        /// </summary>
        [HttpGet]
        public void Clone(int appId, int id)
            => _queryLazy.Init(appId, Log).Clone(appId, id);


        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations.
        /// Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        [HttpDelete]
        public bool Delete(int appId, int id)
            => _queryLazy.Init(appId, Log).DeleteIfUnused(appId, id);

        [HttpPost]
        public bool Import(EntityImportDto args) 
            => _queryLazy.Init(args.AppId, Log).Import(args);

    }
}