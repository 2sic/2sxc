using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Shared;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;
using ISite = ToSic.Eav.Context.ISite;

#if NETFRAMEWORK
using System.IO;
using System.Net.Http;
using ToSic.Eav.WebApi.ImportExport;
#else
using Microsoft.AspNetCore.Mvc;
#endif

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ExportApp: ServiceBase
    {

        #region Constructor / DI

        private readonly AppWorkSxc _appWorkSxc;
        private readonly IZoneMapper _zoneMapper;
        private readonly ZipExport _zipExport;
        private readonly ISite _site;
        private readonly IUser _user;
        private readonly IFeaturesInternal _features;
        private readonly Generator<ImpExpHelpers> _impExpHelpers;

        public ExportApp(
            IZoneMapper zoneMapper, 
            ZipExport zipExport,
            AppWorkSxc appWorkSxc,
            ISite site, 
            IUser user, 
            Generator<ImpExpHelpers> impExpHelpers, 
            IFeaturesInternal features
            ) : base("Bck.Export")
        {
            ConnectServices(
                _appWorkSxc = appWorkSxc,
                _zoneMapper = zoneMapper,
                _zipExport = zipExport,
                _site = site,
                _user = user,
                _features = features,
                _impExpHelpers = impExpHelpers
            );
        }

        #endregion

        public AppExportInfoDto GetAppInfo(int zoneId, int appId)
        {
            Log.A($"get app info for app:{appId} and zone:{zoneId}");
            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared);
            var cultCount = _zoneMapper.CulturesWithState(_site).Count(c => c.IsEnabled);

            var appCtx = _appWorkSxc.AppWork.Context(currentApp);
            var appEntities = _appWorkSxc.AppWork.Entities;
            var appViews = _appWorkSxc.AppViews(appCtx);

            return new AppExportInfoDto
            {
                Name = currentApp.Name,
                Guid = currentApp.NameId,
                Version = currentApp.VersionSafe(),
                EntitiesCount = appEntities.All(appCtx).Count(e => !e.HasAncestor()),
                LanguagesCount = cultCount,
                TemplatesCount = appViews.GetAll().Count(),
                HasRazorTemplates = appViews.GetRazor().Any(),
                HasTokenTemplates = appViews.GetToken().Any(),
                FilesCount = zipExport.FileManager.AllFiles().Count() // PortalFilesCount
                    + (currentApp.AppState.HasCustomParentApp() ? 0 : zipExport.FileManagerGlobal.AllFiles().Count()), // GlobalFilesCount
                TransferableFilesCount = zipExport.FileManager.GetAllTransferableFiles().Count() // TransferablePortalFilesCount
                    + (currentApp.AppState.HasCustomParentApp() ? 0 : zipExport.FileManagerGlobal.GetAllTransferableFiles().Count()), // TransferableGlobalFilesCount
            };
        }

        internal bool SaveDataForVersionControl(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid, bool withSiteFiles)
        {
            Log.A($"export for version control z#{zoneId}, a#{appId}, include:{includeContentGroups}, reset:{resetAppGuid}");
            SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            // Ensure feature available...
            SyncWithSiteFilesVerifyFeaturesOrThrow(_features, withSiteFiles);

            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared);
            zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid, withSiteFiles);

            return true;
        }

        internal static void SyncWithSiteFilesVerifyFeaturesOrThrow(IFeaturesInternal features, bool withSiteFiles)
        {
            if (!withSiteFiles) return;
            features.ThrowIfNotEnabled("Requires features enabled to sync with site files ",
                BuiltInFeatures.AppSyncWithSiteFiles.Guid);
        }

#if NETFRAMEWORK
        public HttpResponseMessage Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
#else
        public IActionResult Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
#endif

        {
            Log.A($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
            SecurityHelpers.ThrowIfNotSiteAdmin(_user, Log); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = _impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared);
            var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

            var fileName =
                $"2sxcApp_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}{addOnWhenContainingContent}.zip";
            Log.A($"file name:{fileName}");

            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {

                var fileBytes = fileStream.ToArray();
                Log.A("will stream so many bytes:" + fileBytes.Length);
                var mimeType = MimeHelper.FallbackType;
#if NETFRAMEWORK
                return HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, mimeType, new MemoryStream(fileBytes));
#else
                return new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName };
#endif
            }
        }
    }
}
