using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Xml.Linq;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Cms
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    public class ImportExportController : DnnApiControllerWithFixes, IImportExportController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sIExC");
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public dynamic GetAppInfo(int appId, int zoneId)
        {
            Log.Add($"get app info for app:{appId} and zone:{zoneId}");
            var currentApp = SxcAppForWebApi.AppBasedOnUserPermissions(zoneId, appId, UserInfo, Log);

            var zipExport = new ZipExport(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, Log);
            var cultCount = Env.ZoneMapper
                .CulturesWithState(currentApp.Tenant.Id, currentApp.ZoneId)
                .Count(c => c.Active);

            var cms = new CmsRuntime(currentApp, Log, true, false);

            return new
            {
                currentApp.Name,
                Guid = currentApp.AppGuid,
                Version = currentApp.VersionSafe(),
                EntitiesCount = cms.Entities.All.Count(),
                LanguagesCount = cultCount,
                TemplatesCount = cms.Views.GetAll().Count(),
                HasRazorTemplates = cms.Views.GetRazor().Any(),
                HasTokenTemplates = cms.Views.GetToken().Any(),
                FilesCount = zipExport.FileManager.AllFiles.Count(),
                TransferableFilesCount = zipExport.FileManager.AllTransferableFiles.Count()
            };
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public dynamic GetContentInfo(int appId, int zoneId, string scope)
        {
            Log.Add($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{UserInfo.IsSuperUser}");
            var currentApp = SxcAppForWebApi.AppBasedOnUserPermissions(zoneId,appId, UserInfo, Log);// AppWithRestrictedZoneChange(appId, zoneId);

            var cms = new CmsRuntime(currentApp, Log, true, false);
            var contentTypes = cms.ContentTypes.FromScope(scope);
            var entities = cms.Entities.All ;
            var templates = cms.Views.GetAll();

            return new
            {
                ContentTypes = contentTypes.Select(c => new
                {
                    Id = c.ContentTypeId,
                    c.Name,
                    c.StaticName,
                    Templates = templates.Where(t => t.ContentType == c.StaticName).Select(t => new
                    {
                        t.Id,
                        t.Name
                    }),
                    Entities = entities
                        .Where(e => e.Type.ContentTypeId == c.ContentTypeId)
                        .Select(e => new
                        {
                            Title = e.GetBestTitle(),
                            Id = e.EntityId
                        })
                }),
                TemplatesWithoutContentTypes = templates.Where(t => !string.IsNullOrEmpty(t.ContentType)).Select(t => new
                {
                    t.Id,
                    t.Name
                })
            };
        }



        [HttpGet]
        public HttpResponseMessage ExportApp(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            Log.Add($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
            EnsureUserIsAdmin(); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var currentApp = SxcAppForWebApi.AppBasedOnUserPermissions(zoneId, appId, UserInfo, Log);// AppWithRestrictedZoneChange(appId, zoneId);

            var zipExport = new ZipExport(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, Log);
            var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

            var fileName =
                $"2sxcApp_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}{addOnWhenContainingContent}.zip";
            Log.Add($"file name:{fileName}");
            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {
                var fileBytes = fileStream.ToArray();
                Log.Add("will stream so many bytes:" + fileBytes.Length);
                return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "application/octet-stream", new MemoryStream(fileBytes));
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public bool ExportForVersionControl(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            Log.Add($"export for version control z#{zoneId}, a#{appId}, include:{includeContentGroups}, reset:{resetAppGuid}");
            EnsureUserIsAdmin();

            var currentApp = SxcAppForWebApi.AppBasedOnUserPermissions(zoneId, appId, UserInfo, Log);// AppWithRestrictedZoneChange(appId, zoneId);

            var zipExport = new ZipExport(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, Log);
            zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid);

            return true;
        }

        [HttpGet]
        public HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            Log.Add($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");
            EnsureUserIsAdmin();

            var currentApp = SxcAppForWebApi.AppBasedOnUserPermissions(zoneId, appId, UserInfo, Log);// AppWithRestrictedZoneChange(appId, zoneId);
            var appRuntime = new AppRuntime(currentApp, true, Log);

            var fileName = $"2sxcContentExport_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}.xml";
            var fileXml = new DnnXmlExporter().Init(zoneId, appId, appRuntime, false,
                contentTypeIdsString?.Split(';') ?? new string[0],
                entityIdsString?.Split(';') ?? new string[0],
                Log
            ).GenerateNiceXml();

            return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResult ImportApp()
        {
            Log.Add("import app start");
            var result = new ImportResult();

            var request = HttpContext.Current.Request;

            var zoneId  = int.Parse(request["ZoneId"]);
            if (request.Files.Count <= 0) return result;

            var name = request["Name"];
            if (!string.IsNullOrEmpty(name))
            {
                Log.Add($"new app name: {name}");
            }

            var helper = new DnnImportExportEnvironment(Log);
            try
            {
                var zipImport = new ZipImport(helper, zoneId, null, PortalSettings.UserInfo.IsSuperUser, Log);
                var temporaryDirectory = HttpContext.Current.Server.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory, Guid.NewGuid().ToString().Substring(0, 8)));

                // Increase script timeout to prevent timeouts
                HttpContext.Current.Server.ScriptTimeout = 300;
                result.Succeeded = zipImport.ImportZip(request.Files[0].InputStream, temporaryDirectory, name);
                result.Messages = helper.Messages;
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                result.Succeeded = false;
                result.Messages = helper.Messages;
            }
            return result;
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResult ImportContent()
        {
            Log.Add("import content start");
            var result = new ImportResult();

            var request = HttpContext.Current.Request;

            var allowSystemChanges = UserInfo.IsSuperUser;

            var appId = int.Parse(request["AppId"]);
            var zoneId = int.Parse(request["ZoneId"]);
            if (request.Files.Count <= 0) return result;

            var file = request.Files[0];
            if (file.FileName.EndsWith(".zip"))
            {   // ZIP
                try
                {
                    var env = new DnnImportExportEnvironment(Log);
                    var zipImport = new ZipImport(env, zoneId, appId, PortalSettings.UserInfo.IsSuperUser, Log);
                    // Increase script timeout to prevent timeouts
                    HttpContext.Current.Server.ScriptTimeout = 300;
                    var temporaryDirectory = HttpContext.Current.Server.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory, Guid.NewGuid().ToString()));
                    result.Succeeded = zipImport.ImportZip(file.InputStream, temporaryDirectory);
                    result.Messages = env.Messages;
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
            }
            else
            {   // XML
                using (var fileStreamReader = new StreamReader(file.InputStream))
                {
                    var xmlImport = new XmlImportWithFiles(Log, PortalSettings.DefaultLanguage, allowSystemChanges);
                    var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                    result.Succeeded = xmlImport.ImportXml(zoneId, appId, xmlDocument);
                    result.Messages = xmlImport.Messages;
                }
            }
            return result;
        }


        private void EnsureUserIsAdmin()
        {
            Log.Add("ensure user is admin");
            if (!PortalSettings.UserInfo.Roles.Contains(PortalSettings.AdministratorRoleName) && !PortalSettings.UserInfo.IsSuperUser)
                throw new AuthenticationException("user doesn't seem to be admin or super-user");
        }

        ///// <summary>
        ///// get the app, but only switch to another zone if the user is super-user
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="zoneId"></param>
        ///// <returns></returns>
        ///// <exception>
        ///// will throw exception if the app is in another zone, and the user is not a super-user
        ///// </exception>
        //private SxcAppWrapper AppWithRestrictedZoneChange(int appId, int zoneId) 
        //    => SxcAppWrapper.GetBasedOnUserPermissions(zoneId, appId, UserInfo);
    }
}