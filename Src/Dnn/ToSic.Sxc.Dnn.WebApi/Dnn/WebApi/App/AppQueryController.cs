using System.Collections.Generic;
using System.Web.Http;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.PublicApi;

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

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query([FromUri] string name, [FromUri] bool includeGuid = false, [FromUri] string stream = null, [FromUri] int? appId = null) 
            => GetService<AppQuery>().Init(Log).Query(appId, name, includeGuid, stream);

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery([FromUri] string appPath, [FromUri] string name, [FromUri] string stream = null) 
            => GetService<AppQuery>().Init(Log).PublicQuery(appPath, name, stream);

    }
}