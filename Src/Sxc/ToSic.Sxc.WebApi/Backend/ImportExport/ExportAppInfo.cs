using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data.Sys.Ancestors;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ISite = ToSic.Eav.Context.ISite;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExportAppInfo(
    IZoneMapper zoneMapper,
    AppWorkContextService appCtxSvc,
    AppWorkChain<WorkViews> workViews,
    AppWorkChain<WorkEntities> workEntities,
    ExportHelper exportHelper,
    ISite site
) : ServiceBase("Bck.Export", connect: [workEntities, appCtxSvc, workViews, zoneMapper, site, exportHelper])
{
    public AppExportInfoModel GetAppInfo(IAppIdentity appIdentity)
    {
        var l = Log.Fn<AppExportInfoModel>($"get app info for app: {appIdentity.Show()}");
        var (appReader, zipExport) = exportHelper.GetZipExportAndCheckZoneSwitchPermissions(appIdentity);
    
        var ctxNew = appCtxSvc.ContextNew(appReader);
        var appEntities = workEntities.New(ctxNew);
        var appViews = workViews.New(ctxNew);

        var appHasCustomParent = appReader.HasCustomParentApp();

        var filesCount = zipExport.CountFiles(!appHasCustomParent, fm => fm.AllFiles()); // PortalFilesCount + GlobalFilesCount

        var transferableFilesCount =
            zipExport.CountFiles(!appHasCustomParent, fm => fm.GetAllTransferableFiles()); // TransferablePortalFilesCount + TransferableGlobalFilesCount

        var appSpecs = appReader.Specs;
        var allViews = appViews.GetAll();
        return l.Return(new()
        {
            Id = appIdentity.AppId,
            Name = appSpecs.Name,
            NameId = appSpecs.NameId,
            Version = appSpecs.VersionSafe(),
            EntitiesCount = appEntities.All().Count(e => !e.HasAncestor()),
            LanguagesCount = zoneMapper.CulturesEnabledWithState(site).Count,
            TemplatesCount = allViews.Count,
            HasRazorTemplates = allViews.Any(t => t.IsRazor),
            HasTokenTemplates = allViews.Any(t => !t.IsRazor),
            FilesCount = filesCount,
            TransferableFilesCount = transferableFilesCount,
        });
    }

}