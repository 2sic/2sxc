using System.Collections.Generic;
using System.Net;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Configuration.Licenses;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.ImportExport;
using ToSic.Sxc.WebApi.Licenses;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    public class LicenseController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.License";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.license";

        #region License

        /// <summary>
        /// Gives an array of License (sort by priority)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public IEnumerable<LicenseDto> Summary() => GetService<LicenseBackend>().Init(Log).Summary();

        #endregion
    }
}
