using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Sys;
using ToSic.Sxc.Services;
using ToSic.Eav.Security.Files;
using System.IO.Compression;

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
    /// FolderName resolution:
    /// - Use preferredFolderName if provided.
    /// - Otherwise derive from originalFileName (zip filename without extension).
    /// Extraction:
    /// - Always extract under the resolved folderName.
    /// - If the ZIP contains a single top-level folder, that segment is stripped.
    /// </summary>
    public bool InstallExtensionZip(int zoneId, int appId, Stream zipStream, string? preferredFolderName = null, bool overwrite = false, string? originalFileName = null)
    {
        var l = Log.Fn<bool>($"z:{zoneId}, a:{appId}, overwrite:{overwrite}, pref:'{preferredFolderName}', ofn:'{originalFileName}'");
        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var appPaths = appPathSvc.Get(appReader, site);

            var extensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);
            Directory.CreateDirectory(extensionsRoot);

            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
            if (archive.Entries.Count == 0)
                return l.ReturnFalse("empty zip");

            // Resolve folderName: prefer explicit, else use zip file name (base name, no extension)
            var folderName = !string.IsNullOrWhiteSpace(preferredFolderName)
                ? preferredFolderName.Trim()
                : DeriveFolderNameFromFile(originalFileName);

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
                    Directory.Delete(targetRoot, recursive: true);
                }
                catch (Exception exDel)
                {
                    Log.Ex(exDel);
                    return l.ReturnFalse("failed to delete existing target");
                }
            }
            Directory.CreateDirectory(targetRoot);

            // If zip has a single top-level folder, we will strip that segment on extraction
            var topLevel = GetSingleTopLevelFolder(archive);

            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.FullName)) continue;

                var normalized = NormalizeZipPath(entry.FullName);
                if (string.IsNullOrEmpty(normalized)) continue;

                var segments = normalized.Split('/');
                if (segments.Length == 0) continue;

                // Remove the top-level folder segment, if there is exactly one
                if (!string.IsNullOrWhiteSpace(topLevel) && segments[0].Equals(topLevel, StringComparison.Ordinal))
                    segments = segments.Skip(1).ToArray();

                if (segments.Length == 0) continue; // was just a folder entry at root

                // Safety: forbid traversal anywhere
                if (segments.Any(s => s == ".." || s.Contains("..")))
                    return l.ReturnFalse("zip contains illegal traversal path");

                var lastSegment = segments[segments.Length - 1];
                if (lastSegment.StartsWith(".", StringComparison.Ordinal))
                    continue;

                var relativePath = string.Join(Path.DirectorySeparatorChar.ToString(), segments);
                var destinationPath = Path.Combine(targetRoot, relativePath);
                var fullPath = Path.GetFullPath(destinationPath);

                // Must remain under targetRoot
                if (!fullPath.StartsWith(Path.GetFullPath(targetRoot), StringComparison.OrdinalIgnoreCase))
                    return l.ReturnFalse("zip path escapes target folder");

                // Directory entry?
                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(fullPath);
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                using var inStream = entry.Open();
                using var outStream = File.Create(fullPath);
                inStream.CopyTo(outStream);
            }

            return l.ReturnTrue($"installed '{folderName}'");
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            return l.ReturnFalse("error");
        }
    }

    private static string NormalizeZipPath(string path)
        => path.Replace('\\', '/').Trim().Trim('/');

    private static string? DeriveFolderNameFromFile(string? originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName)) return null;

        var baseName = Path.GetFileNameWithoutExtension(originalFileName.Trim());
        return string.IsNullOrWhiteSpace(baseName) ? null : baseName;
    }

    /// <summary>
    /// If all entries share exactly one first path-segment, return it; else empty string.
    /// </summary>
    private static string GetSingleTopLevelFolder(ZipArchive archive)
    {
        var firstSegments = new HashSet<string>(StringComparer.Ordinal);
        foreach (var e in archive.Entries)
        {
            if (string.IsNullOrEmpty(e.FullName)) continue;

            var norm = NormalizeZipPath(e.FullName);
            if (string.IsNullOrEmpty(norm)) continue;

            var seg = norm.Split('/')[0];
            if (string.IsNullOrEmpty(seg)) continue;

            firstSegments.Add(seg);
            if (firstSegments.Count > 1) return string.Empty;
        }
        return firstSegments.Count == 1
            ? firstSegments.First()
            : string.Empty;
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
