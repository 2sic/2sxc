using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Sys;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Backend.App.ExtensionLockHelper;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Validation helper for incoming extension zip archives.
/// </summary>
internal class ExtensionValidationHelper(ExtensionManifestService manifestSvc, ILog? parentLog) : HelperBase(parentLog, "Bck.ExtVal")
{
    internal (string? error, Dictionary<string, LockValidationResult> lockResults, Dictionary<string, ManifestValidationResult> manifestResults) ValidateCandidateSubfolders(string tempDir,
        string[] candidateDirs)
    {
        var l = Log.Fn<(string? error, Dictionary<string, LockValidationResult> lockResults, Dictionary<string, ManifestValidationResult> manifestResults)>();

        var issues = new List<string>();
        var lockResults = new Dictionary<string, LockValidationResult>(StringComparer.OrdinalIgnoreCase);
        var manifestResults = new Dictionary<string, ManifestValidationResult>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in candidateDirs)
        {
            var folder = Path.GetFileName(dir);
            l.A($"validate:'{folder}'");

            var appDataDir = Path.Combine(dir, FolderConstants.DataFolderProtected);

            var folderIssues = new List<string>();

            var extensionJsonPath = Path.Combine(appDataDir, FolderConstants.AppExtensionJsonFile);
            if (!File.Exists(extensionJsonPath))
                folderIssues.Add($"missing {FolderConstants.AppExtensionJsonFile}");

            var lockJsonPath = Path.Combine(appDataDir, FolderConstants.AppExtensionLockJsonFile);
            if (!File.Exists(lockJsonPath))
                folderIssues.Add($"missing {FolderConstants.AppExtensionLockJsonFile}");

            // Validate extension.json contents if present
            LockValidationResult? lockValidation = null;
            if (File.Exists(extensionJsonPath))
            {
                var extVal = ValidateExtensionJsonFile(extensionJsonPath);
                if (!extVal.Success)
                    folderIssues.Add(extVal.Error ?? "extension.json invalid");
                else
                    manifestResults[folder] = extVal;
            }

            // Validate lock file contents restricted to this candidate
            if (File.Exists(lockJsonPath))
            {
                lockValidation = ValidateLockFile(lockJsonPath, tempDir, dir);
                if (!lockValidation.Success)
                    folderIssues.Add(lockValidation.Error ?? $"{FolderConstants.AppExtensionLockJsonFile} invalid");
            }

            if (folderIssues.Any())
                issues.Add($"{folder}: {string.Join(", ", folderIssues)}");
            else if (lockValidation != null)
                lockResults[folder] = lockValidation;
        }

        return l.ReturnAndLog((issues.Any() ? $"invalid extension subfolder(s): {string.Join("; ", issues)}" : null, lockResults, manifestResults));
    }

    internal ManifestValidationResult ValidateExtensionJsonFile(string extensionJsonFilePath)
    {
        var l = Log.Fn<ManifestValidationResult>();

        try
        {
            var manifest = manifestSvc.LoadManifest(new FileInfo(extensionJsonFilePath));
            if (manifest == null)
                return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionJsonFile} parse error", false, null));

            if (manifest.IsInstalled != true)
                return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionJsonFile} missing 'isInstalled' True", false, manifest));

            var editionsSupported = manifest.EditionsSupported;

            return l.ReturnAsOk(new(true, null, editionsSupported, manifest));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnAsError(new(false, $"{FolderConstants.AppExtensionJsonFile} parse error", false, null));
        }
    }

    // Validate lock file against a single candidate folder only
    internal LockValidationResult ValidateLockFile(string lockFilePath, string tempDir, string candidatePath)
    {
        var l = Log.Fn<LockValidationResult>();

        var lockRead = ReadLockFile(lockFilePath, l);
        if (!lockRead.Success || lockRead.ExpectedWithHash == null || lockRead.AllowedFiles == null)
            return l.ReturnAsError(new(false, lockRead.Error ?? $"{FolderConstants.AppExtensionLockJsonFile} invalid", null));

        var allowed = lockRead.AllowedFiles;
        var expectedWithHash = lockRead.ExpectedWithHash;
        var folderName = Path.GetFileName(candidatePath);
        var tmpAppCodeExtensionDirectory = Path.Combine(tempDir, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);

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

            var actualHash = CalculateHash(full);
            var expected = expectedWithHash[rel];
            if (!string.Equals(actualHash, expected, StringComparison.OrdinalIgnoreCase))
                return l.ReturnAsError(new(false, $"hash mismatch: {rel}", null));
        }

        return l.ReturnAsOk(new(true, null, allowed));
    }
}
