using System.Security.Cryptography;
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
using ToSic.Sys.Security.Encryption;

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
            var extensionsDir = Path.Combine(tempDir, FolderConstants.AppExtensionsFolder);
            if (!Directory.Exists(extensionsDir))
                return l.ReturnFalse($"zip missing top-level '{FolderConstants.AppExtensionsFolder}' folder");

            var candidates = Directory.GetDirectories(extensionsDir, "*", SearchOption.TopDirectoryOnly);
            if (candidates.Length == 0)
                return l.ReturnFalse($"'{FolderConstants.AppExtensionsFolder}' folder empty");

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

            // Mandatory lock file validation
            var lockFilePath = Path.Combine(sourcePath, FolderConstants.DataFolderProtected, FolderConstants.AppExtensionLockJsonFile);
            if (!File.Exists(lockFilePath))
                return l.ReturnFalse($"missing {FolderConstants.AppExtensionLockJsonFile}");

            var lockValidation = ValidateLockFile(lockFilePath, sourcePath, l);
            if (!lockValidation.Success)
                return l.ReturnFalse(lockValidation.Error ?? "lock validation failed");

            var targetRoot = Path.Combine(extensionsRoot, folderName);
            if (Directory.Exists(targetRoot))
            {
                if (!overwrite) return l.ReturnFalse("target exists - set overwrite");
                try { Zipping.TryToDeleteDirectory(targetRoot, l); } catch { /* ignore */ }
            }
            Directory.CreateDirectory(targetRoot);

            // Copy contents (only allowed & verified files)
            CopyDirectory(sourcePath, targetRoot, l, lockValidation.AllowedFiles);

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

    private sealed record LockValidationResult(bool Success, string? Error, HashSet<string>? AllowedFiles);

    private static LockValidationResult ValidateLockFile(string lockFilePath, string sourcePath, ILog l)
    {
        try
        {
            var json = File.ReadAllText(lockFilePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("files", out var filesProp) || filesProp.ValueKind != JsonValueKind.Array)
                return new(false, "lock missing files array", null);

            var extFolderName = Path.GetFileName(sourcePath);
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var expectedWithHash = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in filesProp.EnumerateArray())
            {
                string? file = null;
                string? hash = null;

                if (item.ValueKind == JsonValueKind.Object)
                {
                    // Prefer "file" as per spec, fallback to legacy "path"
                    if (!item.TryGetProperty("file", out var f) || f.ValueKind != JsonValueKind.String)
                        return new(false, "lock entry missing file", null);

                    file = f.GetString();
                    if (item.TryGetProperty("hash", out var h) && h.ValueKind == JsonValueKind.String)
                        hash = h.GetString();
                    else
                        return new(false, "lock entry missing hash", null);
                }
                //else if (item.ValueKind == JsonValueKind.String)
                //{
                //    // Legacy: just a string path (no hash)
                //    file = item.GetString();
                //}
                else
                {
                    return new(false, "invalid lock entry", null);
                }

                if (string.IsNullOrWhiteSpace(file))
                    return new(false, "empty file in lock", null);

                // Normalize: slashes, trim, remove leading '/', remove optional 'extensions/' + extFolderName prefix
                file = file!.Replace('\\', '/').Trim();
                file = file.TrimStart('/');
                if (file.StartsWith("extensions/", StringComparison.OrdinalIgnoreCase))
                {
                    var afterExt = file.Substring("extensions/".Length);
                    if (afterExt.StartsWith(extFolderName + "/", StringComparison.OrdinalIgnoreCase))
                        file = afterExt.Substring(extFolderName.Length + 1);
                    else
                        file = afterExt; // tolerate missing folder name in path
                }
                // basic traversal protection
                if (file.StartsWith("..") || file.Contains("/../"))
                    return new(false, "illegal path in lock", null);

                // Ensure we keep App_Data prefix casing
                allowed.Add(file);
                if (!string.IsNullOrWhiteSpace(hash))
                {
                    var hex = hash!.Trim();
                    expectedWithHash[file] = hex;
                }
            }

            // Gather actual files under source (relative)
            var lockFileName = FolderConstants.AppExtensionLockJsonFile;
            var actualFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => f.Substring(sourcePath.Length).TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/'))
                .Where(f => !string.Equals(Path.GetFileName(f), lockFileName, StringComparison.OrdinalIgnoreCase))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Check for missing files
            var missing = allowed.Where(a => !actualFiles.Contains(a)).ToList();
            if (missing.Any()) return new(false, $"missing files: {string.Join(",", missing)}", null);

            // Check for extra files
            var extras = actualFiles.Where(a => !allowed.Contains(a)).ToList();
            if (extras.Any()) return new(false, $"unexpected files: {string.Join(",", extras)}", null);

            // Hash verification (all entries required to have a hash)
            foreach (var rel in allowed)
            {
                if (!expectedWithHash.TryGetValue(rel, out var expected))
                    return new(false, $"hash missing for: {rel}", null);
                var full = Path.Combine(sourcePath, rel.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(full)) return new(false, $"file for hash missing: {rel}", null);
                var actualHash = Sha256.Hash(File.ReadAllText(full));
                if (!string.Equals(actualHash, expected, StringComparison.OrdinalIgnoreCase))
                    return new(false, $"hash mismatch: {rel}", null);
            }

            return new(true, null, allowed);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return new(false, "lock parse error", null);
        }
    }

    private static void CopyDirectory(string source, string target, ILog l, HashSet<string>? allowedFiles)
    {
        var sourceLen = source.Length;
        var allFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        foreach (var file in allFiles)
        {
            var rel = file.Substring(sourceLen).TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/');
            if (string.Equals(rel, FolderConstants.AppExtensionLockJsonFile, StringComparison.OrdinalIgnoreCase)) continue; // skip lock itself (will copy at end)
            if (allowedFiles != null && !allowedFiles.Contains(rel)) continue; // shouldn't happen after validation
            var destFull = Path.Combine(target, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(destFull)!);
            File.Copy(file, destFull, overwrite: true);
        }

        // Copy lock file
        var lockPath = Path.Combine(source, FolderConstants.AppExtensionLockJsonFile);
        if (File.Exists(lockPath))
        {
            var destLock = Path.Combine(target, FolderConstants.AppExtensionLockJsonFile);
            try { File.Copy(lockPath, destLock, overwrite: true); } catch { /* ignore */ }
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
