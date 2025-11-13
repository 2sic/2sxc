using System;
using System.IO;
using System.Text.Json;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
using ToSic.Sys.Configuration;
using ToSic.Sys.Security.Encryption;
using ToSic.Sys.Utils;

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

            foreach (var lockResult in lockResults)
            {
                var folderName = lockResult.Key;
                var lockValidation = lockResult.Value;

                l.A($"prepare install:'{folderName}'");

                var installResult = InstallSingleExtension(
                    folderName: folderName,
                    lockValidation: lockValidation,
                    tempDir: tempDir,
                    extensionsRoot: extensionsRoot,
                    appRoot: appPaths.PhysicalPath,
                    overwrite: overwrite,
                    parentLog: l);

                if (!installResult.Success)
                    return l.ReturnFalse(installResult.Error ?? $"install failed:'{folderName}'");

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

    // Install a single extension folder using the lock metadata to guard file copying.
    private ValidationResult InstallSingleExtension(string folderName, LockValidationResult lockValidation, string tempDir, string extensionsRoot, string appRoot, bool overwrite, ILog? parentLog)
    {
        var l = parentLog.Fn<ValidationResult>($"folder:'{folderName}'");

        if (!ExtensionFolderNameValidator.IsValid(folderName))
            return l.ReturnAsError(new(false, $"invalid folder name:'{folderName}'"));

        if (!lockValidation.Success)
            return l.ReturnAsError(new(false, lockValidation.Error ?? $"lock validation failed:'{folderName}'"));

        var allowedFiles = lockValidation.AllowedFiles;
        if (allowedFiles == null || allowedFiles.Count == 0)
            return l.ReturnAsError(new(false, $"no files allowed for '{folderName}'"));

        var tempExtensionFolder = Path.Combine(tempDir, FolderConstants.AppExtensionsFolder, folderName);
        var tempAppCodeFolder = Path.Combine(tempDir, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);

        var extensionTarget = Path.Combine(extensionsRoot, folderName);
        var appCodeTarget = Path.Combine(appRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);

        var extensionTargetValidation = EnsureTargetReadyForCopy(tempExtensionFolder, extensionTarget, overwrite, l, FolderConstants.AppExtensionsFolder);
        if (!extensionTargetValidation.Success)
            return l.ReturnAsError(extensionTargetValidation);

        var appCodeTargetValidation = EnsureTargetReadyForCopy(tempAppCodeFolder, appCodeTarget, overwrite, l, FolderConstants.AppCodeFolder);
        if (!appCodeTargetValidation.Success)
            return l.ReturnAsError(appCodeTargetValidation);

        var copyResult = CopyAllowedFiles(tempDir, appRoot, folderName, allowedFiles, l);
        if (!copyResult.Success)
            return l.ReturnAsError(copyResult);

        return l.ReturnAsOk(new(true, null));
    }

    // Ensure the destination directory is ready to receive new files, deleting previous content when required.
    private static ValidationResult EnsureTargetReadyForCopy(string tempSourcePath, string targetPath, bool overwrite, ILog? parentLog, string areaName)
    {
        var l = parentLog.Fn<ValidationResult>($"area:{areaName}");

        var sourceExists = Directory.Exists(tempSourcePath);
        var targetExists = Directory.Exists(targetPath);

        if (!sourceExists && !targetExists)
            return l.ReturnAsOk(new(true, null));

        if (targetExists)
        {
            if (!overwrite)
                return l.ReturnAsError(new(false, $"'{targetPath}' target exists - set overwrite"));

            // RemoveReadOnlyRecursive(targetPath, parentLog);
            l.A($"cleanup target:'{targetPath}'");
            Zipping.TryToDeleteDirectory(targetPath, parentLog);
        }

        return l.ReturnAsOk(new(true, null));
    }

    private static ValidationResult CopyAllowedFiles(string sourceRoot, string targetRoot, string folderName, HashSet<string> allowedFiles, ILog? parentLog)
    {
        var l = parentLog.Fn<ValidationResult>($"copy:'{folderName}'");

        var sourceRootFull = SuffixBackslash(Path.GetFullPath(sourceRoot));
        var targetRootFull = SuffixBackslash(Path.GetFullPath(targetRoot));

        var sources = new[]
        {
            Path.Combine(sourceRoot, FolderConstants.AppExtensionsFolder, folderName),
            Path.Combine(sourceRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName),
        };

        foreach (var source in sources)
        {
            foreach (var file in EnumerateFilesSafe(source))
            {
                var rel = file
                    .Substring(sourceRootFull.Length)
                    .TrimPrefixSlash()
                    .ForwardSlash();

                var isLockFile = rel.EndsWith(FolderConstants.AppExtensionLockJsonFile, StringComparison.OrdinalIgnoreCase);
                if (!isLockFile && !allowedFiles.Contains(rel))
                    continue;

                var destinationPath = Path.GetFullPath(Path.Combine(targetRoot, rel.Backslash()));
                if (!destinationPath.StartsWith(targetRootFull, StringComparison.OrdinalIgnoreCase))
                    return l.ReturnAsError(new(false, $"illegal destination path:'{rel}'"));

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                RemoveReadOnlyIfNeeded(destinationPath, rel, l);
                File.Copy(file, destinationPath, overwrite: true);
                EnsureReadOnly(destinationPath, rel, l);
                l.A($"copied:'{rel}'");
            }
        }

        return l.ReturnAsOk(new(true, null));
    }

    private static string SuffixBackslash(string path)
        => path.SuffixSlash().Backslash();


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

                var file = f.GetString()!.Trim().TrimPrefixSlash().ForwardSlash();
                var hash = h.GetString()!.Trim();

                if (file.ContainsPathTraversal())
                    return l.ReturnAsError(new(false, $"illegal path:'{file}' in {FolderConstants.AppExtensionLockJsonFile}", null));

                allowed.Add(file);
                expectedWithHash[file] = hash;
                l.A($"added candidate file '{file}' with hash");
            }
            var actualFiles = EnumerateFilesSafe(candidatePath)
                .Union(EnumerateFilesSafe(tmpAppCodeExtensionDirectory))
                .Select(f => f
                    .Substring(tempDir.Length)
                    .TrimPrefixSlash()
                    .ForwardSlash())
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
                var full = Path.Combine(tempDir, rel.Backslash());
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

    private static IEnumerable<string> EnumerateFilesSafe(string? path)
        => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)
            ? Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            : Array.Empty<string>();

    private static void RemoveReadOnlyRecursive(string directory, ILog? log)
    {
        if (!Directory.Exists(directory))
            return;

        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            var rel = file.Substring(directory.Length)
                .TrimPrefixSlash()
                .ForwardSlash();
            RemoveReadOnlyIfNeeded(file, rel, log);
        }

        foreach (var dir in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories))
        {
            var rel = dir.Substring(directory.Length)
                .TrimPrefixSlash()
                .ForwardSlash();
            ClearDirectoryReadOnly(dir, rel, log);
        }

        ClearDirectoryReadOnly(directory, string.Empty, log);
    }

    private static void RemoveReadOnlyIfNeeded(string path, string relPath, ILog? log)
    {
        if (!File.Exists(path))
            return;

        var attributes = File.GetAttributes(path);
        if (!attributes.HasFlag(FileAttributes.ReadOnly))
            return;

        File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
        log?.A($"cleared readonly:'{relPath}'");
    }

    private static void EnsureReadOnly(string path, string relPath, ILog? log)
    {
        if (!File.Exists(path))
            return;

        var attributes = File.GetAttributes(path);
        if (attributes.HasFlag(FileAttributes.ReadOnly))
            return;

        File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
        log?.A($"set readonly:'{relPath}'");
    }

    private static void ClearDirectoryReadOnly(string directory, string relPath, ILog? log)
    {
        var info = new DirectoryInfo(directory);
        var attributes = info.Attributes;
        if (!attributes.HasFlag(FileAttributes.ReadOnly))
            return;

        info.Attributes = attributes & ~FileAttributes.ReadOnly;
        if (!string.IsNullOrEmpty(relPath))
            log?.A($"cleared readonly dir:'{relPath}'");
    }

}
