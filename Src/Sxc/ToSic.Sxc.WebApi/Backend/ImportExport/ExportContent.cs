#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif
using System;
using System.Linq;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.App;

namespace ToSic.Sxc.Backend.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ExportContent : ServiceBase
{
    private readonly GenWorkPlus<WorkEntities> _workEntities;

    #region Constructor / DI

    public ExportContent(XmlExporter xmlExporter, GenWorkPlus<WorkViews> workViews, GenWorkPlus<WorkEntities> workEntities, ISite site, IUser user, Generator<ImpExpHelpers> impExpHelpers, IResponseMaker responseMaker)
        : base("Bck.Export")
    {
        ConnectServices(
            _xmlExporter = xmlExporter,
            _workViews = workViews,
            _workEntities = workEntities,
            _site = site,
            _user = user,
            _impExpHelpers = impExpHelpers,
            _responseMaker = responseMaker
        );
    }

    private readonly GenWorkPlus<WorkViews> _workViews;
    private readonly XmlExporter _xmlExporter;
    private readonly ISite _site;
    private readonly IUser _user;
    private readonly Generator<ImpExpHelpers> _impExpHelpers;
    private readonly IResponseMaker _responseMaker;

    #endregion

    public ExportPartsOverviewDto PreExportSummary(int zoneId, int appId, string scope)
    {
        Log.A($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{_user.IsSystemAdmin}");
        var contextZoneId = _site.ZoneId;
        var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

        var appCtx = _workEntities.CtxSvc.ContextPlus(currentApp);
        var contentTypes = currentApp.ContentTypes.OfScope(scope);
        var entities = _workEntities.New(appCtx).All();
        var templates = _workViews.New(appCtx).GetAll();

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
                .Where(t => string.IsNullOrEmpty(t.ContentType))
                .Select(t => new IdNameDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
        };
    }


    public THttpResponseType Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
    {
        var l = Log.Fn<THttpResponseType>($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");
        SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, _site.ZoneId);

        var fileName = $"2sxcContentExport_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}.xml";
        var fileXml = _xmlExporter.Init(zoneId, appId, currentApp, false,
            contentTypeIdsString?.Split(';') ?? Array.Empty<string>(),
            entityIdsString?.Split(';') ?? Array.Empty<string>()
        ).GenerateNiceXml();

        var result = _responseMaker.File(fileXml, fileName, "text/xml");
        return l.Return(result);
    }
}