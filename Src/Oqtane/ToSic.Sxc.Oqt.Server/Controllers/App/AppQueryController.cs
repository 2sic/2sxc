using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.PublicApi;

// TODO: #MissingFeature
// 1. Query from context / header
// 2. GetContext using current context
// 3. Verify that additional query params would affect query results

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // Release routes
    [Route(WebApiConstants.AppRoot)]
    [Route(WebApiConstants.AppRoot2)]
    [Route(WebApiConstants.AppRoot3)]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/app/")]

    [ApiController]
    public class AppQueryController : OqtStatefulControllerBase, IAppQueryController
    {
        private readonly Lazy<AppQuery> _appQuery;

        #region DI / Constructor
        protected override string HistoryLogName => "App.AppQry";

        public AppQueryController(StatefulControllerDependencies dependencies, Lazy<AppQuery> appQuery) : base(dependencies)
        {
            _appQuery = appQuery;
        }

        #endregion

        [HttpGet("{appPath}/query/{name}/{stream?}")]
        [HttpPost("{appPath}/query/{name}/{stream?}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(
            [FromRoute] string appPath,
            [FromRoute] string name,
            AppQueryParameters more,
            [FromRoute] string stream = null
        ) => _appQuery.Value.Init(Log).PublicQuery(appPath, name, stream, more);

        [HttpGet("auto/query/{name}/{stream?}")]
        [HttpPost("auto/query/{name}/{stream?}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(
            [FromRoute] string name,
            AppQueryParameters more,
            [FromQuery] bool includeGuid = false,
            [FromRoute] string stream = null,
            [FromQuery] int? appId = null
        ) => _appQuery.Value.Init(Log).Query(appId, name, includeGuid, stream, more);
    }
}
