using System.IO;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Apps.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFolderInitializer(IServerPaths serverPaths, IGlobalConfiguration globalConfiguration, ISite site)
    : ServiceBase("Viw.Help", connect: [serverPaths, globalConfiguration, site])
{

    /// <summary>
    /// Creates a directory and copies the needed web.config for razor files
    /// if the directory does not exist.
    /// </summary>
    public void EnsureTemplateFolderExists(string appStateFolder, bool isShared)
    {
        var l = Log.Fn($"{isShared}");
        var portalPath = isShared
            ? serverPaths.FullAppPath(globalConfiguration.SharedAppsFolder)
            : site.AppsRootPhysicalFull ?? "";

        var sxcFolder = new DirectoryInfo(portalPath);

        // Create 2sxc folder if it does not exists
        sxcFolder.Create();

        // Create web.config (copy from DesktopModules folder, but only if is there, and for Oqtane is not)
        // Note that DNN needs it because many razor file don't use @inherits and the web.config contains the default class
        // but in Oqtane we'll require that to work
        var webConfigTemplateFilePath =
            Path.Combine(globalConfiguration.GlobalFolder, SpecialFiles.WebConfigTemplateFile);
        if (File.Exists(webConfigTemplateFilePath) && !sxcFolder.GetFiles(SpecialFiles.WebConfigFileName).Any())
            File.Copy(webConfigTemplateFilePath, Path.Combine(sxcFolder.FullName, SpecialFiles.WebConfigFileName));

        // Create a Content folder (or App Folder)
        if (string.IsNullOrEmpty(appStateFolder))
        {
            l.Done("Folder name not given, won't create");
            return;
        }

        var contentFolder = new DirectoryInfo(Path.Combine(sxcFolder.FullName, appStateFolder));
        contentFolder.Create();

        var appDataProtectedFolder =
            new DirectoryInfo(Path.Combine(contentFolder.FullName, Eav.Constants.AppDataProtectedFolder));
        appDataProtectedFolder.Create();

        var appJsonTemplateFilePath =
            Path.Combine(globalConfiguration.AppDataTemplateFolder, Eav.Constants.AppJson);
        if (File.Exists(appJsonTemplateFilePath) && !appDataProtectedFolder.GetFiles(Eav.Constants.AppJson).Any())
            File.Copy(appJsonTemplateFilePath,
                Path.Combine(appDataProtectedFolder.FullName, Eav.Constants.AppJson));
        l.Done("ok");
    }

}