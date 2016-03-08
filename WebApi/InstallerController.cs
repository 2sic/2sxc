using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.WebApi
{
    [SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    public class InstallerController : DnnApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage InstallPackage(string packageUrl)
        {
            var zoneId = ZoneHelpers.GetZoneID(ActiveModule.PortalID);
            var appId = AppHelpers.GetAppIdFromModule(ActiveModule);
            bool success;

            // Install package
            var messages = new List<ExportImportMessage>();
            try
            {
                success = new ZipImport(zoneId.Value, appId, PortalSettings.UserInfo.IsSuperUser)
                    .ImportZipFromUrl(packageUrl, messages, ActiveModule.DesktopModule.ModuleName == "2sxc-app");
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                throw new Exception("An error occurred while installing the app: " + ex.Message, ex);
            }
            
            return Request.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, new { success, messages });
        }

    }
}