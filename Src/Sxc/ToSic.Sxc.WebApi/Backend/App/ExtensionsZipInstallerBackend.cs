using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
using ToSic.Sys.Configuration;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Backend.App.ExtensionLockHelper;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsZipInstallerBackend(
    LazySvc<IAppReaderFactory> appReadersLazy,
    ISite site,
    IAppPathsMicroSvc appPathSvc,
    IGlobalConfiguration globalConfiguration,
    ExtensionManifestService manifestService,
    LazySvc<ExtensionsInspectorBackend> inspectorLazy)
    : ServiceBase("Bck.ExtZip", connect: [appReadersLazy, site, appPathSvc, globalConfiguration, manifestService, inspectorLazy])
{
    public bool InstallExtensionZip(int appId, Stream zipStream, bool overwrite = false, string? originalZipFileName = null, string[]? editions = null)
    {
        var l = Log.Fn<bool>($"a:{appId}, overwrite:{overwrite}, ofn:'{originalZipFileName}'");

        string? tempDir = null;
        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var appPaths = appPathSvc.Get(appReader, site);
            var editionList = NormalizeEditions(editions);
            var appRoot = appPaths.PhysicalPath;
            var extensionsRoot = Path.Combine(appRoot, FolderConstants.AppExtensionsFolder);
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
            var (error, lockResults, manifestResults) = ValidateCandidateSubfolders(tempDir, candidateDirs, manifestService);
            if (error != null)
                return l.ReturnFalse(error);

            var installed = new List<string>();

            foreach (var lockResult in lockResults)
            {
                var folderName = lockResult.Key;
                var lockValidation = lockResult.Value;

                l.A($"prepare install:'{folderName}'");

                if (!manifestResults.TryGetValue(folderName, out var manifestValidation))
                    return l.ReturnFalse($"missing manifest info for '{folderName}'");

                var editionsSupported = manifestValidation.EditionsSupported;
                if (editionList.Any(e => e.HasValue()) && !editionsSupported)
                    return l.ReturnFalse("extension does not support editions");

                foreach (var edition in editionList)
                {
                    var editionRoot = edition.HasValue()
                        ? Path.Combine(appRoot, edition)
                        : appRoot;

                    var editionExtensionsRoot = Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder);
                    Directory.CreateDirectory(editionExtensionsRoot);

                    var installResult = InstallSingleExtension(
                        folderName: folderName,
                        lockValidation: lockValidation,
                        tempDir: tempDir,
                        extensionsRoot: editionExtensionsRoot,
                        appRoot: editionRoot,
                        overwrite: overwrite);

                    if (!installResult.Success)
                        return l.ReturnFalse(installResult.Error ?? $"install failed:'{folderName}'");
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

    public ExtensionInstallPreflightResultDto InstallExtensionPreflight(int appId, Stream zipStream, string? originalZipFileName = null, string[]? editions = null)
    {
        var l = Log.Fn<ExtensionInstallPreflightResultDto>($"a:{appId}, ofn:'{originalZipFileName}'");

        string? tempDir = null;
        try
        {
            var appReader = appReadersLazy.Value.Get(appId);
            var appPaths = appPathSvc.Get(appReader, site);
            var appRoot = appPaths.PhysicalPath;
            var requestedEditions = NormalizeEditions(editions);

            tempDir = Path.Combine(globalConfiguration.TemporaryFolder(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            new Zipping(Log).ExtractZipStream(zipStream, tempDir, allowCodeImport: true);

            var extensionsDir = Path.Combine(tempDir, FolderConstants.AppExtensionsFolder);
            if (!Directory.Exists(extensionsDir))
                throw new InvalidOperationException($"zip missing top-level '{FolderConstants.AppExtensionsFolder}' folder");

            var candidateDirs = Directory.GetDirectories(extensionsDir, "*", SearchOption.TopDirectoryOnly);
            if (candidateDirs.Length == 0)
                throw new InvalidOperationException($"'{FolderConstants.AppExtensionsFolder}' folder empty");

            var (error, lockResults, manifestResults) = ValidateCandidateSubfolders(tempDir, candidateDirs, manifestService);
            if (error != null)
                throw new InvalidOperationException(error);

            var result = new ExtensionInstallPreflightResultDto();

            foreach (var kvp in lockResults)
            {
                var folderName = kvp.Key;
                var lockValidation = kvp.Value;

                if (!manifestResults.TryGetValue(folderName, out var manifestValidation) || manifestValidation.Manifest == null)
                    throw new InvalidOperationException($"missing manifest info for '{folderName}'");

                var manifest = manifestValidation.Manifest;
                var editionsSupported = manifestValidation.EditionsSupported;
                if (requestedEditions.Any(e => e.HasValue()) && !editionsSupported)
                    throw new InvalidOperationException("extension does not support editions");

                var editionTargets = MergeEditions(requestedEditions, DetectInstalledEditions(appRoot, folderName));
                var extDto = new ExtensionInstallPreflightExtensionDto
                {
                    Name = folderName,
                    Version = manifest.Version,
                    EditionsSupported = editionsSupported,
                    FileCount = lockValidation.AllowedFiles?.Count ?? 0,
                    Features = MapFeatures(manifest)
                };

                foreach (var edition in editionTargets)
                {
                    var editionInfo = BuildEditionInfo(appId, appRoot, folderName, edition, manifest);
                    if (editionInfo is not null)
                        extDto.Editions.Add(editionInfo);
                }

                result.Extensions.Add(extDto);
            }

            return l.Return(result, $"extensions:{result.Extensions.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            throw;
        }
        finally
        {
            if (tempDir != null)
                Zipping.TryToDeleteDirectory(tempDir, l);
        }
    }

    private (string? error, Dictionary<string, LockValidationResult> lockResults, Dictionary<string, ManifestValidationResult> manifestResults) ValidateCandidateSubfolders(string tempDir,
            string[] candidateDirs, ExtensionManifestService manifestSvc)
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
                var extVal = ValidateExtensionJsonFile(extensionJsonPath, manifestSvc);
                if (!extVal.Success)
                    folderIssues.Add(extVal.Error ?? "extension.json invalid");
                else
                    manifestResults[folder] = extVal;
            }
            // Validate lock file contents restricted to this candidate
            if (File.Exists(lockJsonPath))
            {
                // Use specialized candidate validation to avoid cross-extension interference
                lockValidation = ValidateLockFile(lockJsonPath, tempDir, dir);
                if (!lockValidation.Success)
                    folderIssues.Add(lockValidation.Error ?? "extension.lock.json invalid");
            }

            if (folderIssues.Any())
                issues.Add($"{folder}: {string.Join(", ", folderIssues)}");
            else if (lockValidation != null)
                lockResults[folder] = lockValidation;
        }

        return l.ReturnAndLog((issues.Any() ? $"invalid extension subfolder(s): {string.Join("; ", issues)}" : null, lockResults, manifestResults));
    }

    // Install a single extension folder using the lock metadata to guard file copying.
    private ValidationResult InstallSingleExtension(string folderName, LockValidationResult lockValidation, string tempDir, string extensionsRoot, string appRoot, bool overwrite)
    {
        var l = Log.Fn<ValidationResult>($"folder:'{folderName}'");

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

        var extensionTargetValidation = EnsureTargetReadyForCopy(tempExtensionFolder, extensionTarget, overwrite, FolderConstants.AppExtensionsFolder);
        if (!extensionTargetValidation.Success)
            return l.ReturnAsError(extensionTargetValidation);

        var appCodeTargetValidation = EnsureTargetReadyForCopy(tempAppCodeFolder, appCodeTarget, overwrite, FolderConstants.AppCodeFolder);
        if (!appCodeTargetValidation.Success)
            return l.ReturnAsError(appCodeTargetValidation);

        var copyResult = CopyAllowedFiles(tempDir, appRoot, folderName, allowedFiles);
        if (!copyResult.Success)
            return l.ReturnAsError(copyResult);

        return l.ReturnAsOk(new(true, null));
    }

    // Ensure the destination directory is ready to receive new files, deleting previous content when required.
    private ValidationResult EnsureTargetReadyForCopy(string tempSourcePath, string targetPath, bool overwrite, string areaName)
    {
        var l = Log.Fn<ValidationResult>($"area:{areaName}");

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
            Zipping.TryToDeleteDirectory(targetPath, l);
        }

        return l.ReturnAsOk(new(true, null));
    }

    private ValidationResult CopyAllowedFiles(string sourceRoot, string targetRoot, string folderName, HashSet<string> allowedFiles)
    {
        var l = Log.Fn<ValidationResult>($"copy:'{folderName}'");

        var sourceRootFull = EnsureTrailingBackslash(Path.GetFullPath(sourceRoot));
        var targetRootFull = EnsureTrailingBackslash(Path.GetFullPath(targetRoot));

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
                RemoveReadOnlyIfNeeded(destinationPath, rel);
                File.Copy(file, destinationPath, overwrite: true);
                EnsureReadOnly(destinationPath, rel);
                l.A($"copied:'{rel}'");
            }
        }

        return l.ReturnAsOk(new(true, null));
    }

    private ExtensionInstallPreflightEditionDto? BuildEditionInfo(int appId, string appRoot, string extensionName, string edition, ExtensionManifest incomingManifest)
    {
        var l = Log.Fn<ExtensionInstallPreflightEditionDto>();

        var editionRoot = edition.HasValue()
            ? Path.Combine(appRoot, edition)
            : appRoot;

        l.A($"prep edition:'{edition}', root:'{editionRoot}', ext:'{extensionName}'");

        if (!Directory.Exists(Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder, extensionName)))
            return l.ReturnNull($"extension not found:'{extensionName}'");

        var installedManifest = LoadInstalledManifest(editionRoot, extensionName);
        if (installedManifest is null)
            return l.Return(new ExtensionInstallPreflightEditionDto
                {
                    Edition = edition
                }, $"extension without manifest:'{extensionName}'");

        var currentVersion = installedManifest.Version;
        var isInstalled = installedManifest.IsInstalled == true;

        var inspectEdition = inspectorLazy.Value.Inspect(appId, extensionName, edition.HasValue() ? edition : null);
        var hasFileChanges = inspectEdition.FoundLock && inspectEdition.Summary != null
            && (inspectEdition.Summary.Changed > 0 || inspectEdition.Summary.Added > 0 || inspectEdition.Summary.Missing > 0);
        var hasData = inspectEdition.Data?.ContentTypes.Any(ct => ct.LocalEntities > 0) == true;
        var breakingChanges = HasBreakingChanges(incomingManifest, currentVersion);

        l.A($"state installed:{isInstalled}, ver:'{currentVersion}', changes:{hasFileChanges}, data:{hasData}, breaking:{breakingChanges}");

        return l.ReturnAsOk(new ExtensionInstallPreflightEditionDto
        {
            Edition = edition,
            IsInstalled = isInstalled,
            CurrentVersion = currentVersion,
            HasFileChanges = hasFileChanges,
            HasData = hasData,
            BreakingChanges = breakingChanges
        });
    }

    private static ExtensionInstallPreflightFeaturesDto MapFeatures(ExtensionManifest manifest) => new()
    {
        FieldsInside = manifest.HasFields,
        RazorInside = manifest.HasRazor,
        AppCodeInside = manifest.HasAppCode,
        WebApiInside = manifest.HasWebApi,
        ContentTypesInside = manifest.HasContentTypes,
        DataBundlesInside = manifest.HasDataBundles,
        QueriesInside = manifest.HasQueries,
        ViewsInside = manifest.HasViews,
        DataInside = manifest.DataInside,
        InputTypeInside = manifest.InputTypeInside.HasValue()
    };

    private static bool HasBreakingChanges(ExtensionManifest incomingManifest, string? currentVersion)
    {
        if (incomingManifest.Releases.ValueKind != JsonValueKind.Array)
            return false;

        var current = new Version();
        var hasCurrent = currentVersion.HasValue() && Version.TryParse(currentVersion, out current);

        foreach (var release in incomingManifest.Releases.EnumerateArray())
        {
            if (release.ValueKind != JsonValueKind.Object)
                continue;

            var breaking = release.TryGetProperty("breaking", out var breakingProp)
                           && breakingProp.ValueKind == JsonValueKind.True;
            if (!breaking)
                continue;

            if (!hasCurrent)
                return false;

            if (release.TryGetProperty("version", out var versionProp)
                && versionProp.ValueKind == JsonValueKind.String
                && Version.TryParse($"{versionProp}", out var releaseVersion))
            {
                if (releaseVersion > current)
                    return true;
            }
        }

        return false;
    }

    private ExtensionManifest? LoadInstalledManifest(string editionRoot, string extensionName)
    {
        var manifestPath = Path.Combine(editionRoot, FolderConstants.AppExtensionsFolder, extensionName, FolderConstants.DataFolderProtected, FolderConstants.AppExtensionJsonFile);
        return File.Exists(manifestPath)
            ? manifestService.LoadManifest(new FileInfo(manifestPath))
            : null;
    }

    private List<string> DetectInstalledEditions(string appRoot, string extensionName)
    {
        var list = new List<string>();
        var rootPath = Path.Combine(appRoot, FolderConstants.AppExtensionsFolder, extensionName);
        if (Directory.Exists(rootPath))
            list.Add(string.Empty);

        foreach (var dir in Directory.GetDirectories(appRoot))
        {
            var name = Path.GetFileName(dir);
            if (name.Equals(FolderConstants.AppExtensionsFolder, StringComparison.OrdinalIgnoreCase)
                || name.Equals(FolderConstants.AppCodeFolder, StringComparison.OrdinalIgnoreCase)
                || name.Equals(FolderConstants.DataFolderProtected, StringComparison.OrdinalIgnoreCase))
                continue;

            var editionExtPath = Path.Combine(dir, FolderConstants.AppExtensionsFolder, extensionName);
            if (Directory.Exists(editionExtPath))
                list.Add(name);
        }

        return list;
    }

    private static List<string> MergeEditions(List<string> requested, List<string> installed)
        => requested
            .Concat(installed)
            .Where(e => e != null)
            .Select(e => e!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private record ValidationResult(bool Success, string? Error);

    private ManifestValidationResult ValidateExtensionJsonFile(string extensionJsonFilePath, ExtensionManifestService manifestSvc)
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

    private record LockValidationResult(bool Success, string? Error, HashSet<string>? AllowedFiles) : ValidationResult(Success, Error);
    private record ManifestValidationResult(bool Success, string? Error, bool EditionsSupported, ExtensionManifest? Manifest) : ValidationResult(Success, Error);

    // Validate lock file against a single candidate folder only
    private LockValidationResult ValidateLockFile(string lockFilePath, string tempDir, string candidatePath)
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
    
    private void RemoveReadOnlyIfNeeded(string path, string relPath)
    {
        var l = Log.Fn();

        if (!File.Exists(path))
        {
            l.Done($"file not found: {relPath}");
            return;
        }

        var attributes = File.GetAttributes(path);
        if (!attributes.HasFlag(FileAttributes.ReadOnly))
        {
            l.Done($"file is not readonly: {relPath}");
            return;
        }

        File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
        l.Done($"cleared readonly:'{relPath}'");
    }

    private void EnsureReadOnly(string path, string relPath)
    {
        var l = Log.Fn();

        if (!File.Exists(path))
        {
            l.Done($"file not found: {relPath}");
            return;
        }

        var attributes = File.GetAttributes(path);
        if (attributes.HasFlag(FileAttributes.ReadOnly))
        {
            l.Done($"file is already readonly: {relPath}");
            return;
        }

        File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
        l.Done($"set readonly:'{relPath}'");
    }

    private static List<string> NormalizeEditions(string[]? editions)
    {
        var segments = (editions ?? Array.Empty<string>())
            .SelectMany(raw =>
            {
                // Split comma-delimited entries so DNN/webforms can send editions=staging,live
                if (raw == null)
                    return new[] { string.Empty };
                return raw.Split(new[] { ',' }, StringSplitOptions.None);
            });

        var normalized = segments
            .Select(e => e.NullIfNoValue()?.Trim().Trim('/', '\\') ?? string.Empty)
            .Select(e =>
            {
                if (e.ContainsPathTraversal())
                    throw new ArgumentException("edition contains invalid path traversal", nameof(editions));
                return e;
            })
            .ToList();

        if (normalized.Count == 0)
            normalized.Add(string.Empty);

        return normalized.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

}
