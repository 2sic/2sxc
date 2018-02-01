using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace ToSic.SexyContent.Internal
{
    public class TemplateHelpers
    {
        public const string RazorC = "C# Razor";
        public const string TokenReplace = "Token";

        public App App;
        public TemplateHelpers(App app)
        {
            App = app;
        }
        
        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        /// <param name="templateLocation"></param>
        public void EnsureTemplateFolderExists(string templateLocation)
        {
            var portalPath = templateLocation == Settings.TemplateLocations.HostFileSystem 
                ? Path.Combine(HostingEnvironment.MapPath(Settings.PortalHostDirectory), Settings.TemplateFolder) 
                : App.Tennant.RootPath;//.Settings.HomeDirectoryMapPath;
            var sexyFolderPath = portalPath;// Path.Combine(portalPath, Settings.TemplateFolder);

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            if (!sexyFolder.Exists)
                sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles("web.config").Any())
                File.Copy(HostingEnvironment.MapPath(Settings.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));

            // Create a Content folder (or App Folder)
            if (!String.IsNullOrEmpty(App.Folder))
            {
                var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
                if (!contentFolder.Exists)
                    contentFolder.Create();
            }

        }
        

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public static string GetTemplatePathRoot(string locationId, App app)
        {
            var rootFolder = locationId == Settings.TemplateLocations.HostFileSystem
                ? Settings.PortalHostDirectory + Settings.TemplateFolder
                : app.Tennant.RootPath;//.Settings.HomeDirectory;
            rootFolder += /*Settings.TemplateFolder +*/ "/" + app.Folder;
            return rootFolder;
        }

        public static string GetTemplateThumbnail(App app, string locationId, string templatePath)
        {
            var iconFile = GetTemplatePathRoot(locationId, app) + "/" + templatePath;
            iconFile = iconFile.Substring(0, iconFile.LastIndexOf(".")) + ".png";
            var exists = File.Exists(HostingEnvironment.MapPath(iconFile));

            return exists ? iconFile : null;

        }
    }
}