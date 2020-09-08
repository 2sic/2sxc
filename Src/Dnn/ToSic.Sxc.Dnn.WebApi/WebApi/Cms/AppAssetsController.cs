using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public partial class AppAssetsController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.Assets";

        #region Public API

        [HttpGet]
        public List<string> List(int appId, bool global = false, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
        {
            Log.Add($"list a#{appId}, global:{global}, path:{path}, mask:{mask}, withSub:{withSubfolders}, withFld:{returnFolders}");
            // set global access security if ok...
            var allowFullAccess = UserInfo.IsSuperUser;

            // make sure the folder-param is not null if it's missing
            if (string.IsNullOrEmpty(path)) path = "";
            var appPath = ResolveAppPath(appId, global, allowFullAccess);
            var fullPath = Path.Combine(appPath, path);

            // make sure the resulting path is still inside 2sxc
            if (!allowFullAccess && !fullPath.Contains("2sxc"))
                throw new DirectoryNotFoundException("the folder is not inside 2sxc-scope any more and the current user doesn't have the permissions - must cancel");

            // if the directory doesn't exist, return empty list
            if (!Directory.Exists(fullPath))
                return new List<string>();

            var opt = withSubfolders 
                ? SearchOption.AllDirectories 
                : SearchOption.TopDirectoryOnly;

            // try to collect all files, ignoring long paths errors and similar etc.
            var files = new List<FileInfo>();           // List that will hold the files and sub-files in path
            var folders = new List<DirectoryInfo>();    // List that hold directories that cannot be accessed
            var di = new DirectoryInfo(fullPath);
            FullDirList(di, mask, folders, files, opt);

            // return folders or files (depending on setting) with/without subfolders
            return (returnFolders
                    ? folders.Select(f => f.FullName)
                    : files.Select(f => f.FullName)
                )
                .Select(p => EnsurePathMayBeAccessed(p, appPath, allowFullAccess))  // do another security check
                .Select(x => x.Replace(appPath + "\\", ""))           // truncate / remove internal server root path
                .Select(x =>
                    x.Replace("\\", "/")) // tip the slashes to web-convention (old template entries used "\")
                .ToList();
        }

        /// <summary>
        /// Create a new file (if it doesn't exist yet) and optionally prefill it with content
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <returns></returns>
        [HttpPost]
        public bool Create([FromUri] int appId, [FromUri] string path,[FromBody] ContentHelper content, bool global = false)
        {
            Log.Add($"create a#{appId}, path:{path}, global:{global}, cont-length:{content.Content?.Length}");
            path = path.Replace("/", "\\");

            var thisApp = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (content.Content == null)
                content.Content = "";

            path = SanitizePathAndContent(path, content);

            var isAdmin = UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            var assetEditor = new AssetEditor(thisApp, path, UserInfo.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow(path);
            return assetEditor.Create(content.Content);
        }



        /// <summary>
        /// helper class, because it's really hard to get a post-body in a web-api call if it's not in a json-object format
        /// </summary>
        public class ContentHelper
        {
            public string Content;
        }


        #endregion


        /// <summary>
        /// Get details and source code
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        /// <returns></returns>

        #region Template --> later neutralize to standard asset-editing
        [HttpGet]
        public AssetEditInfo Asset(int templateId = 0, string path = null, bool global = false, int appId = 0)
        {
            var wrapLog = Log.Call<AssetEditInfo>($"asset templ:{templateId}, path:{path}, global:{global}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor.EditInfoWithSource);
        }


        /// <summary>
        /// Update an asset with POST
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Asset([FromBody] AssetEditInfo template,[FromUri] int templateId = 0, [FromUri] bool global = false, [FromUri] string path = null, [FromUri] int appId = 0)
        {
            var wrapLog = Log.Call<bool>($"templ:{templateId}, global:{global}, path:{path}");
            var assetEditor = GetAssetEditorOrThrowIfInsufficientPermissions(appId, templateId, global, path);
            assetEditor.Source = template.Code;
            return wrapLog(null, true);
        }

        private AssetEditor GetAssetEditorOrThrowIfInsufficientPermissions(int appId, int templateId, bool global, string path)
        {
            var wrapLog = Log.Call<AssetEditor>($"{appId}, {templateId}, {global}, {path}");
            var isAdmin = UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            var app = GetBlock().App;
            if (appId != 0 && appId != app.AppId)
                app = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId),  Log);
            var assetEditor = templateId != 0 && path == null
                ? new AssetEditor(app, templateId, UserInfo.IsSuperUser, isAdmin, Log)
                : new AssetEditor(app, path, UserInfo.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAssetOrThrow();
            return wrapLog(null, assetEditor);
        }

        #endregion

    }
}