using System.Text.Json;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Sys.Configuration;
using ToSic.Sys.Security.Encryption;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsZipInstallerBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    IGlobalConfiguration globalConfiguration)
    : ServiceBase("Bck.ExtZip", connect: [appReadersLazy, site, appPathSvc, globalConfiguration])
{
    public bool InstallExtensionZip(int appId, Stream zipStream, bool overwrite = false, string? originalZipFileName = null)
    {
        var l = Log.Fn<bool>($"a:{appId}, overwrite:{overwrite}, ofn:'{originalZipFileName}'");

        string? tempDir = null;
        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var appPaths = appPathSvc.Get(appReader, site);
            var extensionsRoot = Path.Combine(appPaths.PhysicalPath, ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder);
            Directory.CreateDirectory(extensionsRoot);

            tempDir = Path.Combine(globalConfiguration.TemporaryFolder(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                new Zipping(Log).ExtractZipStream(zipStream, tempDir, allowCodeImport: true);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return l.ReturnFalse("invalid zip");
            }

            var extensionsDir = Path.Combine(tempDir, ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder);
            if (!Directory.Exists(extensionsDir))
                return l.ReturnFalse($"zip missing top-level '{ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder}' folder");

            var candidates = Directory.GetDirectories(extensionsDir, "*", SearchOption.TopDirectoryOnly);
            if (candidates.Length == 0)
                return l.ReturnFalse($"'{ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder}' folder empty");

            var (resolvedName, sourcePath) = ResolveName(originalZipFileName: originalZipFileName, extensionsDir: extensionsDir, candidates: candidates);
            if (sourcePath == null)
                return l.ReturnFalse("could not determine extension subfolder – specify 'name'");

            var folderName = resolvedName!;
            if (!ExtensionFolderNameValidator.IsValid(folderName))
                return l.ReturnFalse($"invalid folder name:'{folderName}'");

            var extensionJsonFilePath = Path.Combine(sourcePath, ToSic.Eav.Sys.FolderConstants.DataFolderProtected, ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile);
            if (!File.Exists(extensionJsonFilePath))
                return l.ReturnFalse($"missing {ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile}");

            var extensionJsonValidation = ValidateExtensionJsonFile(extensionJsonFilePath, sourcePath, l);
            if (!extensionJsonValidation.Success)
                return l.ReturnFalse(extensionJsonValidation.Error ?? $"{ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile} validation failed");

            var lockFilePath = Path.Combine(sourcePath, ToSic.Eav.Sys.FolderConstants.DataFolderProtected, ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile);
            if (!File.Exists(lockFilePath))
                return l.ReturnFalse($"missing {ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile}");

            var lockValidation = ValidateLockFile(lockFilePath, sourcePath, l);
            if (!lockValidation.Success)
                return l.ReturnFalse(lockValidation.Error ?? $"{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile} validation failed");

            var targetRoot = Path.Combine(extensionsRoot, folderName);
            if (Directory.Exists(targetRoot))
            {
                if (!overwrite) return l.ReturnFalse("target exists - set overwrite");
                try { Zipping.TryToDeleteDirectory(targetRoot, l); } catch { }
            }

            var appCodeExtensionDirectory = Path.Combine(appPaths.PhysicalPath, ToSic.Eav.Sys.FolderConstants.AppCodeFolder, ToSic.Eav.Sys.FolderConstants.AppExtensionsFolder, folderName);
            if (Directory.Exists(appCodeExtensionDirectory))
            {
                if (!overwrite) return l.ReturnFalse("target exists - set overwrite");
                try { Zipping.TryToDeleteDirectory(appCodeExtensionDirectory, l); } catch { }
            }

            CopyDirectory(tempDir, appPaths.PhysicalPath, l, lockValidation.AllowedFiles);

            var appData = Path.Combine(targetRoot, ToSic.Eav.Sys.FolderConstants.DataFolderProtected);
            Directory.CreateDirectory(appData);
            var extJson = Path.Combine(appData, ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile);
            if (!File.Exists(extJson))
            {
                try { File.WriteAllText(extJson, @"{}", new System.Text.UTF8Encoding(false)); } catch { }
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
                try { Zipping.TryToDeleteDirectory(tempDir, l); } catch { }
            }
        }
    }

    private static (string? resolvedName, string? sourcePath) ResolveName(string? originalZipFileName,
        string extensionsDir,
        string[] candidates)
    {
        string? resolvedName = null;
        string? sourcePath = null;

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

        return (resolvedName, sourcePath);
    }

    private static string? DeriveFolderNameFromZipFileName(string? originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
            return null;

        var baseName = Path.GetFileNameWithoutExtension(originalFileName!.Trim());
        if (string.IsNullOrWhiteSpace(baseName))
            return null;

        baseName = Regex.Replace(baseName, @"\s\(\d+\)$", string.Empty);

        var match = Regex.Match(baseName, @"^(?<name>.+)_(?<version>\d+\.\d+\.\d+)$");
        if (match.Success)
            return match.Groups["name"].Value;

        return baseName;
    }

    private record ValidationResult(bool Success, string? Error);

    private static ValidationResult ValidateExtensionJsonFile(string extensionJsonFilePath, string sourcePath, ILog? parentLog)
    {
        var l = parentLog.Fn<ValidationResult>();

        try
        {
            var json = File.ReadAllText(extensionJsonFilePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("isInstalled", out var isInstalledProp) || isInstalledProp.ValueKind != JsonValueKind.True)
                return l.ReturnAsError(new(false, $"{ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile} missing 'isInstalled' True"));

            return l.ReturnAsOk(new(true, null));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnAsError(new(false, $"{ToSic.Eav.Sys.FolderConstants.AppExtensionJsonFile} parse error"));
        }
    }

    private record LockValidationResult(bool Success, string? Error, HashSet<string>? AllowedFiles) : ValidationResult(Success, Error);

    private static LockValidationResult ValidateLockFile(string lockFilePath, string sourcePath, ILog? parentLog)
    {
        var l = parentLog.Fn<LockValidationResult>();

        try
        {
            var json = File.ReadAllText(lockFilePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("files", out var filesProp) || filesProp.ValueKind != JsonValueKind.Array)
                return l.ReturnAsError(new(false, $"{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile} missing 'files' array", null));

            var extractionPath = Directory.GetParent(Directory.GetParent(sourcePath)!.FullName)!.FullName;
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var expectedWithHash = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in filesProp.EnumerateArray())
            {
                string? file = null;
                string? hash = null;

                if (item.ValueKind == JsonValueKind.Object)
                {
                    if (!item.TryGetProperty("file", out var f) || f.ValueKind != JsonValueKind.String)
                        return l.ReturnAsError(new(false, $"{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile} entry missing 'file'", null));

                    file = f.GetString();

                    if (item.TryGetProperty("hash", out var h) && h.ValueKind == JsonValueKind.String)
                        hash = h.GetString();
                    else
                        return l.ReturnAsError(new(false, $"{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile} entry missing 'hash'", null));
                }
                else
                    return l.ReturnAsError(new(false, $"invalid {ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile} entry: {item.ValueKind}", null));

                if (string.IsNullOrWhiteSpace(file))
                    return l.ReturnAsError(new(false, $"empty 'file' in {ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile}", null));

                if (string.IsNullOrWhiteSpace(hash))
                    return l.ReturnAsError(new(false, $"empty 'hash' in {ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile}", null));

                file = file!.Replace('\\', '/').Trim().TrimStart('/');

                if (file.StartsWith("..") || file.Contains("/../"))
                    return l.ReturnAsError(new(false, $"illegal path:'{file}' in {ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile}", null));

                hash = hash!.Trim();

                allowed.Add(file);
                expectedWithHash[file] = hash;
                l.A($"added 'file':'{file}' to allowed with 'hash':'{hash}'");
            }

            var lockFileName = ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile;
            var actualFiles = Directory.GetFiles(extractionPath, "*", SearchOption.AllDirectories)
                .Select(f => f.Substring(extractionPath.Length).TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/'))
                .Where(f => !string.Equals(Path.GetFileName(f), lockFileName, StringComparison.OrdinalIgnoreCase))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missing = allowed.Where(a => !actualFiles.Contains(a)).ToList();
            if (missing.Any())
                return l.ReturnAsError(new(false, $"missing files:'{string.Join("','", missing)}'", null));

            var extras = actualFiles.Where(a => !allowed.Contains(a)).ToList();
            if (extras.Any())
                return l.ReturnAsError(new(false, $"unexpected files:'{string.Join("','", extras)}'", null));

            foreach (var rel in allowed)
            {
                if (!expectedWithHash.TryGetValue(rel, out var expected))
                    return l.ReturnAsError(new(false, $"hash missing for:{rel}", null));

                var full = Path.Combine(extractionPath, rel.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(full))
                    return l.ReturnAsError(new(false, $"file for hash missing:{rel}", null));

                var actualHash = Sha256.Hash(File.ReadAllText(full));
                if (!string.Equals(actualHash, expected, StringComparison.OrdinalIgnoreCase))
                    return l.ReturnAsError(new(false, $"hash mismatch: {rel}", null));
            }

            return l.ReturnAsOk(new(true, null, allowed));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return  l.ReturnAsError(new(false, $"{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile} parse error", null));
        }
    }

    private static void CopyDirectory(string source, string target, ILog? parentLog, HashSet<string>? allowedFiles)
    {
        var l = parentLog.Fn();

        var sourceLen = source.Length;
        var allFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        l.A($"files found: {allFiles.Length}");

        foreach (var file in allFiles)
        {
            var rel = file.Substring(sourceLen).TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/');

            if (string.Equals(rel, ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile, StringComparison.OrdinalIgnoreCase))
                continue; 

            if (allowedFiles != null && !allowedFiles.Contains(rel))
                continue; 

            var destFull = Path.Combine(target, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(destFull)!);
            File.Copy(file, destFull, overwrite: true);
            l.A($"file copy :'${rel}'");
        }

        var lockPath = Path.Combine(source, ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile);
        if (File.Exists(lockPath))
        {
            var destLock = Path.Combine(target, ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile);
            try
            {
                File.Copy(lockPath, destLock, overwrite: true);
                l.A($"file copy '{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile}'");
            }
            catch(Exception ex)
            {
                l.Ex(ex,$"can't copy '{ToSic.Eav.Sys.FolderConstants.AppExtensionLockJsonFile}'");
            }
        }

        l.Done();
    }
}
