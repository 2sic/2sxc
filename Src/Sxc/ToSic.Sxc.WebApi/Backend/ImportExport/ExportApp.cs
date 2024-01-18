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
public class ExportApp: ServiceBase
{

    #region Constructor / DI

    private readonly GenWorkPlus<WorkEntities> _workEntities;
    private readonly AppWorkContextService _appWorkCtxSvc;
    private readonly GenWorkPlus<WorkViews> _workViews;
    private readonly IZoneMapper _zoneMapper;
    private readonly ZipExport _zipExport;
    private readonly ISite _site;
    private readonly IUser _user;
    private readonly IEavFeaturesService _features;
    private readonly Generator<ImpExpHelpers> _impExpHelpers;
    private readonly IAppPathsMicroSvc _appPathSvc;

    public ExportApp(
        IZoneMapper zoneMapper, 
        ZipExport zipExport,
        AppWorkContextService appWorkCtxSvc,
        GenWorkPlus<WorkViews> workViews,
        GenWorkPlus<WorkEntities> workEntities,
        ISite site, 
        IUser user, 
        Generator<ImpExpHelpers> impExpHelpers, 
        IEavFeaturesService features,
        IAppPathsMicroSvc appPathSvc
    ) : base("Bck.Export")
    {
        ConnectServices(
            _workEntities = workEntities,
            _appWorkCtxSvc = appWorkCtxSvc,
            _workViews = workViews,
            _zoneMapper = zoneMapper,
            _zipExport = zipExport,
            _site = site,
            _user = user,
            _features = features,
            _impExpHelpers = impExpHelpers,
            _appPathSvc = appPathSvc
        );
    }

    #endregion

    public AppExportInfoDto GetAppInfo(int zoneId, int appId)
    {
        var l = Log.Fn<AppExportInfoDto>($"get app info for app:{appId} and zone:{zoneId}");
        var contextZoneId = _site.ZoneId;
        var appRead = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
        var appPaths = _appPathSvc.Init(_site, appRead);

        var zipExport = _zipExport.Init(zoneId, appId, appRead.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
        var cultCount = _zoneMapper.CulturesWithState(_site).Count(c => c.IsEnabled);

        var appCtx = _appWorkCtxSvc.ContextPlus(appRead);
        var appEntities = _workEntities.New(appCtx);
        var appViews = _workViews.New(appCtx);

        var appHasCustomParent = appRead.HasCustomParentApp();

        return l.Return(new AppExportInfoDto
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
        SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        // Ensure feature available...
        SyncWithSiteFilesVerifyFeaturesOrThrow(_features, withSiteFiles);

        var contextZoneId = _site.ZoneId;
        var appRead = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
        var appPaths = _appPathSvc.Init(_site, appRead);

        var zipExport = _zipExport.Init(zoneId, appId, appRead.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
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

        SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

        var contextZoneId = _site.ZoneId;
        var appRead = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);
        var appPaths = _appPathSvc.Init(_site, appRead);

        var zipExport = _zipExport.Init(zoneId, appId, appRead.Folder, appPaths.PhysicalPath, appPaths.PhysicalPathShared);
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