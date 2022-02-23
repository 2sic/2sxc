using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Licenses;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public class LicenseController : DnnApiControllerWithFixes<LicenseControllerReal>
    {
        public LicenseController() : base("License") { }

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
        public IEnumerable<LicenseDto> Summary() => Real.Summary();

        #endregion
    }
}
