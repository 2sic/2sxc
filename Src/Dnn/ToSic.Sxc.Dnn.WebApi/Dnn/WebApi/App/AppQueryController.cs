using System.Collections.Generic;
using System.Web.Http;
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
    public class AppQueryController : SxcApiControllerBase
    {
        #region Constructor / DI
        protected override string HistoryLogName => "Api.ApQrCt";
        #endregion

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> Query([FromUri] string name, [FromUri] bool includeGuid = false, [FromUri] string stream = null, [FromUri] int? appId = null) 
            => Eav.Factory.Resolve<AppQuery>().Init(Log).Query(GetContext(), GetBlock(), GetBlock().App, name, includeGuid, stream, appId);

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery([FromUri] string appPath, [FromUri] string name, [FromUri] string stream = null) 
            => Eav.Factory.Resolve<AppQuery>().Init(Log).PublicQuery(GetContext(), appPath, name, stream, GetBlock());

    }
}