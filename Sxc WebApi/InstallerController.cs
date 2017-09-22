using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.ImportExport;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi
{
    [SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    public class InstallerController : DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sInst");
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        //[ValidateAntiForgeryToken]
        public HttpResponseMessage InstallPackage(string packageUrl)
        {
            var zoneId = Env.ZoneMapper.GetZoneId(ActiveModule.PortalID);// ZoneHelpers.GetZoneId(ActiveModule.PortalID).Value;
            var appId = AppHelpers.GetAppIdFromModule(ActiveModule, zoneId);
            bool success;

            // Install package
            // var messages = new List<ExportImportMessage>();
            var helper = new ImportExportEnvironment();
            try
            {
                // Increase script timeout to prevent timeouts
                HttpContext.Current.Server.ScriptTimeout = 300;

                success = new ZipImport(helper, zoneId, appId, PortalSettings.UserInfo.IsSuperUser)
                    .ImportZipFromUrl(packageUrl, ActiveModule.DesktopModule.ModuleName == "2sxc-app");
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                throw new Exception("An error occurred while installing the app: " + ex.Message, ex);
            }
            
            return Request.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, new { success, helper.Messages });
        }

    }
}