using System;
using System.Linq;
using System.Net.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi.ImportExport
{
    internal class ExportContent: HasLog
    {

        #region Constructor / DI

        public ExportContent(IZoneMapper zoneMapper, XmlExporter xmlExporter, Lazy<AppRuntime> appRuntime) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _xmlExporter = xmlExporter;
            _appRuntime = appRuntime;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly XmlExporter _xmlExporter;
        private readonly Lazy<AppRuntime> _appRuntime;
        private AppRuntime AppRuntime => _appRuntime.Value;
        private IUser _user;
        private int _siteId;

        public ExportContent Init(int tenantId, IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _siteId = tenantId;
            _user = user;
            return this;
        }

        #endregion

        public ExportPartsOverviewDto PreExportSummary(int appId, int zoneId, string scope)
        {
            Log.Add($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{_user.IsSuperUser}");
            var contextZoneId = _zoneMapper.GetZoneId(_siteId);
            var currentApp = ImpExpHelpers.GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId, Log);

            var cms = new CmsRuntime(currentApp, Log, true, false);
            var contentTypes = cms.ContentTypes.All.OfScope(scope);
            var entities = cms.Entities.All;
            var templates = cms.Views.GetAll();

            return new ExportPartsOverviewDto
            {
                ContentTypes = contentTypes.Select(c => new ExportPartsContentTypesDto
                {
                    Id = c.ContentTypeId,
                    Name = c.Name,
                    StaticName = c.StaticName,
                    Templates = templates.Where(t => t.ContentType == c.StaticName)
                        .Select(t => new IdNameDto
                        {
                            Id = t.Id,
                            Name = t.Name
                        }),
                    Entities = entities
                        .Where(e => e.Type.ContentTypeId == c.ContentTypeId)
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

            var contextZoneId = _zoneMapper.GetZoneId(_siteId);
            var currentApp = ImpExpHelpers.GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId, Log);
            var appRuntime = AppRuntime.Init(currentApp, true, Log);

            var fileName = $"2sxcContentExport_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}.xml";
            var fileXml = _xmlExporter.Init(zoneId, appId, appRuntime, false,
                contentTypeIdsString?.Split(';') ?? new string[0],
                entityIdsString?.Split(';') ?? new string[0],
                Log
            ).GenerateNiceXml();

            return HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, "text/xml", fileXml);
        }




    }
}
