using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Security.Files;
using ToSic.Eav.Sys;
using ToSic.Sxc.Services;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    LazySvc<IJsonService> jsonLazy,
    IGlobalConfiguration globalConfiguration)
    : ServiceBase("Bck.Exts", connect: [appReadersLazy, site, appPathSvc, jsonLazy, globalConfiguration])
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
    /// Expected zip layout: extensions/{extensionName}/App_Data/extension.json + assets.
    /// If name not provided, derive from zip filename or use the single subfolder under 'extensions'.
    /// </summary>
    public bool InstallExtensionZip(int zoneId, int appId, Stream zipStream, string? name = null, bool overwrite = false, string? originalZipFileName = null)
    {
        var l = Log.Fn<bool>($"z:{zoneId}, a:{appId}, overwrite:{overwrite}, pref:'{name}', ofn:'{originalZipFileName}'");
        string? tempDir = null;
        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var appPaths = appPathSvc.Get(appReader, site);
            var extensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);
            Directory.CreateDirectory(extensionsRoot);

            // Use configured temporary folder root (same pattern as app import)
            tempDir = Path.Combine(globalConfiguration.TemporaryFolder(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            // Extract entire zip to temp; allowCodeImport:true so front-end assets (.js/.css) are not blocked.
            try
            {
                new Zipping(l).ExtractZipStream(zipStream, tempDir, allowCodeImport: true);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return l.ReturnFalse("invalid zip");
            }

            // Locate top-level 'extensions' directory
            var extensionsDir = Path.Combine(tempDir, "extensions");
            if (!Directory.Exists(extensionsDir))
                return l.ReturnFalse("zip missing top-level 'extensions' folder");

            var candidates = Directory.GetDirectories(extensionsDir, "*", SearchOption.TopDirectoryOnly);
            if (candidates.Length == 0)
                return l.ReturnFalse("'extensions' folder empty");

            // Determine source extension directory
            string? resolvedName = null;
            string? sourcePath = null;

            if (!string.IsNullOrWhiteSpace(name))
            {
                var preferred = Path.Combine(extensionsDir, name.Trim());
                if (Directory.Exists(preferred))
                {
                    resolvedName = name.Trim();
                    sourcePath = preferred;
                }
            }
            if (sourcePath == null && candidates.Length == 1)
            {
                sourcePath = candidates[0];
                resolvedName = Path.GetFileName(sourcePath);
            }
            if (sourcePath == null)
            {
                var derived = DeriveFolderNameFromZipFileName(originalZipFileName);
                if (!string.IsNullOrWhiteSpace(derived))
                {
                    var derivedPath = Path.Combine(extensionsDir, derived);
                    if (Directory.Exists(derivedPath))
                    {
                        sourcePath = derivedPath;
                        resolvedName = derived;
                    }
                }
            }
            if (sourcePath == null)
                return l.ReturnFalse("could not determine extension subfolder – specify 'name'");

            // Final folder name
            var folderName = !string.IsNullOrWhiteSpace(name) ? name!.Trim() : resolvedName!;
            if (!IsValidFolderName(folderName))
                return l.ReturnFalse($"invalid folder name:'{folderName}'");

            var targetRoot = Path.Combine(extensionsRoot, folderName);
            if (Directory.Exists(targetRoot))
            {
                if (!overwrite) return l.ReturnFalse("target exists - set overwrite");
                try { Zipping.TryToDeleteDirectory(targetRoot, l); } catch { /* ignore */ }
            }
            Directory.CreateDirectory(targetRoot);

            // Copy contents
            CopyDirectory(sourcePath, targetRoot, l);

            // Ensure App_Data/extension.json exists (create empty object if missing)
            var appData = Path.Combine(targetRoot, FolderConstants.DataFolderProtected);
            Directory.CreateDirectory(appData);
            var extJson = Path.Combine(appData, FolderConstants.AppExtensionJsonFile);
            if (!File.Exists(extJson))
            {
                try { File.WriteAllText(extJson, "{}", new UTF8Encoding(false)); } catch { /* ignore */ }
            }

            return l.ReturnTrue($"installed '{folderName}'");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
        finally
        {
            if (tempDir != null)
            {
                try { Zipping.TryToDeleteDirectory(tempDir, l); } catch { /* ignore */ }
            }
        }
    }

    private static void CopyDirectory(string source, string target, ILog l)
    {
        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            var rel = dir.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Path.Combine(target, rel));
        }
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var rel = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar);
            var dest = Path.Combine(target, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, overwrite: true);
        }
    }

    private static string? DeriveFolderNameFromZipFileName(string? originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
            return null;

        var baseName = Path.GetFileNameWithoutExtension(originalFileName!.Trim());
        if (string.IsNullOrWhiteSpace(baseName))
            return null;

        // Remove trailing duplicate indicator like " (1)" or " (23)"
        baseName = Regex.Replace(baseName, @"\s\(\d+\)$", string.Empty);

        // If pattern ends with _<version> (e.g. _00.00.01) strip the version part
        // Version pattern: one underscore + n.n.n (digits sections)
        var match = Regex.Match(baseName, @"^(?<name>.+)_(?<version>\d+\.\d+\.\d+)$");
        if (match.Success)
            return match.Groups["name"].Value;

        return baseName;
    }

    internal static bool IsValidFolderName(string name)
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
