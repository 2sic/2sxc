#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.App;

namespace ToSic.Sxc.Backend.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ExportContent(
    XmlExporter xmlExporter,
    GenWorkPlus<WorkViews> workViews,
    GenWorkPlus<WorkEntities> workEntities,
    ISite site,
    IUser user,
    Generator<ImpExpHelpers> impExpHelpers,
    IResponseMaker responseMaker)
    : ServiceBase("Bck.Export",
        connect: [xmlExporter, workViews, workEntities, site, user, impExpHelpers, responseMaker])
{

    public ExportPartsOverviewDto PreExportSummary(int zoneId, int appId, string scope)
    {
        Log.A($"get content info for z#{zoneId}, a#{appId}, scope:{scope} super?:{user.IsSystemAdmin}");
        var contextZoneId = site.ZoneId;
        var currentApp = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, contextZoneId);

        var appCtx = workEntities.CtxSvc.ContextPlus(currentApp);
        var contentTypes = currentApp.ContentTypes.OfScope(scope);
        var entities = workEntities.New(appCtx).All();
        var templates = workViews.New(appCtx).GetAll();

        return new()
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
        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        var currentApp = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, site.ZoneId);

        var fileName = $"2sxcContentExport_{currentApp.Specs.ToFileNameWithVersion()}.xml";
        var fileXml = xmlExporter.Init(new AppExportSpecs(zoneId, appId), currentApp, false,
            contentTypeIdsString?.Split(';') ?? [],
            entityIdsString?.Split(';') ?? []
        ).GenerateNiceXml();

        var result = responseMaker.File(fileXml, fileName, "text/xml");
        return l.Return(result);
    }
}