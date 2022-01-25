using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Query;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    [AllowAnonymous]
    public class AppQueryController : SxcApiControllerBase, IAppQueryController
    {
        #region Constructor / DI
        protected override string HistoryLogName => "Api.ApQrCt";
        #endregion

        // GET is separated from POST to solve HttpResponseException that happens when
        // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

        [HttpGet] 
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> Query([FromUri] string name,
            [FromUri] bool includeGuid = false,
            [FromUri] string stream = null,
            [FromUri] int? appId = null
        ) => GetService<AppQuery>().Init(Log).Query(appId, name, includeGuid, stream, null);

        [HttpPost]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost([FromUri] string name,
            [FromBody] QueryParameters more,
            [FromUri] bool includeGuid = false,
            [FromUri] string stream = null,
            [FromUri] int? appId = null
        ) => GetService<AppQuery>().Init(Log).Query(appId, name, includeGuid, stream, more);

        [HttpGet] 
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
            [FromUri] string appPath,
            [FromUri] string name,
            [FromUri] string stream = null
        ) => GetService<AppQuery>().Init(Log).PublicQuery(appPath, name, stream, null);

        [HttpPost]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(
            [FromUri] string appPath,
            [FromUri] string name,
            [FromBody] QueryParameters more,
            [FromUri] string stream = null
        ) => GetService<AppQuery>().Init(Log).PublicQuery(appPath, name, stream, more);

    }
}