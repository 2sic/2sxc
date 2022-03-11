using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Sys.Licenses;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public class LicenseController : DnnApiControllerWithFixes<LicenseControllerReal>, ILicenseController
    {
        public LicenseController() : base("License") { }

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup => "web-api.license";

        [HttpGet]
        public IEnumerable<LicenseDto> Summary() => Real.Summary();

    }
}
