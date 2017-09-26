using System;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Xml.Linq;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.WebApi.Dnn;
using ToSic.SexyContent.WebApi.ImportExport;

namespace ToSic.SexyContent.WebApi
{
    public class ImportExportController : DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sIExC");
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetAppInfo(int appId, int zoneId)
        {
            Log.Add($"get app info for app:{appId} and zone:{zoneId}");
            var appWrapper = AppBasedOnUserPermissions(appId, zoneId);

            var zipExport = new ZipExport(zoneId, appId, appWrapper.App.Folder, appWrapper.App.PhysicalPath, Log);
            var cultCount = /*new Environment.DnnEnvironment(Log)*/Env.ZoneMapper
                .CulturesWithState(appWrapper.App.OwnerPortalSettings.PortalId, appWrapper.App.ZoneId)
                .Count(c => c.Active);
            return new
            {
                appWrapper.App.Name,
                Guid = appWrapper.App.AppGuid,
                Version = appWrapper.GetVersion(),
                EntitiesCount = appWrapper.GetEntities().Count,
                LanguagesCount = cultCount,// 2017-04-01 2dm from: appWrapper.GetActiveLanguages().Count(),
                TemplatesCount = appWrapper.GetTemplates().Count(),
                HasRazorTemplates = appWrapper.GetRazorTemplates().Any(),
                HasTokenTemplates = appWrapper.GetTokenTemplates().Any(),
                FilesCount = zipExport.FileManager.AllFiles.Count(),
                TransferableFilesCount = zipExport.FileManager.AllTransferableFiles.Count()
            };
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetContentInfo(int appId, int zoneId, string scope)
        {
            Log.Add($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{UserInfo.IsSuperUser}");
            var appWrapper = AppBasedOnUserPermissions(appId, zoneId);

            var contentTypes = new AppRuntime(appWrapper.App).ContentTypes.FromScope(scope);
            var entities = appWrapper.GetEntities();
            var templates = appWrapper.GetTemplates();

            return new
            {
                ContentTypes = contentTypes.Select(c => new
                {
                    Id = c.ContentTypeId,
                    c.Name,
                    c.StaticName,
                    Templates = templates.Where(t => t.ContentTypeStaticName == c.StaticName).Select(t => new
                    {
                        Id = t.TemplateId,
                        t.Name
                    }),
                    Entities = entities
                        .Where(e => e.Value.Type.ContentTypeId == c.ContentTypeId)
                        .Select(e => new
                        {
                            Title = e.Value.GetBestTitle(),
                            Id = e.Value.EntityId
                        })
                }),
                TemplatesWithoutContentTypes = templates.Where(t => !string.IsNullOrEmpty(t.ContentTypeStaticName)).Select(t => new
                {
                    Id = t.TemplateId,
                    t.Name
                })
            };
        }



        [HttpGet]
        public HttpResponseMessage ExportApp(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            Log.Add($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
            EnsureUserIsAdmin(); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var appWrapper = AppBasedOnUserPermissions(appId, zoneId);

            var zipExport = new ZipExport(zoneId, appId, appWrapper.App.Folder, appWrapper.App.PhysicalPath, Log);
            var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

            var fileName =
                $"2sxcApp_{appWrapper.GetNameWithoutSpecialChars()}_{appWrapper.GetVersion()}{addOnWhenContainingContent}.zip";
            Log.Add($"file name:{fileName}");
            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {
                var fileBytes = fileStream.ToArray();
                Log.Add("will stream so many bytes:" + fileBytes.Length);
                return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "application/octet-stream", new MemoryStream(fileBytes));
            }
        }

        [HttpGet]
        public bool ExportForVersionControl(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            Log.Add($"export for version control z#{zoneId}, a#{appId}, include:{includeContentGroups}, reset:{resetAppGuid}");
            EnsureUserIsAdmin();

            // ReSharper disable once UnusedVariable
            var appWrapper = AppBasedOnUserPermissions(appId, zoneId);

            var zipExport = new ZipExport(zoneId, appId, appWrapper.App.Folder, appWrapper.App.PhysicalPath, Log);
            zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid);

            return true;
        }

        [HttpGet]
        public HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            Log.Add($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");
            EnsureUserIsAdmin();

            var appWrapper = AppBasedOnUserPermissions(appId, zoneId);

            var fileName = $"2sxcContentExport_{appWrapper.GetNameWithoutSpecialChars()}_{appWrapper.GetVersion()}.xml";
            var fileXml = new ToSxcXmlExporter().Init(zoneId, appId, false,
                contentTypeIdsString?.Split(';') ?? new string[0],
                entityIdsString?.Split(';') ?? new string[0],
                Log
            ).GenerateNiceXml();

            return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportApp()
        {
            Log.Add("import app start");
            var result = new ImportResult();

            var request = HttpContext.Current.Request;

            var zoneId  = int.Parse(request["ZoneId"]);
            if (request.Files.Count <= 0) return result;

            var helper = new ImportExportEnvironment(Log);
            try
            {
                var zipImport = new ZipImport(helper, zoneId, null, PortalSettings.UserInfo.IsSuperUser, Log);
                var temporaryDirectory = HttpContext.Current.Server.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory, Guid.NewGuid().ToString().Substring(0, 8)));

                // Increase script timeout to prevent timeouts
                HttpContext.Current.Server.ScriptTimeout = 300;
                result.Succeeded = zipImport.ImportZip(request.Files[0].InputStream, temporaryDirectory);// HttpContext.Current.Server);
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
                    var env = new ImportExportEnvironment(Log);
                    var zipImport = new ZipImport(env, zoneId, appId, PortalSettings.UserInfo.IsSuperUser, Log);
                    // Increase script timeout to prevent timeouts
                    HttpContext.Current.Server.ScriptTimeout = 300;
                    var temporaryDirectory = HttpContext.Current.Server.MapPath(Path.Combine(Eav.ImportExport.Settings.TemporaryDirectory, Guid.NewGuid().ToString()));
                    result.Succeeded = zipImport.ImportZip(file.InputStream, temporaryDirectory);// HttpContext.Current.Server); // , /*PortalSettings,*/ env.Messages);
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
                    var xmlImport = new XmlImportWithFiles(Log, PortalSettings.DefaultLanguage, /*UserIdentity.CurrentUserIdentityToken,*/ allowSystemChanges);
                    var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                    result.Succeeded = xmlImport.ImportXml(zoneId, appId, xmlDocument);
                    result.Messages = xmlImport.ImportLog;
                }
            }
            return result;
        }


        private void EnsureUserIsAdmin()
        {
            Log.Add("ensure user is admin");
            if (!PortalSettings.UserInfo.Roles.Contains(PortalSettings.AdministratorRoleName) && !PortalSettings.UserInfo.IsSuperUser) // todo: add to 8.5
                throw new AuthenticationException("user doesn't seem to be admin or super-user");
        }

        /// <summary>
        /// get the app, but only switch to another zone if the user is super-user
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        /// <exception>
        /// will throw exception if the app is in another zone, and the user is not a super-user
        /// </exception>
        private SxcAppWrapper AppBasedOnUserPermissions(int appId, int zoneId)
        {
            var appWrapper = UserInfo.IsSuperUser
                ? new SxcAppWrapper(zoneId, appId) // only super-user may switch to another zone for export
                : new SxcAppWrapper(appId, false);
            return appWrapper;
        }

    }
}