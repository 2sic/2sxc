using System.Text;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionWriterBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.ExtWrite", connect: [appReadersLazy, site, appPathSvc])
{
    // Remove previous local serializer & use shared helper
    public bool SaveConfiguration(int appId, string name, ExtensionManifest manifest)
    {
        var l = Log.Fn<bool>($"a:{appId}, f:'{name}'");
        if (name.IsEmpty())
            return l.ReturnFalse("no folder");
        name = name.Trim();
        if (!ExtensionFolderNameValidator.IsValid(name))
            return l.ReturnFalse($"invalid folder name:'{name}'");
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var dir = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder, name);
        
        // Create extension folder if it doesn't exist
        Directory.CreateDirectory(dir);
        
        var appData = Path.Combine(dir, FolderConstants.DataFolderProtected);
        var jsonPath = Path.Combine(appData, FolderConstants.AppExtensionJsonFile);
        try
        {
            Directory.CreateDirectory(appData);
            var json = ExtensionManifestSerializer.Serialize(manifest);
            File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
            return l.ReturnTrue("saved");
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            return l.ReturnFalse("error");
        }
    }
}
