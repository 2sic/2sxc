using System;
using System.IO;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Blocks;

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
        public TemplateHelpers(IServerPaths serverPaths, IGlobalConfiguration globalConfiguration, Lazy<IValueConverter> iconConverterLazy): base("Viw.Help")
        {
            ServerPaths = serverPaths;
            _globalConfiguration = globalConfiguration;
            _iconConverterLazy = iconConverterLazy;
        }

        public TemplateHelpers Init(IApp app, ILog parentLog)
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
            var sexyFolderPath = portalPath;

            var sexyFolder = new DirectoryInfo(sexyFolderPath);

            // Create 2sxc folder if it does not exists
            sexyFolder.Create();

            // Create web.config (copy from DesktopModules folder, but only if is there, and for Oqtane is not)
            // Note that DNN needs it because many razor file don't use @inherits and the web.config contains the default class
            // but in Oqtane we'll require that to work
            var webConfigTempletFilePath = Path.Combine(_globalConfiguration.GlobalFolder, Settings.WebConfigTemplateFile);
            if (File.Exists(webConfigTempletFilePath) && !sexyFolder.GetFiles(Settings.WebConfigFileName).Any())
            {
                File.Copy(webConfigTempletFilePath,
                    Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
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
                        //? _linkPaths.ToAbsolute(Path.Combine(Settings.PortalHostDirectory, Settings.AppsRootFolder))
                        ? Path.Combine(_globalConfiguration.GlobalSiteFolder, Settings.AppsRootFolder, App.Folder).ToAbsolutePathForwardSlash()
                        : App.Path;
                    break;
                case PathTypes.PhysRelative:
                    basePath = global
                        //? _linkPaths.ToAbsolute(Path.Combine(Settings.PortalHostDirectory, Settings.AppsRootFolder))
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

            //var finalPath = basePath.Contains(LinkPaths.AppFolderPlaceholder)
            //    ? basePath.Replace(LinkPaths.AppFolderPlaceholder, App.Folder)
            //    : Path.Combine(basePath, App.Folder);
            return wrapLog(basePath, basePath);
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
            // See if we have an icon - but only if we need the link
            if(!string.IsNullOrWhiteSpace(view.Icon))
            {
                var iconInConfig = view.Icon;
                
                // If we have the App:Path in front, replace as expected, but never on global
                if (AppPathTokenDetected(iconInConfig))
                    return AppPathTokenReplace(iconInConfig, AppPathRoot(false, type));
                // AppPathRoot(false, type) + iconInConfig.Substring(AppAssets.AppPathPlaceholder.Length);
                
                // If not, we must assume it's file:## placeholder and we can only convert to relative link
                if (type == PathTypes.Link) return _iconConverterLazy.Value.ToValue(iconInConfig, view.Guid);

                // Otherwise ignore the request and proceed by standard
            }

            var viewPath1 = ViewPath(view, type);
            return viewPath1.Substring(0, viewPath1.LastIndexOf(".", StringComparison.Ordinal)) + ".png";
        }

        public string ViewPath(IView view, PathTypes type) => Path.Combine(AppPathRoot(view.IsShared, type), view.Path);

        public static bool AppPathTokenDetected(string iconInConfig) =>
            (iconInConfig ?? "").StartsWith(AppAssets.AppPathPlaceholder, StringComparison.OrdinalIgnoreCase);

        public static string AppPathTokenReplace(string iconInConfig, string appPath) =>
            AppPathTokenDetected(iconInConfig)
                ? appPath + iconInConfig.Substring(AppAssets.AppPathPlaceholder.Length)
                : iconInConfig;
    }
}