using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Admin.Query;

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

    public class QueryController : OqtStatefulControllerBase<QueryControllerReal>, IQueryController
    {
        public QueryController() : base(QueryControllerReal.LogSuffix) { }

        [HttpGet] public QueryDefinitionDto Get(int appId, int? id = null) => Real.Init(appId).Get(appId, id);

        [HttpGet] public IEnumerable<DataSourceDto> DataSources(int zoneId, int appId) => Real.Init(appId).DataSources();

        [HttpPost] public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id)
            => Real.Init(appId).Save(data, appId, id);

        [HttpGet] public QueryRunDto Run(int appId, int id, int top = 0) => Real.Init(appId).RunDev(appId, id, top);

        [HttpGet] public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25) 
            => Real.Init(appId).DebugStream(appId, id, @from, @out, top);

        [HttpGet] public void Clone(int appId, int id) => Real.Init(appId).Clone(appId, id);

        [HttpDelete] public bool Delete(int appId, int id) => Real.Init(appId).DeleteIfUnused(appId, id);

        [HttpPost] public bool Import(EntityImportDto args) => Real.Init(args.AppId).Import(args);
    }
}