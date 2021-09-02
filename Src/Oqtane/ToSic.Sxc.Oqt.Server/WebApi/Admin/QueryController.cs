using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.LookUp;
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
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class QueryController : OqtStatefulControllerBase, IQueryController
    {
        private readonly Lazy<QueryBackend> _queryLazy;
        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly Lazy<AppConfigDelegate> _configProviderLazy;
        protected override string HistoryLogName => "Api.Query";

        public QueryController(Lazy<QueryBackend> queryLazy, Lazy<CmsManager> cmsManagerLazy, Lazy<AppConfigDelegate> configProviderLazy)
        {
            _queryLazy = queryLazy;
            _cmsManagerLazy = cmsManagerLazy;
            _configProviderLazy = configProviderLazy;
        }

        /// <summary>
        /// Get a Pipeline with DataSources
        /// </summary>
        [HttpGet]
        public QueryDefinitionDto Get(int appId, int? id = null) => _queryLazy.Value.Init(appId, Log).Definition(appId, id);

        /// <summary>
        /// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
        /// </summary>
        [HttpGet]
        public IEnumerable<DataSourceDto> DataSources() => _queryLazy.Value.Init(0, Log).DataSources();

        /// <summary>
        /// Save Pipeline
        /// </summary>
        /// <param name="data">JSON object { pipeline: pipeline, dataSources: dataSources }</param>
        /// <param name="appId">AppId this Pipeline belongs to</param>
        /// <param name="id">PipelineEntityId</param>
        [HttpPost]
        public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id)
            => _queryLazy.Value.Init(appId, Log).Save(data, appId, id);


        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        [HttpGet]
        public QueryRunDto Run(int appId, int id, int top = 0)
        {
            // todo: the first three lines should be in the QueryApi backend, but ATM that's still in EAV and is missing some objects
            var block = GetBlock();
            var context = GetContext();
            var config = _configProviderLazy.Value.Init(Log).GetConfigProviderForModule(context, block?.App, block);
            return _queryLazy.Value.Init(appId, Log).Run(appId, id, top, config);
        }

        /// <summary>
        /// Query the Result of a Pipeline using Test-Parameters
        /// </summary>
        [HttpGet]
        public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25)
        {
            // todo: the first three lines should be in the QueryApi backend, but ATM that's still in EAV and is missing some objects
            var block = GetBlock();
            var context = GetContext();
            var config = _configProviderLazy.Value.Init(Log).GetConfigProviderForModule(context, block?.App, block);
            return _queryLazy.Value.Init(appId, Log).DebugStream(appId, id, top, config, from, @out);
        }

        /// <summary>
        /// Clone a Pipeline with all DataSources and their configurations
        /// </summary>
        [HttpGet]
        public void Clone(int appId, int id) => _queryLazy.Value.Init(appId, Log).Clone(appId, id);


        /// <summary>
        /// Delete a Pipeline with the Pipeline Entity, Pipeline Parts and their Configurations. Stops if the if the Pipeline Entity has relationships to other Entities or is in use in a 2sxc-Template.
        /// </summary>
        [HttpDelete]
        public bool Delete(int appId, int id)
            => ((QueryBackend)_queryLazy.Value.Init(appId, Log)).DeleteIfUnused(appId, id);
            //=> _cmsManagerLazy.Value.Init(State.Identity(null, appId), true, Log)
        //    .DeleteQueryIfNotUsedByView(id, Log);

        [HttpPost]
        public bool Import(EntityImportDto args) => _queryLazy.Value.Init(args.AppId, Log).Import(args);

    }
}