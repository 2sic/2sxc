using System;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using static System.StringComparison;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Engines
{
    public enum PathTypes
    {
        PhysFull,
        PhysRelative,
        Link
    }

    public class AppPathHelpers: HasLog
    {
        #region Constructor / DI

        private IServerPaths ServerPaths { get; }
        public IApp App;
        public AppPathHelpers(IServerPaths serverPaths, IGlobalConfiguration globalConfiguration, Lazy<IValueConverter> iconConverterLazy): base("Viw.Help")
        {
            ServerPaths = serverPaths;
            _globalConfiguration = globalConfiguration;
            _iconConverterLazy = iconConverterLazy;
        }

        public AppPathHelpers Init(IApp app, ILog parentLog)
        {
            App = app;
            Log.LinkTo(parentLog);
            return this;
        }

        #endregion

        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly Lazy<IValueConverter> _iconConverterLazy;

        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        public void EnsureTemplateFolderExists(bool isShared)
        {
            var wrapLog = Log.Call($"{isShared}");
            var portalPath = isShared
                ? Path.Combine(ServerPaths.FullAppPath(_globalConfiguration.GlobalSiteFolder) ?? "", Settings.AppsRootFolder)
                : App.Site.AppsRootPhysicalFull ?? "";

            var sexyFolder = new DirectoryInfo(portalPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder, but only if is there, and for Oqtane is not)
            // Note that DNN needs it because many razor file don't use @inherits and the web.config contains the default class
            // but in Oqtane we'll require that to work
            var webConfigTemplateFilePath = Path.Combine(_globalConfiguration.GlobalFolder, Settings.WebConfigTemplateFile);
            if (File.Exists(webConfigTemplateFilePath) && !sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
                File.Copy(webConfigTemplateFilePath, Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));

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

        public string AppPathRoot(bool global) => AppPathRoot(global, PathTypes.PhysFull);

        /// <summary>
        /// Returns the location where Templates are stored for the current app
        /// </summary>
        public string AppPathRoot(bool global, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"{global}, {pathType}");
            string basePath;
            switch (pathType)
            {
                case PathTypes.Link:
                    basePath = global
                        ? Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, App.Folder).ToAbsolutePathForwardSlash()
                        : App.Path;
                    break;
                case PathTypes.PhysRelative:
                    basePath = global
                        ? Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, App.Folder).ToAbsolutePathForwardSlash()
                        : Path.Combine(App.Site.AppsRootPhysical, App.Folder);
                    break;
                case PathTypes.PhysFull:
                    basePath = global
                        ? ServerPaths.FullAppPath(Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, App.Folder))
                        : Path.Combine(App.Site.AppsRootPhysicalFull, App.Folder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }

            return wrapLog(basePath, basePath);
        }

        public string IconPathOrNull(IView view, PathTypes type)
        {
            var wrapLog = Log.Call<string>();
            // 1. Check if the file actually exists
            var iconFile = IconPath(view, PathTypes.PhysFull);
            var exists = File.Exists(iconFile);

            // 2. Return as needed
            var result = exists ? IconPath(view, type) : null;
            return wrapLog(result ?? "not found", result);
        }

        private string IconPath(IView view, PathTypes type)
        {
            // See if we have an icon - but only if we need the link
            if(!string.IsNullOrWhiteSpace(view.Icon))
            {
                var iconInConfig = view.Icon;
                
                // If we have the App:Path in front, replace as expected, but never on global
                if (HasAppPathToken(iconInConfig))
                    return AppPathTokenReplace(iconInConfig, AppPathRoot(false, type), AppPathRoot(true, type));

                // If not, we must assume it's file:## placeholder and we can only convert to relative link
                if (type == PathTypes.Link) return _iconConverterLazy.Value.ToValue(iconInConfig, view.Guid);

                // Otherwise ignore the request and proceed by standard
            }

            var viewPath1 = ViewPath(view, type);
            return viewPath1.Substring(0, viewPath1.LastIndexOf(".", Ordinal)) + ".png";
        }

        public string ViewPath(IView view, PathTypes type) => Path.Combine(AppPathRoot(view.IsShared, type), view.Path);

        public static bool HasAppPathToken(string value)
        {
            value = value ?? "";
            return value.StartsWith(AppConstants.AppPathPlaceholder, InvariantCultureIgnoreCase)
                || value.StartsWith(AppConstants.AppPathSharedPlaceholder, InvariantCultureIgnoreCase);
        }

        public static string AppPathTokenReplace(string value, string appPath, string appPathShared)
        {
            value = value ?? "";
            if (value.StartsWith(AppConstants.AppPathPlaceholder, InvariantCultureIgnoreCase))
                return appPath + value.After(AppConstants.AppPathPlaceholder);
            if (value.StartsWith(AppConstants.AppPathSharedPlaceholder, InvariantCultureIgnoreCase))
                return appPathShared + value.After(AppConstants.AppPathSharedPlaceholder);
            return value;
        }

        /// <summary>
        /// Returns the location where module global folder web assets are stored
        /// </summary>
        public string AssetsLocation(string path, PathTypes pathType)
        {
            var wrapLog = Log.Call<string>($"path:{path},pathType:{pathType}");
            var assetPath = Path.Combine(_globalConfiguration.AssetsVirtualUrl.Backslash(), path);
            string assetLocation;
            switch (pathType)
            {
                case PathTypes.Link:
                    assetLocation = assetPath.ToAbsolutePathForwardSlash();
                    break;
                case PathTypes.PhysRelative:
                    assetLocation = assetPath.TrimStart('~').Backslash();
                    break;
                case PathTypes.PhysFull:
                    assetLocation = ServerPaths.FullAppPath(assetPath).Backslash();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathType), pathType, null);
            }
            return wrapLog("ok", assetLocation);
        }
    }
}