using System;
using System.IO;
using System.Linq;
using ToSic.Eav;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class TemplateHelpers
    {
        public const string RazorC = "C# Razor";
        public const string TokenReplace = "Token";

        public IApp App;
        public TemplateHelpers(IHttp http)
        {
            _http = http;
        }

        public TemplateHelpers Init(IApp app)
        {
            App = app;
            return this;
        }

        protected IHttp Http => _http ?? (_http = Factory.Resolve<IHttp>());
        private IHttp _http;
        
        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        /// <param name="templateLocation"></param>
        public void EnsureTemplateFolderExists(string templateLocation)
        {
            var portalPath = templateLocation == Settings.TemplateLocations.HostFileSystem 
                ? Path.Combine(Http.MapPath(Settings.PortalHostDirectory) ?? "", Settings.AppsRootFolder) 
                : Http.MapPath(App.Tenant.AppsRoot) ?? "";
            var sexyFolderPath = portalPath;

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
                File.Copy(Http.MapPath(Settings.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));

            // Create a Content folder (or App Folder)
            if (string.IsNullOrEmpty(App.Folder)) return;

            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
            contentFolder.Create();
        }

        public string AppPathRoot(bool global) =>
            AppPathRoot(global
                ? Settings.TemplateLocations.HostFileSystem
                : Settings.TemplateLocations.PortalFileSystem, true);

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public string AppPathRoot(string locationId, bool fullPath = false)
        {
            var rootFolder = locationId == Settings.TemplateLocations.HostFileSystem
                ? _http.ToAbsolute(Settings.PortalHostDirectory + Settings.AppsRootFolder)
                : App.Tenant.AppsRoot;
            rootFolder += "\\" + App.Folder;
            if (fullPath) rootFolder = _http.MapPath(rootFolder);
            return rootFolder;
        }

        public string ViewThumbnail(IView view)
        {
            var iconFile = ViewPath(view);
            iconFile = ViewIconFileName(iconFile);
            var exists = File.Exists(_http.MapPath(iconFile));

            return exists ? iconFile : null;
        }

        public string ViewIconFileName(string viewPath) 
            => viewPath.Substring(0, viewPath.LastIndexOf(".", StringComparison.Ordinal)) + ".png";

        public string ViewPath(IView view) => AppPathRoot(view.Location) + "/" + view.Path;
    }
}