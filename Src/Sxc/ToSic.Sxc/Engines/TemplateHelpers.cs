using System;
using System.IO;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Engines
{
    public enum PathTypes
    {
        PhysFull,
        PhysRelative,
        Link
    }

    public class TemplateHelpers: HasLog
    {
        public const string RazorC = "C# Razor";
        public const string TokenReplace = "Token";

        #region Constructor / DI

        
        private IServerPaths ServerPaths { get; }
        public IApp App;
        public TemplateHelpers(IServerPaths serverPaths, ILinkPaths linkPaths): base("Viw.Help")
        {
            ServerPaths = serverPaths;
            //_serverPaths = serverPaths;
            _linkPaths = linkPaths;
        }

        public TemplateHelpers Init(IApp app, ILog parentLog)
        {
            App = app;
            Log.LinkTo(parentLog);
            return this;
        }

        #endregion

        //protected IServerPaths ServerPaths => _serverPaths ?? (_serverPaths = Factory.Resolve<IServerPaths>());
        //private IServerPaths _serverPaths;
        private readonly ILinkPaths _linkPaths;

        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        /// <param name="templateLocation"></param>
        public void EnsureTemplateFolderExists(string templateLocation)
        {
            var wrapLog = Log.Call(templateLocation);
            var portalPath = templateLocation == Settings.TemplateLocations.HostFileSystem
                ? Path.Combine(ServerPaths.FullAppPath(Settings.PortalHostDirectory) ?? "", Settings.AppsRootFolder)
                : App.Site.AppsRootPhysicalFull ?? "";// ServerPaths.FullAppPath(App.Tenant.AppsRootPhysical) ?? "";
            var sexyFolderPath = portalPath;

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder)
            if (!sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
            {
                File.Copy(ServerPaths.FullSystemPath(Settings.WebConfigTemplatePath), Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            }

            // Create a Content folder (or App Folder)
            if (string.IsNullOrEmpty(App.Folder))
            {
                wrapLog("Folder name not given, won't create");
                return;
            }

            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, App.Folder));
            contentFolder.Create();
            wrapLog("ok");
        }

        public string AppPathRoot(bool global) =>
            AppPathRoot(global
                ? Settings.TemplateLocations.HostFileSystem
                : Settings.TemplateLocations.PortalFileSystem, PathTypes.PhysFull);

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public string AppPathRoot(string locationId, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"{locationId}, {pathType}");
            string basePath;
            var useSharedFileSystem = locationId == Settings.TemplateLocations.HostFileSystem;
            switch (pathType)
            {
                case PathTypes.Link:
                    basePath = useSharedFileSystem
                        ? _linkPaths.ToAbsolute(Settings.PortalHostDirectory, Settings.AppsRootFolder)
                        : App.Site.AppsRootLink;
                    break;
                case PathTypes.PhysRelative:
                    basePath = useSharedFileSystem
                        ? _linkPaths.ToAbsolute(Settings.PortalHostDirectory, Settings.AppsRootFolder)
                        : App.Site.AppsRootPhysical;
                    break;
                case PathTypes.PhysFull:
                    basePath = useSharedFileSystem
                        ? ServerPaths.FullAppPath(Path.Combine(Settings.PortalHostDirectory, Settings.AppsRootFolder))
                        : App.Site.AppsRootPhysicalFull;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            var finalPath = Path.Combine(basePath, App.Folder);
            return wrapLog(finalPath, finalPath);
        }

        public string IconPathOrNull(IView view, PathTypes type)
        {
            // 1. Check if the file actually exists
            //var iconFile = ViewPath(view);
            var iconFile = IconPath(view, PathTypes.PhysFull);
            var exists = File.Exists(iconFile);

            // 2. Return as needed
            return exists ? IconPath(view, type) : null;
        }

        private string IconPath(IView view, PathTypes type)
        {
            var viewPath1 = ViewPath(view, type);
            return viewPath1.Substring(0, viewPath1.LastIndexOf(".", StringComparison.Ordinal)) + ".png";
        }

        public string ViewPath(IView view, PathTypes type) => AppPathRoot(view.Location, type) + "/" + view.Path;
    }
}