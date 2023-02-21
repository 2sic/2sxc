using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Apps.Paths
{
    public class AppFolderInitializer : ServiceBase
    {
        #region Constructor / DI

        public AppFolderInitializer(IServerPaths serverPaths, IGlobalConfiguration globalConfiguration, ISite site): base("Viw.Help")
        {
            ConnectServices(
                ServerPaths = serverPaths,
                _globalConfiguration = globalConfiguration,
                _site = site
            );
        }
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly ISite _site;
        private IServerPaths ServerPaths { get; }

        #endregion


        /// <summary>
        /// Creates a directory and copies the needed web.config for razor files
        /// if the directory does not exist.
        /// </summary>
        public void EnsureTemplateFolderExists(AppState appState, bool isShared) => Log.Do($"{isShared}", () =>
        {
            var portalPath = isShared
                ? ServerPaths.FullAppPath(_globalConfiguration.SharedAppsFolder)
                : _site.AppsRootPhysicalFull ?? "";

            var sxcFolder = new DirectoryInfo(portalPath);

            // Create 2sxc folder if it does not exists
            sxcFolder.Create();

            // Create web.config (copy from DesktopModules folder, but only if is there, and for Oqtane is not)
            // Note that DNN needs it because many razor file don't use @inherits and the web.config contains the default class
            // but in Oqtane we'll require that to work
            var webConfigTemplateFilePath =
                Path.Combine(_globalConfiguration.GlobalFolder, Constants.WebConfigTemplateFile);
            if (File.Exists(webConfigTemplateFilePath) && !sxcFolder.GetFiles(Constants.WebConfigFileName).Any())
                File.Copy(webConfigTemplateFilePath, Path.Combine(sxcFolder.FullName, Constants.WebConfigFileName));

            // Create a Content folder (or App Folder)
            if (string.IsNullOrEmpty(appState.Folder))
                return "Folder name not given, won't create";

            var contentFolder = new DirectoryInfo(Path.Combine(sxcFolder.FullName, appState.Folder));
            contentFolder.Create();

            var appDataProtectedFolder =
                new DirectoryInfo(Path.Combine(contentFolder.FullName, Eav.Constants.AppDataProtectedFolder));
            appDataProtectedFolder.Create();

            var appJsonTemplateFilePath =
                Path.Combine(_globalConfiguration.AppDataTemplateFolder, Eav.Constants.AppJson);
            if (File.Exists(appJsonTemplateFilePath) && !appDataProtectedFolder.GetFiles(Eav.Constants.AppJson).Any())
                File.Copy(appJsonTemplateFilePath,
                    Path.Combine(appDataProtectedFolder.FullName, Eav.Constants.AppJson));
            return "ok";
        });

    }
}