using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.ImportExport.Internal.Zip;
using ToSic.Eav.Integration;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.App;
using ISite = ToSic.Eav.Context.ISite;

#if NETFRAMEWORK
using System.IO;
using System.Net.Http;
using ToSic.Eav.WebApi.ImportExport;
#else
using Microsoft.AspNetCore.Mvc;
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
        var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, contextZoneId);
        var appPaths = appPathSvc.Init(site, appRead);

        var zipExport = export.Init(zoneId, appId, appRead.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        var cultCount = zoneMapper.CulturesEnabledWithState(site).Count;

        var appCtx = appWorkCtxSvc.ContextPlus(appRead);
        var appEntities = workEntities.New(appCtx);
        var appViews = workViews.New(appCtx);

        var appHasCustomParent = appRead.HasCustomParentApp();

        return l.Return(new()
        {
            Name = appRead.Name,
            Guid = appRead.NameId,
            Version = appRead.VersionSafe(),
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

    internal bool SaveDataForVersionControl(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool withSiteFiles)
    {
        var l = Log.Fn<bool>($"export for version control z#{zoneId}, a#{appId}, include:{includeContentGroups}, reset:{resetAppGuid}");
        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        // Ensure feature available...
        SyncWithSiteFilesVerifyFeaturesOrThrow(features, withSiteFiles);

        var contextZoneId = site.ZoneId;
        var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, contextZoneId);
        var appPaths = appPathSvc.Init(site, appRead);

        var zipExport = export.Init(zoneId, appId, appRead.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid, withSiteFiles);

        return l.ReturnTrue();
    }

    internal static void SyncWithSiteFilesVerifyFeaturesOrThrow(IEavFeaturesService features, bool withSiteFiles)
    {
        if (!withSiteFiles) return;
        features.ThrowIfNotEnabled("Requires features enabled to sync with site files ",
            BuiltInFeatures.AppSyncWithSiteFiles.Guid);
    }

#if NETFRAMEWORK
        public HttpResponseMessage Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
        {
            var l = Log.Fn<HttpResponseMessage>($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
#else
    public IActionResult Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
    {
        var l = Log.Fn<IActionResult>($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
#endif

        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        var contextZoneId = site.ZoneId;
        var appRead = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, user, contextZoneId);
        var appPaths = appPathSvc.Init(site, appRead);

        var zipExport = export.Init(zoneId, appId, appRead.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

        var fileName =
            $"2sxcApp_{appRead.NameWithoutSpecialChars()}_{appRead.VersionSafe()}{addOnWhenContainingContent}.zip";
        Log.A($"file name:{fileName}");

        using var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid);
        var fileBytes = fileStream.ToArray();
        Log.A("will stream so many bytes:" + fileBytes.Length);
        var mimeType = MimeHelper.FallbackType;
#if NETFRAMEWORK
            return l.Return(HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, mimeType, new MemoryStream(fileBytes)));
#else
        return l.Return(new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName });
#endif
    }
}