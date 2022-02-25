using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Admin.App;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    [AllowAnonymous] // All functions will check security internally, so assume no requirements
    public class AppQueryController : SxcApiControllerBase<AppQueryControllerReal>, IAppQueryController
    {
        public AppQueryController() : base(AppQueryControllerReal.LogSuffix) { }

        // GET is separated from POST to solve HttpResponseException that happens when
        // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

        [HttpGet]
        public IDictionary<string, IEnumerable<EavLightEntity>> Query([FromUri] string name,
            [FromUri] int? appId = null,
            [FromUri] string stream = null,
            [FromUri] bool includeGuid = false
        ) => Real.Query(name, appId, stream, includeGuid);

        [HttpPost]
        public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost([FromUri] string name,
            [FromBody] QueryParameters more,
            [FromUri] int? appId = null,
            [FromUri] string stream = null,
            [FromUri] bool includeGuid = false
        ) => Real.QueryPost(name, more, appId, stream, includeGuid);

        [HttpGet] 
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
            [FromUri] string appPath,
            [FromUri] string name,
            [FromUri] string stream = null
        ) => Real.PublicQuery(appPath, name, stream);

        [HttpPost]
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(
            [FromUri] string appPath,
            [FromUri] string name,
            [FromBody] QueryParameters more,
            [FromUri] string stream = null
        ) => Real.PublicQueryPost(appPath, name, more, stream);
    }
}