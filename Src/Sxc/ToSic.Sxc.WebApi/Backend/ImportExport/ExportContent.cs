using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.XmlExport;
using ToSic.Eav.WebApi.Sys.ImportExport;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExportContent(
    XmlExporter xmlExporter,
    GenWorkPlus<WorkViews> workViews,
    GenWorkPlus<WorkEntities> workEntities,
    Generator<ImpExpHelpers> impExpHelpers,
    IResponseMaker responseMaker)
    : ServiceBase("Bck.Export",
        connect: [xmlExporter, workViews, workEntities, impExpHelpers, responseMaker])
{

    public ExportPartsOverviewDto PreExportSummary(int zoneId, int appId, string scope)
    {
        var appIdentity = new AppIdentity(zoneId, appId);
        var l = Log.Fn<ExportPartsOverviewDto>($"get content info for {appIdentity.Show()} scope:{scope}");
        var currentApp = impExpHelpers.New().GetReaderAfterZoneSwitchPermissionCheck(appIdentity);

        var appCtx = workEntities.CtxSvc.ContextPlus(currentApp);
        var contentTypes = currentApp.ContentTypes.OfScope(scope);
        var entities = workEntities.New(appCtx).All();
        var templates = workViews.New(appCtx).GetAll();

        return l.Return(new()
        {
            ContentTypes = contentTypes
                .Select(c => new ExportPartsContentTypesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StaticName = c.NameId,
                    Templates = templates
                        .Where(t => t.ContentType == c.NameId)
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
        });
    }


    public THttpResponseType Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
    {
        var l = Log.Fn<THttpResponseType>($"export content z#{zoneId}, a#{appId}, ids:{entityIdsString}, templId:{templateIdsString}");

        var specs = new AppExportSpecs(zoneId, appId);
        var currentApp = impExpHelpers.New().GetReaderAfterZoneSwitchPermissionCheck(specs);

        var fileName = $"2sxcContentExport_{currentApp.Specs.ToFileNameWithVersion()}.xml";
        var fileXml = xmlExporter.Init(specs, currentApp, false,
            contentTypeIdsString?.Split(';') ?? [],
            entityIdsString?.Split(';') ?? []
        ).GenerateNiceXml();

        var result = responseMaker.File(fileXml, fileName, "text/xml");
        return l.Return(result);
    }
}