using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using ToSic.Eav;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.WebApi
{
    public class ImportExportController : DnnApiController
    {
        private App app;

        private ZipExport zipExport;

        private ZipImport zipImport;

        private XmlImport xmlImport;


        private void InitController(int appId, int zoneId)
        {
            app = Environment.Dnn7.Factory.App(appId) as App;

            zipExport = new ZipExport(zoneId, appId);
            zipImport = new ZipImport(zoneId, appId, app.OwnerPortalSettings.UserInfo.IsSuperUser);

            xmlImport = new XmlImport(app.OwnerPortalSettings.DefaultLanguage, Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);
        }

        private void EnsureUserIsAdmin()
        {
            if (!app.OwnerPortalSettings.UserInfo.Roles.Contains("Administrators"))
                throw new AuthenticationException();
        }


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetAppInfo(int appId, int zoneId)
        {
            InitController(appId, zoneId);

            var info = new
            {
                Name    = app.Name,
                Guid    = app.AppGuid,
                Version = GetAppVersion(),
                EntitiesCount   = GetEntities().Count(),
                LanguagesCount  = GetLanguages().Count(),
                TemplatesCount  = GetTemplates().Count(),
                HasRazorTemplates = GetRazorTemplates().Count() > 0,
                HasTokenTemplates = GetTokenTemplates().Count() > 0,
                FilesCount        = GetExportFiles().Count(),
                TransferableFilesCount = GetTransferableExportFiles().Count()
            };
            return info;
        }
        
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetContentInfo(int appId, int zoneId, string scope)
        {
            InitController(appId, zoneId);

            var contentTypes = GetContentTypes(scope);
            var entities = GetEntities();
            var templates = GetTemplates();
            var dimensions = new string[] { app.OwnerPortalSettings.CultureCode };

            return new
            {
                ContentTypes = contentTypes.Select(c => new
                {
                    Id = c.AttributeSetId,
                    Name = c.Name,
                    StaticName = c.StaticName,
                    Templates = templates.Where(t => t.ContentTypeStaticName == c.StaticName).Select(t => new
                    {
                        Id = t.TemplateId,
                        Name = t.Name
                    }),
                    Entities = entities.Where(e => e.Value.Type.AttributeSetId == c.AttributeSetId).Select(e => new DynamicEntity(e.Value, dimensions, null).ToDictionary())
                }),
                TemplatesWithoutContentTypes = templates.Where(t => !string.IsNullOrEmpty(t.ContentTypeStaticName)).Select(t => new
                {
                    Id   = t.TemplateId,
                    Name = t.Name
                })
            };
        }

        [HttpGet]
        public HttpResponseMessage ExportApp(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            InitController(appId, zoneId);
            EnsureUserIsAdmin();

            var fileName = string.Format("2scxApp_{0}_{1}.zip", GetAppNameWithoutSpecialChars(), GetAppVersion());
            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {
                var fileBytes = fileStream.ToArray();
                return GetAttachmentHttpResponseMessage(fileName, "application/octet-stream", new MemoryStream(fileBytes));
            }
        }

        [HttpGet]
        public HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            InitController(appId, zoneId);
            EnsureUserIsAdmin();

            var fileName = string.Format("2scxContentExport_{0}_{1}.xml", GetAppNameWithoutSpecialChars(), GetAppVersion());
            var fileXml = new XmlExporter
            (
                zoneId, 
                appId, 
                false,
                contentTypeIdsString == null ? new string[0] : contentTypeIdsString.Split(';'),
                entityIdsString == null ? new string[0] : entityIdsString.Split(';')
            ).GenerateNiceXml();
            return GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }


        public class ImportArgs
        {
            public int AppId;

            public int ZoneId;

            public string FileName;

            public string FileData;
        }

        public class ImportResult
        {
            public bool Succeeded;

            public List<ExportImportMessage> Messages;

            public ImportResult()
            {
                Messages = new List<ExportImportMessage>();
            }
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportApp(ImportArgs args)
        {
            InitController(args.AppId, args.ZoneId);

            return null;
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportContent(ImportArgs args)
        {
            InitController(args.AppId, args.ZoneId);

            var result = new ImportResult();
            using (var fileStream = new MemoryStream(GetArrayFromBase64Url(args.FileData)))
            {
                if (args.FileName.EndsWith(".zip"))
                {   // ZIP
                    result.Succeeded = zipImport.ImportZip(fileStream, HttpContext.Current.Server, app.OwnerPortalSettings, result.Messages);
                }
                else
                {   // XML
                    using (var fileStreamReader = new StreamReader(fileStream))
                    {
                        var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                        result.Succeeded = xmlImport.ImportXml(app.ZoneId, app.AppId, xmlDocument);
                        result.Messages = xmlImport.ImportLog;
                    }
                }
            }
            return result;
        }



        #region Helpers
        private IDictionary<int, IEntity> GetEntities()
        {
            return DataSource.GetInitialDataSource(app.ZoneId, app.AppId).List;
        }

        private IEnumerable<ZoneHelpers.CulturesWithActiveState> GetLanguages()
        {
            return ZoneHelpers.GetCulturesWithActiveState(app.OwnerPortalSettings.PortalId, app.ZoneId).Where(c => c.Active);
        }

        private IEnumerable<IContentType> GetContentTypes(string scope)
        {
            return app.TemplateManager.GetAvailableContentTypes(scope, true);
        }

        private IEnumerable<Template> GetTemplates()
        {
            return app.TemplateManager.GetAllTemplates();
        }

        private IEnumerable<Template> GetRazorTemplates()
        {
            return app.TemplateManager.GetAllTemplates().Where(t => t.IsRazor);
        }

        private IEnumerable<Template> GetTokenTemplates()
        {
            return app.TemplateManager.GetAllTemplates().Where(t => !t.IsRazor);
        }

        private IEnumerable<string> GetExportFiles()
        {
            return zipExport.FileManager.AllFiles;
        }

        private IEnumerable<string> GetTransferableExportFiles()
        {
            return zipExport.FileManager.AllTransferableFiles;
        }

        private string GetAppVersion()
        {
            return app.Configuration == null ? "" : app.Configuration.Version;
        }

        private string GetAppNameWithoutSpecialChars()
        {
            return Regex.Replace(app.Name, "[^a-zA-Z0-9-_]", "");
        }


        private byte[] GetArrayFromBase64Url(string base64Url)
        {
            var base64Data = Regex.Match(base64Url, @"data:(?<media>.+?)/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            // Correct transfer errors!
            base64Data = base64Data.Replace('-', '+').Replace('_', '/').Trim('=');
            var base64Padding = base64Data.Length % 4;
            if (base64Padding > 0)
            {
                base64Data += new string('=', 4 - base64Padding);
            }

            return Convert.FromBase64String(base64Data);
        }


        private HttpResponseMessage GetAttachmentHttpResponseMessage(string fileName, string fileType, Stream fileContent)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(fileContent);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(fileType);
            response.Content.Headers.ContentLength = fileContent.Length;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            return response;
        }

        private HttpResponseMessage GetAttachmentHttpResponseMessage(string fileName, string fileType, string fileContent)
        {
            var fileBytes = Encoding.UTF8.GetBytes(fileContent);
            return GetAttachmentHttpResponseMessage(fileName, fileType, new MemoryStream(fileBytes));
        }
        #endregion Helpers
    }
}