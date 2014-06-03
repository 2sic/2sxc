using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.GettingStarted
{
    [SupportedModules("2sxc,2sxc-app")]
    public class GettingStartedController : DnnApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage HelloWorld()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hello World!");
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage InstallPackage(PackageInstallInfo package)
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            var appId = SexyContent.GetAppIdFromModule(ActiveModule);

            var messages = new List<ExportImportMessage>();
            return new ZipImport(zoneId.Value, appId, user.IsSuperUser).ImportZipFromUrl(starterPackageUrl, messages);

            return Request.CreateResponse(HttpStatusCode.OK, "Installed " + package.package + " on " + zoneId + "-" + appId);

        }

        public class PackageInstallInfo
        {
            public string package { get; set; }
        }

    }
}