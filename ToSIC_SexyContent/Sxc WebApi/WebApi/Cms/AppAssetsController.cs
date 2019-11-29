using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent;
using ToSic.SexyContent.AppAssets;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.SxcTemp;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class AppAssetsController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sApAs");
        }

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
            var files = new List<FileInfo>();  // List that will hold the files and subfiles in path
            var folders = new List<DirectoryInfo>(); // List that hold direcotries that cannot be accessed
            var di = new DirectoryInfo(fullPath);
            FullDirList(di, mask, folders, files, opt);

            // return folders or files (depending on setting) with/without subfolders
            return (returnFolders
                ? folders
                    .Select(f => f.FullName) 
                // Directory.GetDirectories(fullPath, mask, opt)
                    //.Select(Path.GetDirectoryName)
                : files.Select(f => f.FullName)
                //Directory.GetFiles(fullPath, mask, opt)
                //    .Select(Path.GetFullPath)
                )
                .Select(p => EnsurePathMayBeAccessed(p, appPath, allowFullAccess))   // do another security check
                .Select(x => x.Replace(appPath + "\\", ""))         // truncate / remove internal server root path
                .Select(x => x.Replace("\\", "/"))                  // tip the slashes to web-convention - important, because old entries for templates used that slash
                .ToList();
        }

        private void FullDirList(DirectoryInfo dir, string searchPattern, List<DirectoryInfo> folders, List<FileInfo> files, SearchOption opt)
        {
            // Console.WriteLine("Directory {0}", dir.FullName);
            // list the files
            try
            {
                foreach (var f in dir.GetFiles(searchPattern))
                {
                    try
                    {
                        files.Add(f);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignore errors
                // Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return;  // We alredy got an error trying to access dir so dont try to access it again
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            if (opt != SearchOption.AllDirectories) return;

            foreach (var d in dir.GetDirectories())
            {
                try
                {
                    if (!Eav.ImportExport.Settings.ExcludeFolders.Contains(d.Name))
                    {
                        folders.Add(d);
                        FullDirList(d, searchPattern, folders, files, opt);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }


        private string ResolveAppPath(int appId, bool global,  bool allowFullAccess)
        {
            var thisApp = GetApp.LightWithoutData(new DnnTenant(PortalSettings.Current), appId, Log);

            if (global && !allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            var appPath = SexyContent.Internal.TemplateHelpers.GetTemplatePathRoot(global
                ? Settings.TemplateLocations.HostFileSystem
                : Settings.TemplateLocations.PortalFileSystem
                , thisApp); // get root in global system

            appPath = global::System.Web.Hosting.HostingEnvironment.MapPath(appPath);
            return appPath;
        }


        /// <summary>
        /// Create a new file (if it doesn't exist yet) and optionally prefill it with contetn
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

            var thisApp = GetApp.LightWithoutData(new DnnTenant(PortalSettings.Current), appId, Log);

            if (content.Content == null)
                content.Content = "";

            path = SanitizePathAndContent(path, content);

            var isAdmin = UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            var assetEditor = new AssetEditor(thisApp, path, UserInfo.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAsset(path);
            return assetEditor.Create(content.Content);
        }

        private static string SanitizePathAndContent(string path, ContentHelper content)
        {
            var name = Path.GetFileName(path);
            var folder = Path.GetDirectoryName(path);
            var ext = Path.GetExtension(path);

            if (ext?.ToLowerInvariant() == AssetEditor.CsExtension)
            {
                if ((folder?.ToLower().IndexOf(AssetEditor.CsApiFolder) ?? -1) > -1) 
                    content.Content = AssetEditor.DefaultCsBody;
                return path;
            }

            if (ext?.ToLowerInvariant() != AssetEditor.CshtmlExtension) return path;

            if (name == null) name = "missing-name.txt";

            if (!name.StartsWith(AssetEditor.CshtmlPrefix))
            {
                name = AssetEditor.CshtmlPrefix + name;
                path = (string.IsNullOrWhiteSpace(folder) ? "" : folder + "\\") + name;
            }

            // if we're creating a cshtml and it's empty, or has the dummy-text from the old 2sxc 9 admin-UI, then replace it
            if (string.IsNullOrEmpty(content.Content) || content.Content.StartsWith("<p>You successfully"))
                content.Content = AssetEditor.DefaultCshtmlBody;

            return path;
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
        /// 
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="path"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        #region Template --> later neutralize to standard asset-editing
        [HttpGet]
        public AssetEditInfo Asset(int templateId = 0, string path = null, bool global = false)
        {
            Log.Add($"asset templ:{templateId}, path:{path}, global:{global}");
            var isAdmin = UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            var assetEditor = (templateId != 0 && path == null)
                ? new AssetEditor(CmsBlock.App, templateId, UserInfo.IsSuperUser, isAdmin, Log)
                : new AssetEditor(CmsBlock.App, path, UserInfo.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAsset();
            return assetEditor.EditInfoWithSource;
        }


        /// <summary>
        /// Get details and source code
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Asset([FromBody] AssetEditInfo template,[FromUri] int templateId = 0, [FromUri] bool global = false, [FromUri] string path = null)
        {
            Log.Add($"asset templ:{templateId}, global:{global}, path:{path}");
            var isAdmin = UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            var assetEditor = (templateId != 0 && path == null)
                ? new AssetEditor(CmsBlock.App, templateId, UserInfo.IsSuperUser, isAdmin, Log)
                : new AssetEditor(CmsBlock.App, path, UserInfo.IsSuperUser, isAdmin, global, Log);
            assetEditor.EnsureUserMayEditAsset();
            assetEditor.Source = template.Code;
            return true;
        }

        #endregion

        #region Helpers
        private string EnsurePathMayBeAccessed(string p, string appPath, bool allowFullAccess)
        {
            if (appPath == null) throw new ArgumentNullException(nameof(appPath));
            // security check, to ensure no results leak from outside the app

            if (!allowFullAccess && !p.StartsWith(appPath))
                throw new DirectoryNotFoundException("Result was not inside the app any more - must cancel");
            return p;
        }

        #endregion


    }
}