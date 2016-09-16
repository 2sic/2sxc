using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.AppAssets;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class AppAssetsController : SxcApiController
    {
        // todo: Create the create-api
        // todo: create the delete-file/folder api
        // todo: create the copy file api

        #region Public API

        private bool _allowFullAccess;
        [HttpGet]
        public  List<string> List(int appId, bool global = false, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
        {
            // set global access security if ok...
            _allowFullAccess = UserInfo.IsSuperUser;

            // make sure the folder-param is not null if it's missing
            if (string.IsNullOrEmpty(path)) path = "";
            var appPath = ResolveAppPath(appId, global);
            var fullPath = Path.Combine(appPath, path);

            // make sure the resulting path is still inside 2sxc
            if (!_allowFullAccess && !fullPath.Contains("2sxc"))
                throw new DirectoryNotFoundException("the folder is not inside 2sxc-scope any more and the current user doesn't have the permissions - must cancel");

            // if the directory doesn't exist, return empty list
            if (!Directory.Exists(fullPath))
                return new List<string>();


            var opt = withSubfolders 
                ? SearchOption.AllDirectories 
                : SearchOption.TopDirectoryOnly;

            // return folders or files (depending on setting) with/without subfolders
            return (returnFolders
                ? Directory.GetDirectories(fullPath, mask, opt)
                    .Select(Path.GetDirectoryName)
                : Directory.GetFiles(fullPath, mask, opt)
                    .Select(Path.GetFullPath)
                )
                .Select(p => EnsurePathMayBeAccessed(p, appPath))   // do another security check
                .Select(x => x.Replace(appPath + "\\", ""))         // truncate / remove internal server root path
                .Select(x => x.Replace("\\", "/"))                  // tip the slashes to web-convention - important, because old entries for templates used that slash
                .ToList();
        }

        private string ResolveAppPath(int appId, bool global)
        {
            var thisApp = new App(PortalSettings.Current, appId);

            if (global && !_allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            var appPath = Internal.TemplateManager.GetTemplatePathRoot(global
                ? Settings.TemplateLocations.HostFileSystem
                : Settings.TemplateLocations.PortalFileSystem
                , thisApp); // get root in global system

            appPath = System.Web.Hosting.HostingEnvironment.MapPath(appPath);
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
            // set global access security if ok...
            _allowFullAccess = UserInfo.IsSuperUser;

            path = path.Replace("/", "\\");

            var thisApp = new App(PortalSettings.Current, appId);

            if (content.Content == null)
                content.Content = "";
            var assetEditor = new AssetEditor(thisApp, path, UserInfo, PortalSettings, global);
            assetEditor.EnsureUserMayEditAsset(path);
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
            // set global access security if ok...
            _allowFullAccess = UserInfo.IsSuperUser;

            var assetEditor = (templateId != 0 && path == null)
                ? new AssetEditor(SxcContext.App, templateId, UserInfo, PortalSettings)
                : new AssetEditor(SxcContext.App, path, UserInfo, PortalSettings, global);
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
            // set global access security if ok...
            _allowFullAccess = UserInfo.IsSuperUser;

            var assetEditor = (templateId != 0 && path == null)
                ? new AssetEditor(SxcContext.App, templateId, UserInfo, PortalSettings)
                : new AssetEditor(SxcContext.App, path, UserInfo, PortalSettings, global);
            assetEditor.EnsureUserMayEditAsset();
            assetEditor.Source = template.Code;
            return true;
        }

        #endregion

        #region Helpers
        private string EnsurePathMayBeAccessed(string p, string appPath)
        {
            if (appPath == null) throw new ArgumentNullException(nameof(appPath));
            // security check, to ensure no results leak from outside the app

            if (!_allowFullAccess && !p.StartsWith(appPath))
                throw new DirectoryNotFoundException("Result was not inside the app any more - must cancel");
            return p;
        }

        #endregion


    }
}