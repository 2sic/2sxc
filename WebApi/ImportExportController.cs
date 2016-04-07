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
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.WebApi
{
    public class ImportExportController : DnnApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetAppInfo(int appId, int zoneId)
        {
            var appWrapper = new SxcAppWrapper(appId);
            var zipExport = new ZipExport(zoneId, appId);

            return new
            {
                Name = appWrapper.App.Name,
                Guid = appWrapper.App.AppGuid,
                Version = appWrapper.GetVersion(),
                EntitiesCount = appWrapper.GetEntities().Count(),
                LanguagesCount = appWrapper.GetActiveLanguages().Count(),
                TemplatesCount = appWrapper.GetTemplates().Count(),
                HasRazorTemplates = appWrapper.GetRazorTemplates().Count() > 0,
                HasTokenTemplates = appWrapper.GetTokenTemplates().Count() > 0,
                FilesCount = zipExport.FileManager.AllFiles.Count(),
                TransferableFilesCount = zipExport.FileManager.AllTransferableFiles.Count()
            };
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public dynamic GetContentInfo(int appId, int zoneId, string scope)
        {
            var appWrapper = new SxcAppWrapper(appId);

            var contentTypes = appWrapper.GetContentTypes(scope);
            var entities = appWrapper.GetEntities();
            var templates = appWrapper.GetTemplates();
            var dimensions = new string[] { appWrapper.GetCultureCode() };

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
                    Id = t.TemplateId,
                    Name = t.Name
                })
            };
        }


        [HttpGet]
        public HttpResponseMessage ExportApp(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            EnsureUserIsAdmin();

            var appWrapper = new SxcAppWrapper(appId);
            var zipExport = new ZipExport(zoneId, appId);

            var fileName = string.Format("2scxApp_{0}_{1}.zip", appWrapper.GetNameWithoutSpecialChars(), appWrapper.GetVersion());
            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {
                var fileBytes = fileStream.ToArray();
                return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "application/octet-stream", new MemoryStream(fileBytes));
            }
        }

        [HttpGet]
        public HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            EnsureUserIsAdmin();

            var appWrapper = new SxcAppWrapper(appId);

            var fileName = string.Format("2scxContentExport_{0}_{1}.xml", appWrapper.GetNameWithoutSpecialChars(), appWrapper.GetVersion());
            var fileXml = new XmlExporter
            (
                zoneId,
                appId,
                false,
                contentTypeIdsString == null ? new string[0] : contentTypeIdsString.Split(';'),
                entityIdsString == null ? new string[0] : entityIdsString.Split(';')
            ).GenerateNiceXml();

            return HttpResponseMessageHelper.GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }



        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportApp(ImportRequestArgs args)
        {
            var result = new ImportResult();
            using (var fileStream = new MemoryStream(GetArrayFromBase64Url(args.FileData)))
            {
                var zipImport = new ZipImport(args.ZoneId, null, PortalSettings.UserInfo.IsSuperUser);
                result.Succeeded = zipImport.ImportApp(fileStream, HttpContext.Current.Server, PortalSettings, result.Messages);
            }
            return result;
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ImportResult ImportContent(ImportRequestArgs args)
        {
            var result = new ImportResult();
            using (var fileStream = new MemoryStream(GetArrayFromBase64Url(args.FileData)))
            {
                if (args.FileName.EndsWith(".zip"))
                {   // ZIP
                    var zipImport = new ZipImport(args.ZoneId, args.AppId, PortalSettings.UserInfo.IsSuperUser);
                    result.Succeeded = zipImport.ImportZip(fileStream, HttpContext.Current.Server, PortalSettings, result.Messages);
                }
                else
                {   // XML
                    using (var fileStreamReader = new StreamReader(fileStream))
                    {
                        var xmlImport = new XmlImport(PortalSettings.DefaultLanguage, UserIdentity.CurrentUserIdentityToken);
                        var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
                        result.Succeeded = xmlImport.ImportXml(args.ZoneId, args.AppId, xmlDocument);
                        result.Messages = xmlImport.ImportLog;
                    }
                }
            }
            return result;
        }


        private void EnsureUserIsAdmin()
        {
            if (!PortalSettings.UserInfo.Roles.Contains("Administrators"))
                throw new AuthenticationException();
        }

        private byte[] GetArrayFromBase64Url(string base64Url)
        {
            var base64Data = base64Url.Substring(base64Url.IndexOf(',') + 1); // or Regex.Match(base64Url, @"[data:(?<media>.+?)/(?<type>.+?),](?<data>.+)").Groups["data"].Value;
            // Correct transfer errors!
            base64Data = base64Data.Replace('-', '+').Replace('_', '/').Trim('=');
            var base64Padding = base64Data.Length % 4;
            if (base64Padding > 0)
            {
                base64Data += new string('=', 4 - base64Padding);
            }

            return Convert.FromBase64String(base64Data);
        }
    }
}