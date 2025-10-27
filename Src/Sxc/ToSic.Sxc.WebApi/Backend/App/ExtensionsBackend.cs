using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Security.Files;
using ToSic.Eav.Sys;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<IJsonService> jsonLazy)
 : ServiceBase("Bck.Exts", connect: [appReadersLazy, site, appPathSvc, jsonLazy])
{
    public ExtensionsResultDto GetExtensions(int appId)
    {
        var l = Log.Fn<ExtensionsResultDto>($"a#{appId}");
        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var root = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);

        var list = new List<ExtensionDto>();
        if (Directory.Exists(root))
        {
            foreach (var dir in Directory.GetDirectories(root))
            {
                var folderName = Path.GetFileName(dir);
                var appData = Path.Combine(dir, FolderConstants.DataFolderProtected);
                var jsonPath = Path.Combine(appData, FolderConstants.AppExtensionJsonFile);
                object? configuration = null;
                try
                {
                    if (File.Exists(jsonPath))
                    {
                        var json = File.ReadAllText(jsonPath);
                        configuration = jsonLazy.Value.ToObject(json);
                    }
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                }

                configuration ??= new JsonObject(); // ensure at least an empty object

                // If configuration is null, the DTO will omit it (see JsonIgnore on property)
                list.Add(new ExtensionDto { Folder = folderName, Configuration = configuration });
            }
        }

        var result = new ExtensionsResultDto { Extensions = list };
        return l.ReturnAsOk(result);
    }

    public bool SaveExtension(int zoneId, int appId, string name, JsonElement configuration)
    {
        var l = Log.Fn<bool>($"z:{zoneId}, a:{appId}, f:'{name}'");
        if (string.IsNullOrWhiteSpace(name)) return l.ReturnFalse("no folder");

        // Basic validation to ensure folder is a simple directory name
        name = name.Trim();
        if (!IsValidFolderName(name))
            return l.ReturnFalse($"invalid folder name:'{name}'");

        var appReader = appReadersLazy.Value.Get(appId);
        var appPaths = appPathSvc.Get(appReader, site);
        var dir = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder, name);
        if (!Directory.Exists(dir))
            return l.ReturnFalse($"extension folder:'{dir}' doesn't exist");

        var appData = Path.Combine(dir, FolderConstants.DataFolderProtected);
        var jsonPath = Path.Combine(appData, FolderConstants.AppExtensionJsonFile);

        try
        {
            Directory.CreateDirectory(appData);
            var json = configuration.GetRawText();
            File.WriteAllText(jsonPath, json, new UTF8Encoding(false));
            return l.ReturnTrue("saved");
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    /// <summary>
    /// Install an extension provided as a ZIP stream into /extensions/{folder}.
    /// </summary>
    public bool InstallExtensionZip(int zoneId, int appId, Stream zipStream, string? name = null, bool overwrite = false, string? originalZipFileName = null)
    {
        var l = Log.Fn<bool>($"z:{zoneId}, a:{appId}, overwrite:{overwrite}, pref:'{name}', ofn:'{originalZipFileName}'");
        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var appPaths = appPathSvc.Get(appReader, site);

            var extensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);
            Directory.CreateDirectory(extensionsRoot);

            // Resolve folderName: prefer explicit, else use zip file name (base name, no extension)
            var folderName = !string.IsNullOrWhiteSpace(name)
                ? name.Trim()
                : DeriveFolderNameFromZipFileName(originalZipFileName);

            if (string.IsNullOrWhiteSpace(folderName))
                return l.ReturnFalse("no folder name - provide 'folder' or upload with a valid filename");

            if (!IsValidFolderName(folderName))
                return l.ReturnFalse($"invalid folder name:'{folderName}'");

            var targetRoot = Path.Combine(extensionsRoot, folderName);

            // Handle existing folder according to overwrite flag
            if (Directory.Exists(targetRoot))
            {
                if (!overwrite)
                    return l.ReturnFalse("target exists - set overwrite to true");
                try
                {
                    Zipping.TryToDeleteDirectory(targetRoot, l);
                }
                catch (Exception exDel)
                {
                    Log.Ex(exDel);
                    return l.ReturnFalse("failed to delete existing target");
                }
            }

            Directory.CreateDirectory(targetRoot);

            // Extract using EAV Zip implementation - enforce no folder entries and code-file restrictions
            try
            {
                new Zipping(l).ExtractZipStream(zipStream, targetRoot, allowCodeImport: true, ignoreFolderEntries: true);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return l.ReturnFalse("invalid zip or not allowed");
            }

            return l.ReturnTrue($"installed '{folderName}'");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    private static string? DeriveFolderNameFromZipFileName(string? originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName)) return null;
        var baseName = Path.GetFileNameWithoutExtension(originalFileName.Trim());
        return string.IsNullOrWhiteSpace(baseName) ? null : baseName;
    }

    private static bool IsValidFolderName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        // disallow path separators and traversal early
        if (name.Contains(Path.DirectorySeparatorChar) || name.Contains(Path.AltDirectorySeparatorChar)) return false;
        if (name.Equals(".") || name.Equals("..")) return false;
        if (name.Contains("..")) return false;

        // must be a plain file/folder segment
        if (name != Path.GetFileName(name)) return false;

        // Use sanitizer to determine if name is acceptable.
        var sanitized = FileNames.SanitizeFileName(name);

        // If sanitization changes the name or returns the safe placeholder, reject it
        if (string.IsNullOrWhiteSpace(sanitized) || sanitized == FileNames.SafeChar) return false;
        if (!string.Equals(sanitized, name, StringComparison.Ordinal)) return false;

        // Additionally, disallow names that look like risky extensions (e.g. "file.exe")
        if (FileNames.IsKnownRiskyExtension(name)) return false;

        return true;
    }
}
