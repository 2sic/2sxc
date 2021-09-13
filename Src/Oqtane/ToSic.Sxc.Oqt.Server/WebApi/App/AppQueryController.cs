using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.ImportExport.Json.V1;

using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.PublicApi;

// TODO: #MissingFeature
// 1. Query from context / header
// 2. GetContext using current context
// 3. Verify that additional query params would affect query results

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    
    // Release routes
    [Route(WebApiConstants.AppRoot)]
    [Route(WebApiConstants.AppRoot2)]
    [Route(WebApiConstants.AppRoot3)]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/app/")]

    //[ApiController]

    //[AllowAnonymous]
    [Produces("application/json")]
    public class AppQueryController : OqtStatefulControllerBase, IAppQueryController
    {
        private readonly Lazy<AppQuery> _appQuery;

        #region DI / Constructor
        protected override string HistoryLogName => "App.AppQry";

        public AppQueryController(Lazy<AppQuery> appQuery)
        {
            _appQuery = appQuery;
        }

        #endregion

        [HttpGet("{appPath}/query/{name}")]
        [HttpGet("{appPath}/query/{name}/{stream}")]
        [HttpPost("{appPath}/query/{name}")]
        [HttpPost("{appPath}/query/{name}/{stream}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery([FromRoute] string appPath,
            [FromRoute] string name,
            AppQueryParameters more,
            [FromRoute] string stream = null) => _appQuery.Value.Init(Log).PublicQuery(appPath, name, stream, more);

        [HttpGet("auto/query/{name}")]
        [HttpGet("auto/query/{name}/{stream?}")]
        [HttpPost("auto/query/{name}")]
        [HttpPost("auto/query/{name}/{stream?}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> Query([FromRoute] string name,
            AppQueryParameters more,
            [FromQuery] bool includeGuid = false,
            [FromRoute] string stream = null,
            [FromQuery] int? appId = null) => _appQuery.Value.Init(Log).Query(appId, name, includeGuid, stream, more);
    }
}
