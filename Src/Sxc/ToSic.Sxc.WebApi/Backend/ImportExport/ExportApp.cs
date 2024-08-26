using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.ImportExport.Internal.Zip;
using ToSic.Eav.Integration;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Sxc.Apps.Internal.Work;
using ISite = ToSic.Eav.Context.ISite;
using ToSic.Eav.ImportExport.Internal;


#if NETFRAMEWORK
using System.IO;
using System.Net.Http;
using ToSic.Eav.WebApi.ImportExport;
using HttpResponse = System.Net.Http.HttpResponseMessage;
#else
using Microsoft.AspNetCore.Mvc;
using HttpResponse = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ExportApp(
    IZoneMapper zoneMapper,
    ZipExport export,
    AppWorkContextService appWorkCtxSvc,
    GenWorkPlus<WorkViews> workViews,
    GenWorkPlus<WorkEntities> workEntities,
    ISite site,
    IUser user,
    Generator<ImpExpHelpers> impExpHelpers,
    IEavFeaturesService features,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.Export",
        connect:
        [
            workEntities, appWorkCtxSvc, workViews, zoneMapper, export, site, user, features, impExpHelpers,
            appPathSvc
        ])
{
    public AppExportInfoDto GetAppInfo(int zoneId, int appId)
    {
        var l = Log.Fn<AppExportInfoDto>($"get app info for app:{appId} and zone:{zoneId}");
        var contextZoneId = site.ZoneId;
        var appReader = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, contextZoneId);
        var appPaths = appPathSvc.Get(appReader, site);
        var specs = appReader.Specs;
        var zipExport = export.Init(zoneId, appId, specs.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        var cultCount = zoneMapper.CulturesEnabledWithState(site).Count;

        var appCtx = appWorkCtxSvc.ContextPlus(appReader);
        var appEntities = workEntities.New(appCtx);
        var appViews = workViews.New(appCtx);

        var appHasCustomParent = appReader.HasCustomParentApp();

        return l.Return(new()
        {
            Name = specs.Name,
            Guid = specs.NameId,
            Version = specs.VersionSafe(),
            EntitiesCount = appEntities.All().Count(e => !e.HasAncestor()),
            LanguagesCount = cultCount,
            TemplatesCount = appViews.GetAll().Count(),
            HasRazorTemplates = appViews.GetRazor().Any(),
            HasTokenTemplates = appViews.GetToken().Any(),
            FilesCount = zipExport.FileManager.AllFiles().Count() // PortalFilesCount
                         + (appHasCustomParent ? 0 : zipExport.FileManagerGlobal.AllFiles().Count()), // GlobalFilesCount
            TransferableFilesCount = zipExport.FileManager.GetAllTransferableFiles().Count() // TransferablePortalFilesCount
                                     + (appHasCustomParent ? 0 : zipExport.FileManagerGlobal.GetAllTransferableFiles().Count()), // TransferableGlobalFilesCount
        });
    }

    internal bool SaveDataForVersionControl(AppExportSpecs specs)
    {
        var l = Log.Fn<bool>(specs.Dump());
        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        // Ensure feature available...
        SyncWithSiteFilesVerifyFeaturesOrThrow(features, specs.WithSiteFiles);

        var contextZoneId = site.ZoneId;
        var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(specs.ZoneId, specs.AppId, user, contextZoneId);
        var appPaths = appPathSvc.Get(appRead, site);

        var zipExport = export.Init(specs.ZoneId, specs.AppId, appRead.Specs.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        zipExport.ExportForSourceControl(specs);

        return l.ReturnTrue();
    }

    internal static void SyncWithSiteFilesVerifyFeaturesOrThrow(IEavFeaturesService features, bool withSiteFiles)
    {
        if (!withSiteFiles) return;
        features.ThrowIfNotEnabled("Requires features enabled to sync with site files ",
            BuiltInFeatures.AppSyncWithSiteFiles.Guid);
    }

    public HttpResponse Export(AppExportSpecs specs)
    {
        var l = Log.Fn<HttpResponse>(specs.Dump());

        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        var contextZoneId = site.ZoneId;
        var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(specs.ZoneId, specs.AppId, user, contextZoneId);
        var appPaths = appPathSvc.Get(appRead, site);

        var zipExport = export.Init(specs.ZoneId, specs.AppId, appRead.Specs.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        var addOnWhenContainingContent = specs.IncludeContentGroups
            ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm")
            : "";

        var fileName =
            $"2sxcApp_{appRead.Specs.ToFileNameWithVersion()}{addOnWhenContainingContent}.zip";
        l.A($"file name:{fileName}");

        using var fileStream = zipExport.ExportApp(specs);
        var fileBytes = fileStream.ToArray();
        l.A("will stream so many bytes:" + fileBytes.Length);
        var mimeType = MimeHelper.FallbackType;

#if NETFRAMEWORK
        return l.Return(HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, mimeType, new MemoryStream(fileBytes)));
#else
        return l.Return(new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName });
#endif
    }
}