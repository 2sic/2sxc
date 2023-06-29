using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using ToSic.Eav.WebApi.Adam;
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

        /// <inheritdoc />
        [HttpGet]
        public IEnumerable<LicenseDto> Summary() => SysHlp.Real.Summary();

        /// <inheritdoc />
        [HttpPost]
        [ValidateAntiForgeryToken]
        public LicenseFileResultDto Upload()
        {
            SysHlp.PreventServerTimeout300();
            return SysHlp.Real.Upload(new HttpUploadedFile(Request, HttpContext.Current.Request));
        }

        /// <inheritdoc />
        [HttpGet]
        public LicenseFileResultDto Retrieve()
        {
            SysHlp.PreventServerTimeout300();
            return SysHlp.Real.Retrieve();
        }
    }
}
