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

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage InstallPackage(string packageUrl)
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            var appId = SexyContent.GetAppIdFromModule(ActiveModule);
            bool success;

            // Install package
            var messages = new List<ExportImportMessage>();
            try
            {
                success = new ZipImport(zoneId.Value, appId, PortalSettings.UserInfo.IsSuperUser).ImportZipFromUrl(
                    packageUrl, messages, ActiveModule.DesktopModule.ModuleName == "2sxc-app");
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while installing the app: " + ex.Message, ex);
            }
            
            return Request.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, new { success, messages });
        }

    }
}