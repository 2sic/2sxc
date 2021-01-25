using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.PublicApi;
using NotImplementedException = System.NotImplementedException;

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


        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(string name, bool includeGuid = false, string stream = null, int? appId = null)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{name}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(
            string appPath,
            string name,
            [FromQuery] string stream = null
        ) => HttpContext.RequestServices.Build<AppQuery>().Init(Log).PublicQuery(appPath, name, stream);

    }
}
