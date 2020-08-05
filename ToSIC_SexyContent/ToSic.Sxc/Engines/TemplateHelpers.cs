using System.IO;
using System.Linq;
using System.Web;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Engines
{
    internal class TemplateHelpers
    {
        public const string RazorC = "C# Razor";
        public const string TokenReplace = "Token";

        public IApp App;
        public TemplateHelpers(IApp app)
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
                ? Path.Combine(netPlumbing.HostingEnvironment_MapPath(Settings.PortalHostDirectory) ?? "", Settings.AppsRootFolder) 
                : netPlumbing.HostingEnvironment_MapPath(App.Tenant.SxcPath) ?? "";
            var sexyFolderPath = portalPath;

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
                File.Copy(netPlumbing.HostingEnvironment_MapPath(Settings.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));

            // Create a Content folder (or App Folder)
            if (string.IsNullOrEmpty(App.Folder)) return;

            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
            contentFolder.Create();
        }
        

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public static string GetTemplatePathRoot(string locationId, IApp app)
        {
            var rootFolder = locationId == Settings.TemplateLocations.HostFileSystem
                ? netPlumbing.VirtualPathUtility_ToAbsolute(Settings.PortalHostDirectory + Settings.AppsRootFolder)
                : app.Tenant.SxcPath;
            rootFolder += "/" + app.Folder;
            return rootFolder;
        }

        public static string GetTemplateThumbnail(IApp app, string locationId, string templatePath)
        {
            var iconFile = GetTemplatePathRoot(locationId, app) + "/" + templatePath;
            iconFile = iconFile.Substring(0, iconFile.LastIndexOf(".")) + ".png";
            var exists = File.Exists(netPlumbing.HostingEnvironment_MapPath(iconFile));

            return exists ? iconFile : null;

        }
    }
}