using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Admin.App;

// TODO: #MissingFeature
// 1. Query from context / header
// 2. GetContext using current context
// 3. Verify that additional query params would affect query results

namespace ToSic.Sxc.Mvc.WebApi.App
{
    [Route(WebApiConstants.WebApiRoot + "/app/{appPath}/query/")]
    [ApiController]
    public class AppQueryController : SxcStatefulControllerBase, IAppQueryController
    {
        #region DI / Constructor
        protected override string HistoryLogName => "App.AppQry";

        #endregion


        [HttpGet("{name}")]
        [HttpPost("{name}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(
            string name,
            AppQueryParameters more,
            bool includeGuid = false,
            string stream = null,
            int? appId = null
        ) => HttpContext.RequestServices.Build<AppQuery>().Init(Log).Query(appId, name, includeGuid, stream, more);

        [HttpGet("{name}")]
        [HttpPost("{name}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(
            string appPath,
            string name,
            AppQueryParameters more,
            [FromQuery] string stream = null
        ) => HttpContext.RequestServices.Build<AppQuery>().Init(Log).PublicQuery(appPath, name, stream, more);

    }
}
