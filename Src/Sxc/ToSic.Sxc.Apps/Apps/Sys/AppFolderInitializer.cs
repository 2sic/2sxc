using ToSic.Eav.Context;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.Sys;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppFolderInitializer(IServerPaths serverPaths, IGlobalConfiguration globalConfiguration, ISite site)
    : ServiceBase("Viw.Help", connect: [serverPaths, globalConfiguration, site])
{

    /// <summary>
    /// Creates a directory and copies the needed web.config for razor files
    /// if the directory does not exist.
    /// </summary>
    public void EnsureTemplateFolderExists(string appFolder, bool isShared)
    {
        var l = Log.Fn($"{isShared}");
        var portalPath = isShared
            ? serverPaths.FullAppPath(globalConfiguration.SharedAppsFolder())
            : site.AppsRootPhysicalFull ?? "";

        var sxcFolder = new DirectoryInfo(portalPath);

        // Create 2sxc folder if it does not exists
        sxcFolder.Create();

        // Create a Content folder (or App Folder)
        if (string.IsNullOrEmpty(appFolder))
        {
            l.Done("Folder name not given, won't create");
            return;
        }

        var contentFolder = new DirectoryInfo(Path.Combine(sxcFolder.FullName, appFolder));
        contentFolder.Create();

        var appDataProtectedFolder =
            new DirectoryInfo(Path.Combine(contentFolder.FullName, FolderConstants.AppDataProtectedFolder));
        appDataProtectedFolder.Create();

        var appJsonTemplateFilePath =
            Path.Combine(globalConfiguration.AppDataTemplateFolder(), FolderConstants.AppJson);
        if (File.Exists(appJsonTemplateFilePath) && !appDataProtectedFolder.GetFiles(FolderConstants.AppJson).Any())
            File.Copy(appJsonTemplateFilePath,
                Path.Combine(appDataProtectedFolder.FullName, FolderConstants.AppJson));
        l.Done("ok");
    }

}