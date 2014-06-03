using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.GettingStarted
{
    [SupportedModules("2sxc,2sxc-app")]
    public class GettingStartedController : DnnApiController
    {
        //[AllowAnonymous]
        //[HttpGet]
        //public HttpResponseMessage HelloWorld()
        //{
        //    return Request.CreateResponse(HttpStatusCode.OK, "Hello World!");
        //}

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage InstallPackage([FromUri]string packageUrl)
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            var appId = SexyContent.GetAppIdFromModule(ActiveModule);

            // Install packages
            var messages = new List<ExportImportMessage>();
            var error = "";

            var success = true;
            try
            {
                success = new ZipImport(zoneId.Value, appId, PortalSettings.UserInfo.IsSuperUser).ImportZipFromUrl(
                    packageUrl, messages, ActiveModule.DesktopModule.ModuleName == "2sxc-app");
            }
            catch (Exception ex)
            {
                error = "Error while installing: " + ex.Message;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { success, messages, error });
        }

    }
}