using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class AppAssetsController
    {

        private void FullDirList(DirectoryInfo dir, string searchPattern, List<DirectoryInfo> folders, List<FileInfo> files, SearchOption opt)
        {
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
                // We already got an error trying to access dir so dont try to access it again
                return;
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            if (opt != SearchOption.AllDirectories) return;

            foreach (var d in dir.GetDirectories())
            {
                try
                {
                    // todo: possibly re-include subfolders with ".data"
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


        private string ResolveAppPath(int appId, bool global, bool allowFullAccess)
        {
            var thisApp = Factory.Resolve<Apps.App>().InitNoData(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), Log);

            if (global && !allowFullAccess)
                throw new NotSupportedException("only host user may access global files");

            var appPath = TemplateHelpers.GetTemplatePathRoot(
                global
                    ? Settings.TemplateLocations.HostFileSystem
                    : Settings.TemplateLocations.PortalFileSystem
                , thisApp); // get root in global system

            appPath = global::System.Web.Hosting.HostingEnvironment.MapPath(appPath);
            return appPath;
        }


        private static string SanitizePathAndContent(string path, ContentHelper content)
        {
            var name = Path.GetFileName(path);
            var folder = Path.GetDirectoryName(path);
            var ext = Path.GetExtension(path);

            // not sure what this is for, since I believe code should only get here if there was an ext and it's cshtml
            // probably just to prevent some very unexpected create
            if (name == null) name = "missing-name.txt";

            switch (ext?.ToLowerInvariant())
            {
                // .cs files - usually API controllers
                case AssetEditor.CsExtension:
                    if ((folder?.ToLower().IndexOf(AssetEditor.CsApiFolder, StringComparison.Ordinal) ?? -1) > -1)
                    {
                        var nameWithoutExt = name.Substring(0, name.Length - ext.Length);
                        content.Content =
                            AssetEditor.DefaultCsBody.Replace(AssetEditor.CsApiTemplateControllerName, nameWithoutExt);
                    }
                    break;

                // .cshtml files (razor) or .code.cshtml (razor code-behind)
                case AssetEditor.CshtmlExtension:
                {
                    // ensure all .cshtml start with "_"
                    if (!name.StartsWith(AssetEditor.CshtmlPrefix))
                    {
                        name = AssetEditor.CshtmlPrefix + name;
                        path = (string.IsNullOrWhiteSpace(folder) ? "" : folder + "\\") + name;
                    }

                    // first check the code-extension, because it's longer but also would contain the non-code extension
                    if (name.EndsWith(AssetEditor.CodeCshtmlExtension)) 
                        content.Content = AssetEditor.DefaultCodeCshtmlBody;
                    else if (name.EndsWith(AssetEditor.CshtmlExtension))
                        content.Content = AssetEditor.DefaultCshtmlBody;
                    break;
                }

                // .html files (Tokens)
                case AssetEditor.TokenHtmlExtension:
                    content.Content = AssetEditor.DefaultTokenHtmlBody;
                    break;
            }

            return path;
        }

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
