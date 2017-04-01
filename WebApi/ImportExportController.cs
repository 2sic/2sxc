using System;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi
{
    public class ImportExportController : DnnApiControllerWithFixes//DnnApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetAppInfo(int appId, int zoneId)
        {
            var appWrapper = (UserInfo.IsSuperUser)
                ? new SxcAppWrapper(zoneId, appId)  // only super-user may switch to another zone for export
                : new SxcAppWrapper(appId);

            var zipExport = new ZipExport(zoneId, appId);
            var cultCount = new Environment.Environment().ZoneMapper
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
            var appWrapper = (UserInfo.IsSuperUser)
                ? new SxcAppWrapper(zoneId, appId)  // only super-user may switch to another zone for export
                : new SxcAppWrapper(appId);

            var contentTypes = appWrapper.GetContentTypes(scope);
            var entities = appWrapper.GetEntities();
            var templates = appWrapper.GetTemplates();
            //var dimensions = new[] { appWrapper.GetCultureCode() };

            return new
            {
                ContentTypes = contentTypes.Select(c => new
                {
                    Id = c.AttributeSetId,
                    c.Name,
                    c.StaticName,
                    Templates = templates.Where(t => t.ContentTypeStaticName == c.StaticName).Select(t => new
                    {
                        Id = t.TemplateId,
                        t.Name
                    }),
                    Entities = entities
                        .Where(e => e.Value.Type.AttributeSetId == c.AttributeSetId)
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
            EnsureUserIsAdmin();

            var appWrapper = (UserInfo.IsSuperUser)
                ? new SxcAppWrapper(zoneId, appId)  // only super-user may switch to another zone for export
                : new SxcAppWrapper(appId);

            var zipExport = new ZipExport(zoneId, appId);
            var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

            var fileName =
                $"2sxcApp_{appWrapper.GetNameWithoutSpecialChars()}_{appWrapper.GetVersion()}{addOnWhenContainingContent}.zip";
            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {
                var fileBytes = fileStream.ToArray();
                return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "application/octet-stream", new MemoryStream(fileBytes));
            }
        }

        [HttpGet]
        public bool ExportForVersionControl(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            EnsureUserIsAdmin();

            // ReSharper disable once UnusedVariable
            var appWrapper = (UserInfo.IsSuperUser)
                ? new SxcAppWrapper(zoneId, appId)  // only super-user may switch to another zone for export
                : new SxcAppWrapper(appId);

            var zipExport = new ZipExport(zoneId, appId);
            zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid);

            return true;
        }

        [HttpGet]
        public HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            EnsureUserIsAdmin();


            var appWrapper = (UserInfo.IsSuperUser)
                ? new SxcAppWrapper(zoneId, appId)  // only super-user may switch to another zone for export
                : new SxcAppWrapper(appId);

            var fileName = $"2sxcContentExport_{appWrapper.GetNameWithoutSpecialChars()}_{appWrapper.GetVersion()}.xml";
            var fileXml = new XmlExporter
            (
                zoneId,
                appId,
                false,
                contentTypeIdsString?.Split(';') ?? new string[0],
                entityIdsString?.Split(';') ?? new string[0]
            ).GenerateNiceXml();

            return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportApp()
        {
            var result = new ImportResult();

            var request = HttpContext.Current.Request;

            //var appId   = int.Parse(request["AppId"]);
            var zoneId  = int.Parse(request["ZoneId"]);
            if (request.Files.Count > 0)
            {
                var zipImport = new ZipImport(zoneId, null, PortalSettings.UserInfo.IsSuperUser);
                result.Succeeded = zipImport.ImportApp(request.Files[0].InputStream, HttpContext.Current.Server, PortalSettings, result.Messages);
            }
            return result;
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportContent()
        {
            var result = new ImportResult();

            var request = HttpContext.Current.Request;

            var allowSystemChanges = UserInfo.IsSuperUser;

            var appId = int.Parse(request["AppId"]);
            var zoneId = int.Parse(request["ZoneId"]);
            if (request.Files.Count > 0)
            {
                var file = request.Files[0];
                if (file.FileName.EndsWith(".zip"))
                {   // ZIP
                    var zipImport = new ZipImport(zoneId, appId, PortalSettings.UserInfo.IsSuperUser);
                    result.Succeeded = zipImport.ImportZip(file.InputStream, HttpContext.Current.Server, PortalSettings, result.Messages);
                }
                else
                {   // XML
                    using (var fileStreamReader = new StreamReader(file.InputStream))
                    {
                        var xmlImport = new XmlImport(PortalSettings.DefaultLanguage, UserIdentity.CurrentUserIdentityToken, allowSystemChanges);
                        var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                        result.Succeeded = xmlImport.ImportXml(zoneId, appId, xmlDocument);
                        result.Messages = xmlImport.ImportLog;
                    }
                }
            }
            return result;
        }


        private void EnsureUserIsAdmin()
        {
            if (!PortalSettings.UserInfo.Roles.Contains(PortalSettings.AdministratorRoleName) && !PortalSettings.UserInfo.IsSuperUser) // todo: add to 8.5
                throw new AuthenticationException("user doesn't seem to be admin or super-user");
        }
    }
}