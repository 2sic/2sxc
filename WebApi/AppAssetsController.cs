using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class AppAssetsController : DnnApiController
	{
        // todo: Create the create-api
        // todo: create the delete-file/folder api
        // todo: create the copy file api

        #region Dialog Helpers

        [HttpGet]
        public List<string> List(int appId, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false)
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
                .Select(p => EnsureScopeIsInsideApp(p, appPath))
                .Select(x => x.Replace(appPath, ""))
                .ToList();
        }

        private static string EnsureScopeIsInsideApp(string p, string appPath)
        {
            // security check, to ensure no results leak from outside the app
            if (p.IndexOf(appPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                throw new DirectoryNotFoundException("Result was not inside the app any more - must cancel");
            return p;
        }

        #endregion


    }
}