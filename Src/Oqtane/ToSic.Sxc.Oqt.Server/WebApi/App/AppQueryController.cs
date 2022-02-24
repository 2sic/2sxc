using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.App;

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
    [Route(WebApiConstants.AppRootNoLanguage)]
    [Route(WebApiConstants.AppRootPathOrLang)]
    [Route(WebApiConstants.AppRootPathNdLang)]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/app/")]

    //[ApiController]

    //[AllowAnonymous]
    [Produces("application/json")]
    public class AppQueryController : OqtStatefulControllerBase<DummyControllerReal>, IAppQueryController
    {
        public AppQueryController(Lazy<AppQuery> appQuery): base("Query")
        {
            _appQuery = appQuery;
        }
        private readonly Lazy<AppQuery> _appQuery;


        // GET is separated from POST to solve HttpResponseException that happens when
        // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

        [HttpGet("{appPath}/query/{name}")]
        [HttpGet("{appPath}/query/{name}/{stream}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery([FromRoute] string appPath,
            [FromRoute] string name,
            [FromRoute] string stream = null) => _appQuery.Value.Init(Log).PublicQuery(appPath, name, stream, null);

        [HttpPost("{appPath}/query/{name}")]
        [HttpPost("{appPath}/query/{name}/{stream}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost([FromRoute] string appPath,
            [FromRoute] string name,
            QueryParameters more,
            [FromRoute] string stream = null) => _appQuery.Value.Init(Log).PublicQuery(appPath, name, stream, more);

        [HttpGet("auto/query/{name}")]
        [HttpGet("auto/query/{name}/{stream?}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> Query([FromRoute] string name,
            [FromQuery] bool includeGuid = false,
            [FromRoute] string stream = null,
            [FromQuery] int? appId = null) => _appQuery.Value.Init(Log).Query(appId, name, includeGuid, stream, null);

        [HttpPost("auto/query/{name}")]
        [HttpPost("auto/query/{name}/{stream?}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost([FromRoute] string name,
            QueryParameters more,
            [FromQuery] bool includeGuid = false,
            [FromRoute] string stream = null,
            [FromQuery] int? appId = null) => _appQuery.Value.Init(Log).Query(appId, name, includeGuid, stream, more);
    }
}
