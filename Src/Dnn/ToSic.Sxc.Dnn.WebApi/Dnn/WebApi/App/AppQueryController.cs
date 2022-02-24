using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Admin.App;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    [AllowAnonymous]
    public class AppQueryController : SxcApiControllerBase<AppQueryControllerReal>, IAppQueryController
    {
        public AppQueryController() : base(AppQueryControllerReal.LogSuffix) { }

        // GET is separated from POST to solve HttpResponseException that happens when
        // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

        [HttpGet] 
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> Query([FromUri] string name,
            [FromUri] int? appId = null,
            [FromUri] string stream = null,
            [FromUri] bool includeGuid = false) => Real.Query(name, appId, stream, includeGuid);

        [HttpPost]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost([FromUri] string name,
            [FromBody] QueryParameters more,
            [FromUri] int? appId = null,
            [FromUri] string stream = null,
            [FromUri] bool includeGuid = false) => Real.QueryPost(name, more, appId, stream, includeGuid);

        [HttpGet] 
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
            [FromUri] string appPath,
            [FromUri] string name,
            [FromUri] string stream = null
        ) => Real.PublicQuery(appPath, name, stream);

        [HttpPost]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(
            [FromUri] string appPath,
            [FromUri] string name,
            [FromBody] QueryParameters more,
            [FromUri] string stream = null
        ) => Real.PublicQueryPost(appPath, name, more, stream);

    }
}