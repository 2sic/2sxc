using System;
using System.IO;
using System.Linq;
using System.Net.Http;
#if NETSTANDARD
using Microsoft.AspNetCore.Mvc;
#endif
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ExportApp: HasLog
    {
        #region Constructor / DI

        public ExportApp(IZoneMapper zoneMapper, ZipExport zipExport, CmsRuntime cmsRuntime, ISite site, IUser user) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _zipExport = zipExport;
            _cmsRuntime = cmsRuntime;
            _site = site;
            _user = user;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly ZipExport _zipExport;
        private readonly CmsRuntime _cmsRuntime;
        private readonly ISite _site;
        private readonly IUser _user;

        public ExportApp Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            return this;
        }

        #endregion

        public AppExportInfoDto GetAppInfo(int appId, int zoneId)
        {
            Log.Add($"get app info for app:{appId} and zone:{zoneId}");
            var contextZoneId = _site.ZoneId;
            var currentApp = _cmsRuntime.ServiceProvider.Build<ImpExpHelpers>().Init(Log).GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared, Log);
            var cultCount = _zoneMapper.CulturesWithState(_site).Count(c => c.IsEnabled);

            var cms = _cmsRuntime.Init(currentApp, true, Log);

            return new AppExportInfoDto
            {
                Name = currentApp.Name,
                Guid = currentApp.AppGuid,
                Version = currentApp.VersionSafe(),
                EntitiesCount = cms.Entities.All.Where(e => !e.HasAncestor()).Count(),
                LanguagesCount = cultCount,
                TemplatesCount = cms.Views.GetAll().Count(),
                HasRazorTemplates = cms.Views.GetRazor().Any(),
                HasTokenTemplates = cms.Views.GetToken().Any(),
                FilesCount = zipExport.FileManager.AllFiles.Count() // PortalFilesCount
                    + (currentApp.AppState.HasParentApp() ? 0 : zipExport.FileManagerGlobal.AllFiles.Count()), // GlobalFilesCount
                TransferableFilesCount = zipExport.FileManager.AllTransferableFiles.Count() // TransferablePortalFilesCount
                    + (currentApp.AppState.HasParentApp() ? 0 : zipExport.FileManagerGlobal.AllTransferableFiles.Count()), // TransferableGlobalFilesCount
            };
        }

        public bool SaveDataForVersionControl(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
        {
            Log.Add($"export for version control z#{zoneId}, a#{appId}, include:{includeContentGroups}, reset:{resetAppGuid}");
            SecurityHelpers.ThrowIfNotAdmin(_user); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = _cmsRuntime.ServiceProvider.Build<ImpExpHelpers>().Init(Log).GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared, Log);
            zipExport.ExportForSourceControl(includeContentGroups, resetAppGuid);

            return true;
        }

#if NETSTANDARD
        public IActionResult Export(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
#else
    public HttpResponseMessage Export(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
#endif
        {
            Log.Add($"export app z#{zoneId}, a#{appId}, incl:{includeContentGroups}, reset:{resetAppGuid}");
            SecurityHelpers.ThrowIfNotAdmin(_user); // must happen inside here, as it's opened as a new browser window, so not all headers exist

            var contextZoneId = _site.ZoneId;
            var currentApp = _cmsRuntime.ServiceProvider.Build<ImpExpHelpers>().Init(Log).GetAppAndCheckZoneSwitchPermissions(zoneId, appId, _user, contextZoneId);

            var zipExport = _zipExport.Init(zoneId, appId, currentApp.Folder, currentApp.PhysicalPath, currentApp.PhysicalPathShared, Log);
            var addOnWhenContainingContent = includeContentGroups ? "_withPageContent_" + DateTime.Now.ToString("yyyy-MM-ddTHHmm") : "";

            var fileName =
                $"2sxcApp_{currentApp.NameWithoutSpecialChars()}_{currentApp.VersionSafe()}{addOnWhenContainingContent}.zip";
            Log.Add($"file name:{fileName}");

            using (var fileStream = zipExport.ExportApp(includeContentGroups, resetAppGuid))
            {

                var fileBytes = fileStream.ToArray();
                Log.Add("will stream so many bytes:" + fileBytes.Length);
                var mimeType = MimeHelper.FallbackType;
#if NETSTANDARD
                return new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName };
#else
                return HttpFileHelper.GetAttachmentHttpResponseMessage(fileName, mimeType, new MemoryStream(fileBytes));
#endif
            }
        }
    }
}
