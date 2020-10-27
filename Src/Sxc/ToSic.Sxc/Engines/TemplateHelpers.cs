using System;
using System.IO;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class TemplateHelpers
    {
        public const string RazorC = "C# Razor";
        public const string TokenReplace = "Token";

        public IApp App;
        public TemplateHelpers(IHttp http, IServerPaths serverPaths, ILinkPaths linkPaths)
        {
            _http = http;
            _serverPaths = serverPaths;
            _linkPaths = linkPaths;
        }

        public TemplateHelpers Init(IApp app)
        {
            App = app;
            return this;
        }

        protected IHttp Http => _http ?? (_http = Factory.Resolve<IHttp>());
        private IHttp _http;
        protected IServerPaths ServerPaths => _serverPaths ?? (_serverPaths = Factory.Resolve<IServerPaths>());
        private IServerPaths _serverPaths;
        private readonly ILinkPaths _linkPaths;

        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        /// <param name="templateLocation"></param>
        public void EnsureTemplateFolderExists(string templateLocation)
        {
            var portalPath = templateLocation == Settings.TemplateLocations.HostFileSystem 
                ? Path.Combine(ServerPaths.FullAppPath(Settings.PortalHostDirectory) ?? "", Settings.AppsRootFolder) 
                : ServerPaths.FullAppPath(App.Tenant.AppsRootPhysical) ?? "";
            var sexyFolderPath = portalPath;

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
                File.Copy(ServerPaths.FullSystemPath(Settings.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));

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
                ? _linkPaths.ToAbsolute(Settings.PortalHostDirectory, Settings.AppsRootFolder)
                : App.Tenant.AppsRootPhysical;
            rootFolder += "\\" + App.Folder;
            if (fullPath) rootFolder = ServerPaths.FullAppPath(rootFolder);
            return rootFolder;
        }

        public string ViewThumbnail(IView view)
        {
            var iconFile = ViewPath(view);
            iconFile = ViewIconFileName(iconFile);
            var exists = File.Exists(ServerPaths.FullAppPath(iconFile));

            return exists ? iconFile : null;
        }

        public string ViewIconFileName(string viewPath) 
            => viewPath.Substring(0, viewPath.LastIndexOf(".", StringComparison.Ordinal)) + ".png";

        public string ViewPath(IView view) => AppPathRoot(view.Location) + "/" + view.Path;
    }
}