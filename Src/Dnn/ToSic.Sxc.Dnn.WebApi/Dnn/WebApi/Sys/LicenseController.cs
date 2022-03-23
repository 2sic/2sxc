using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Sys.Licenses;

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
        public IEnumerable<LicenseDto> Summary() => Real.Summary();

        /// <inheritdoc />
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool Upload()
        {
            PreventServerTimeout300();
            return Real.Upload(new HttpUploadedFile(Request, HttpContext.Current.Request));
        }
    }
}
