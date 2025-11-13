using System.Text.Json;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
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
            var extensionsRoot = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppExtensionsFolder);
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

            var extensionsDir = Path.Combine(tempDir, FolderConstants.AppExtensionsFolder);
            if (!Directory.Exists(extensionsDir))
                return l.ReturnFalse($"zip missing top-level '{FolderConstants.AppExtensionsFolder}' folder");

            var candidateDirs = Directory.GetDirectories(extensionsDir, "*", SearchOption.TopDirectoryOnly);
            if (candidateDirs.Length == 0)
                return l.ReturnFalse($"'{FolderConstants.AppExtensionsFolder}' folder empty");

            // Validate every immediate subfolder: must contain required files and valid lock/json entries
            var (error, lockResults) = ValidateCandidateSubfolders(tempDir, candidateDirs, l);
            if (error != null)
                return l.ReturnFalse(error);

            var installed = new List<string>();

            foreach (var folderName in lockResults.Keys)
            {
                l.A($"install:'{folderName}'");

                if (!ExtensionFolderNameValidator.IsValid(folderName))
                    return l.ReturnFalse($"invalid folder name:'{folderName}'");

                // Reuse per-candidate lock validation result
                if (!lockResults.TryGetValue(folderName, out var lockValidation) || !lockValidation.Success)
                    return l.ReturnFalse(lockValidation?.Error ?? "lock validation missing");

                var targetExtensionRoot = Path.Combine(extensionsRoot, folderName);
                if (Directory.Exists(targetExtensionRoot))
                {
                    if (!overwrite)
                        return l.ReturnFalse($"'{targetExtensionRoot}' target exists - set overwrite");

                    Zipping.TryToDeleteDirectory(targetExtensionRoot, l);
                }

                var tempExtensionRoot = Path.Combine(tempDir, FolderConstants.AppExtensionsFolder, folderName);
                if (Directory.Exists(tempExtensionRoot))
                {
                    if (!overwrite)
                        return l.ReturnFalse($"'{tempExtensionRoot}' target exists - set overwrite");

                    CopyDirectory(tempExtensionRoot, targetExtensionRoot, l, lockValidation.AllowedFiles, tempDir, appPaths.PhysicalPath);
                }

                var appCodeExtensionDirectory = Path.Combine(appPaths.PhysicalPath, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);
                if (Directory.Exists(appCodeExtensionDirectory))
                {
                    if (!overwrite)
                        return l.ReturnFalse($"'{appCodeExtensionDirectory}' target exists - set overwrite");

                    Zipping.TryToDeleteDirectory(appCodeExtensionDirectory, l);
                }

                var tmpAppCodeExtensionDirectory = Path.Combine(tempDir, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);
                if (Directory.Exists(tmpAppCodeExtensionDirectory))
                {
                    if (!overwrite)
                        return l.ReturnFalse($"'{tmpAppCodeExtensionDirectory}' target exists - set overwrite");

                    CopyDirectory(tempDir, appCodeExtensionDirectory, l, lockValidation.AllowedFiles, tempDir, appPaths.PhysicalPath);
                }

                installed.Add(folderName);
            }
            return l.ReturnTrue($"installed '{string.Join("','", installed)}' from '{originalZipFileName}'");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("error");
        }
        finally
        {
            if (tempDir != null)
                Zipping.TryToDeleteDirectory(tempDir, l);
        }
    }

    private static (string? error, Dictionary<string, LockValidationResult> lockResults) ValidateCandidateSubfolders(string tempDir, string[] candidateDirs, ILog? parentLog)
    {
        var l = parentLog.Fn<(string? error, Dictionary<string, LockValidationResult> lockResults)>();
        
        var issues = new List<string>();
        var lockResults = new Dictionary<string, LockValidationResult>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in candidateDirs)
        {
            var folder = Path.GetFileName(dir);
            l.A($"validate:'{folder}'");

            var appDataDir = Path.Combine(dir, FolderConstants.DataFolderProtected);
            var extensionJsonPath = Path.Combine(appDataDir, FolderConstants.AppExtensionJsonFile);
            var lockJsonPath = Path.Combine(appDataDir, FolderConstants.AppExtensionLockJsonFile);
            var folderIssues = new List<string>();

            if (!File.Exists(extensionJsonPath))
                folderIssues.Add($"missing {FolderConstants.AppExtensionJsonFile}");

            if (!File.Exists(lockJsonPath))
                folderIssues.Add($"missing {FolderConstants.AppExtensionLockJsonFile}");

            // Validate extension.json contents if present
            LockValidationResult? lockValidation = null;
            if (File.Exists(extensionJsonPath))
            {
                var extVal = ValidateExtensionJsonFile(extensionJsonPath, dir, l);
                if (!extVal.Success)
                    folderIssues.Add(extVal.Error ?? "extension.json invalid");
            }

            // Validate lock file contents restricted to this candidate
            if (File.Exists(lockJsonPath))
            {
                // Use specialized candidate validation to avoid cross-extension interference
                lockValidation = ValidateLockFile(lockJsonPath, tempDir, dir, l);
                if (!lockValidation.Success)
                    folderIssues.Add(lockValidation.Error ?? "extension.lock.json invalid");
            }

            if (folderIssues.Any())
                issues.Add($"{folder}: {string.Join(", ", folderIssues)}");
            else if (lockValidation != null)
                lockResults[folder] = lockValidation;
        }

        return l.ReturnAndLog((issues.Any() ? $"invalid extension subfolder(s): {string.Join("; ", issues)}" : null, lockResults));
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
                return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionJsonFile} missing 'isInstalled' True"));

            return l.ReturnAsOk(new(true, null));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionJsonFile} parse error"));
        }
    }

    private record LockValidationResult(bool Success, string? Error, HashSet<string>? AllowedFiles) : ValidationResult(Success, Error);

    // Validate lock file against a single candidate folder only
    private static LockValidationResult ValidateLockFile(string lockFilePath, string tempDir, string candidatePath, ILog? parentLog)
    {
        var l = parentLog.Fn<LockValidationResult>();
        try
        {
            var json = File.ReadAllText(lockFilePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("files", out var filesProp) || filesProp.ValueKind != JsonValueKind.Array)
                return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionLockJsonFile} missing 'files' array", null));

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var expectedWithHash = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var folderName = Path.GetFileName(candidatePath);
            var tmpAppCodeExtensionDirectory = Path.Combine(tempDir, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);

            foreach (var item in filesProp.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    return l.ReturnAsError(new(false, $"invalid {FolderConstants.AppExtensionLockJsonFile} entry: {item.ValueKind}", null));

                if (!item.TryGetProperty("file", out var f) || f.ValueKind != JsonValueKind.String)
                    return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionLockJsonFile} entry missing 'file'", null));

                if (!item.TryGetProperty("hash", out var h) || h.ValueKind != JsonValueKind.String)
                    return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionLockJsonFile} entry missing 'hash'", null));

                var file = f.GetString()!.Replace('\\','/').Trim().TrimStart('/');
                var hash = h.GetString()!.Trim();

                if (file.StartsWith("..") || file.Contains("/../"))
                    return l.ReturnAsError(new(false, $"illegal path:'{file}' in {FolderConstants.AppExtensionLockJsonFile}", null));

                allowed.Add(file);
                expectedWithHash[file] = hash;
                l.A($"added candidate file '{file}' with hash");
            }
            var actualFiles = Directory.GetFiles(candidatePath, "*", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(tmpAppCodeExtensionDirectory, "*", SearchOption.AllDirectories))
                .Select(f => f
                    .Substring(tempDir.Length)
                    .TrimStart(Path.DirectorySeparatorChar)
                    .Replace(Path.DirectorySeparatorChar,'/'))
                .Where(f => !string.Equals(Path.GetFileName(f), FolderConstants.AppExtensionLockJsonFile, StringComparison.OrdinalIgnoreCase))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missing = allowed
                .Where(a => !actualFiles.Contains(a))
                .ToList();
            if (missing.Any())
                return l.ReturnAsError(new(false, $"missing files:'{string.Join("','", missing)}'", null));

            var extras = actualFiles
                .Where(a => !allowed.Contains(a))
                .ToList();
            if (extras.Any())
                return l.ReturnAsError(new(false, $"unexpected files:'{string.Join("','", extras)}'", null));

            foreach (var rel in allowed)
            {
                var full = Path.Combine(tempDir, rel.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(full))
                    return l.ReturnAsError(new(false, $"file for hash missing:{rel}", null));

                var actualHash = Sha256.Hash(File.ReadAllText(full));
                var expected = expectedWithHash[rel];
                if (!string.Equals(actualHash, expected, StringComparison.OrdinalIgnoreCase))
                    return l.ReturnAsError(new(false, $"hash mismatch: {rel}", null));
            }

            return l.ReturnAsOk(new(true, null, allowed));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionLockJsonFile} parse error", null));
        }
    }

    private static void CopyDirectory(string source, string target, ILog? parentLog, HashSet<string>? allowedFiles, string sourceRoot, string targetRoot)
    {
        var l = parentLog.Fn();

        var sourceLen = sourceRoot.Length;
        var allFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        l.A($"files found: {allFiles.Length}");

        foreach (var file in allFiles)
        {
            var rel = file
                .Substring(sourceLen)
                .TrimStart(Path.DirectorySeparatorChar)
                .Replace(Path.DirectorySeparatorChar, '/');

            if (!rel.EndsWith(FolderConstants.AppExtensionLockJsonFile, StringComparison.OrdinalIgnoreCase))
                if (allowedFiles != null && !allowedFiles.Contains(rel))
                    continue;

            var destFull = Path.Combine(targetRoot, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(destFull)!);
            File.Copy(file, destFull, overwrite: true);
            l.A($"file copy :'${rel}'");
        }
        
        l.Done();
    }
}
