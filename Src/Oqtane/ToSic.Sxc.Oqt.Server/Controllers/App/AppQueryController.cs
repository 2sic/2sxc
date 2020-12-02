using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;

// TODO: #MissingFeature
// 1. Query from context / header
// 2. GetContext using current context
// 3. Verify that additional query params would affect query results

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/app/")]
    [ApiController]
    public class AppQueryController : OqtStatefulControllerBase
    {
        private readonly AppQuery _appQuery;

        #region DI / Constructor
        protected override string HistoryLogName => "App.AppQry";

        public AppQueryController(StatefulControllerDependencies dependencies, AppQuery appQuery) : base(dependencies)
        {
            _appQuery = appQuery;
        }

        #endregion
        
        [HttpGet("{appPath}/query/{name}/{default}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(
            [FromRoute] string appPath,
            [FromRoute] string name,
            [FromQuery] string stream = null
        ) => _appQuery.Init(Log).PublicQuery(appPath, name, stream);

        [HttpGet("auto/query/{name}/{default}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(
            [FromRoute] string name,
            [FromQuery] bool includeGuid = false,
            [FromQuery] string stream = null,
            [FromQuery] int? appId = null
        ) => _appQuery.Init(Log).Query(GetContext(), name, includeGuid, stream, appId);
    }
}
