using System.Text.Json;
using ToSic.Eav.Apps.Sys.Paths;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsWriterBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc)
    : ServiceBase("Bck.ExtWrite", connect: [appReadersLazy, site, appPathSvc])
{
    public bool SaveExtension(int appId, string name, JsonElement configuration)
    {
        var l = Log.Fn<bool>($"a:{appId}, f:'{name}'");

        if (string.IsNullOrWhiteSpace(name))
            return l.ReturnFalse("no folder");

        name = name.Trim();
        if (!ExtensionFolderNameValidator.IsValid(name))
            return l.ReturnFalse($"invalid folder name:'{name}'");

        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var dir = Path.Combine(appPaths.PhysicalPath, ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder, name);
        if (!Directory.Exists(dir))
            return l.ReturnFalse($"extension folder:'{dir}' doesn't exist");

        var appData = Path.Combine(dir, ToSic.Eav.Sys.FolderConstants.DataFolderProtected);
        var jsonPath = Path.Combine(appData, ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile);

        try
        {
            Directory.CreateDirectory(appData);
            using var jsonDoc = JsonDocument.Parse(configuration.GetRawText());
            var json = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json, new System.Text.UTF8Encoding(false));
            return l.ReturnTrue("saved");
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            return l.ReturnFalse("error");
        }
    }
}
