using System;
using System.Linq;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ExportContent<THttpResponseType> : HasLog
    {
        #region Constructor / DI

        public ExportContent(XmlExporter xmlExporter, Lazy<CmsRuntime> cmsRuntime, ISite site, IUser user, GeneratorLog<ImpExpHelpers> impExpHelpers, ResponseMaker<THttpResponseType> responseMaker) : base("Bck.Export")
        {
            _xmlExporter = xmlExporter;
            _cmsRuntime = cmsRuntime;
            _site = site;
            _user = user;
            _impExpHelpers = impExpHelpers.SetLog(Log);
            _responseMaker = responseMaker;
        }

        private readonly XmlExporter _xmlExporter;
        private readonly Lazy<CmsRuntime> _cmsRuntime;
        private readonly ISite _site;
        private CmsRuntime CmsRuntime => _cmsRuntime.Value;
        private readonly IUser _user;
        private readonly GeneratorLog<ImpExpHelpers> _impExpHelpers;
        private readonly ResponseMaker<THttpResponseType> _responseMaker;

        #endregion

        public ExportPartsOverviewDto PreExportSummary(int zoneId, int appId, string scope)
        {
            Log.A($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{_user.IsSystemAdmin}");
            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

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


        public THttpResponseType Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString,
            string templateIdsString)
        {
            Log.A($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");
            SecurityHelpers.ThrowIfNotAdmin(_user.IsSiteAdmin); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
            var appRuntime = CmsRuntime.Init(currentApp, true, Log);

            var fileName = $"2sxcContentExport_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}.xml";
            var fileXml = _xmlExporter.Init(zoneId, appId, appRuntime, false,
                contentTypeIdsString?.Split(';') ?? Array.Empty<string>(),
                entityIdsString?.Split(';') ?? Array.Empty<string>(),
                Log
            ).GenerateNiceXml();

            return _responseMaker.File(fileXml, fileName, "text/xml");
        }
    }
}
