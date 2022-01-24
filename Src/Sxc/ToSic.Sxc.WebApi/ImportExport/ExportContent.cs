using System;
using System.Linq;
using System.Net.Http;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ExportContent: HasLog<ExportContent>
    {
        #region Constructor / DI

        public ExportContent(XmlExporter xmlExporter, Lazy<CmsRuntime> cmsRuntime, ISite site, IUser user) : base("Bck.Export")
        {
            _xmlExporter = xmlExporter;
            _cmsRuntime = cmsRuntime;
            _site = site;
            _user = user;
        }

        private readonly XmlExporter _xmlExporter;
        private readonly Lazy<CmsRuntime> _cmsRuntime;
        private readonly ISite _site;
        private CmsRuntime CmsRuntime => _cmsRuntime.Value;
        private readonly IUser _user;

        #endregion

        public ExportPartsOverviewDto PreExportSummary(int appId, int zoneId, string scope)
        {
            Log.Add($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{_user.IsSuperUser}");
            var contextZoneId = _site.ZoneId;
            var currentApp = CmsRuntime.ServiceProvider.Build<ImpExpHelpers>().Init(Log).GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var cms = CmsRuntime.Init(currentApp, true, Log);
            var contentTypes = cms.ContentTypes.All.OfScope(scope);
            var entities = cms.Entities.All;
            var templates = cms.Views.GetAll();

            return new ExportPartsOverviewDto
            {
                ContentTypes = contentTypes.Select(c => new ExportPartsContentTypesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StaticName = c.NameId,
                    Templates = templates.Where(t => t.ContentType == c.NameId)
                        .Select(t => new IdNameDto
                        {
                            Id = t.Id,
                            Name = t.Name
                        }),
                    Entities = entities
                        .Where(e => e.Type.Id == c.Id)
                        .Select(e => new ExportPartsEntitiesDto
                        {
                            Title = e.GetBestTitle(),
                            Id = e.EntityId
                        })
                }),
                TemplatesWithoutContentTypes = templates
                    .Where(t => !string.IsNullOrEmpty(t.ContentType))
                    .Select(t => new IdNameDto
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
            };
        }


        public HttpResponseMessage Export(int appId, int zoneId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            Log.Add($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");
            SecurityHelpers.ThrowIfNotAdmin(_user); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = CmsRuntime.ServiceProvider.Build<ImpExpHelpers>().Init(Log).GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
            var appRuntime = CmsRuntime.Init(currentApp, true, Log);

            var fileName = $"2sxcContentExport_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}.xml";
            var fileXml = _xmlExporter.Init(zoneId, appId, appRuntime, false,
                contentTypeIdsString?.Split(';') ?? Array.Empty<string>(),
                entityIdsString?.Split(';') ?? Array.Empty<string>(),
                Log
            ).GenerateNiceXml();

            return HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }




    }
}
