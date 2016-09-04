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

        [HttpGet]
        public  List<string> List(int appId, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
        {
            // make sure the folder-param is not null if it's missing
            if (string.IsNullOrEmpty(path)) path = "";

            //var app = ;
            var appPath = (new App(PortalSettings.Current, appId)).PhysicalPath;
            var fullPath = Path.Combine(appPath, path);

            // make sure the resulting path is still inside 2sxc
            if(fullPath.IndexOf("2sxc", StringComparison.InvariantCultureIgnoreCase) == -1)
                throw new DirectoryNotFoundException("Folder was not inside 2sxc-scope any more - must cancel");

            if (!Directory.Exists(fullPath)) return new List<string>();

            var opt = withSubfolders 
                ? SearchOption.AllDirectories 
                : SearchOption.TopDirectoryOnly;

            return (returnFolders
                ? Directory.GetDirectories(fullPath, mask, opt)
                    .Select(Path.GetDirectoryName)
                : Directory.GetFiles(fullPath, mask, opt)
                    .Select(Path.GetFullPath)
                )
                .Select(p => EnsurePathIsInsideApp(p, appPath))
                .Select(x => x.Replace(appPath + "\\", ""))
                .ToList();
        }


        public void Create(int appId, string path, string content = "")
        {
            
        }

        #endregion

        #region Template --> later neutralize to standard asset-editing
        [HttpGet]
        public AssetEditInfo Asset(int templateId = 0, string path = null)
        {
            var assetEditor = (templateId != 0 && path == null)
                ? new AssetEditor(SxcContext, templateId, UserInfo, PortalSettings)
                : new AssetEditor(SxcContext, path, UserInfo, PortalSettings);
            assetEditor.EnsureUserMayEditAsset();
            return assetEditor.EditInfoWithSource;
        }

        [HttpPost]
        public bool Asset([FromBody] AssetEditInfo template,[FromUri] int templateId = 0, [FromUri] string path = null)
        {
            var assetEditor = (templateId != 0 && path == null)
                ? new AssetEditor(SxcContext, templateId, UserInfo, PortalSettings)
                : new AssetEditor(SxcContext, path, UserInfo, PortalSettings);
            assetEditor.EnsureUserMayEditAsset();
            assetEditor.Source = template.Code;
            return true;
        }

        #endregion

        #region Helpers
        private static string EnsurePathIsInsideApp(string p, string appPath)
        {
            if (appPath == null) throw new ArgumentNullException(nameof(appPath));
            // security check, to ensure no results leak from outside the app
            if (p.IndexOf(appPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                throw new DirectoryNotFoundException("Result was not inside the app any more - must cancel");
            return p;
        }

        #endregion


    }
}