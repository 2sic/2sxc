using ToSic.Eav.ImportExport.Sys.Zip;
using ToSic.Eav.Sys;
using ToSic.Sxc.ImportExport.Package.Sys;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Backend.App.ExtensionLockHelper;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Handles safe copying of extension files from extracted temp folders into the target app.
/// </summary>
internal class ExtensionInstallHelper(ReadOnlyFileHelper readOnlyHelper, ILog? parentLog) : HelperBase(parentLog, "Bck.ExtCopy")
{
    internal ValidationResult InstallSingleExtension(string folderName, LockValidationResult lockValidation, string tempDir, string extensionsRoot, string appRoot, bool overwrite)
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
        var tempAppCodeExtensionFolderName = Path.Combine(tempDir, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, ExtensionValidationHelper.AppCodeExtensionFolderName(folderName));

        var extensionTarget = Path.Combine(extensionsRoot, folderName);
        var appCodeTarget = Path.Combine(appRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, folderName);

        var extensionTargetValidation = EnsureTargetReadyForCopy(tempExtensionFolder, extensionTarget, overwrite, FolderConstants.AppExtensionsFolder);
        if (!extensionTargetValidation.Success)
            return l.ReturnAsError(extensionTargetValidation);

        var appCodeTargetValidation = EnsureTargetReadyForCopy(tempAppCodeFolder, appCodeTarget, overwrite, FolderConstants.AppCodeFolder);
        if (!appCodeTargetValidation.Success)
            return l.ReturnAsError(appCodeTargetValidation);

        var appCodeExtensionFolderNameTargetValidation = EnsureTargetReadyForCopy(tempAppCodeExtensionFolderName, appCodeTarget, overwrite, FolderConstants.AppCodeFolder);
        if (!appCodeExtensionFolderNameTargetValidation.Success)
            return l.ReturnAsError(appCodeExtensionFolderNameTargetValidation);

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
            ExtensionValidationHelper.GetActualCasedPath(Path.Combine(sourceRoot, FolderConstants.AppCodeFolder, FolderConstants.AppExtensionsFolder, ExtensionValidationHelper.AppCodeExtensionFolderName(folderName))),
        };

        foreach (var source in sources)
        {
            foreach (var file in EnumerateFilesSafe(source))
            {
                var rel = file
                    .Substring(sourceRootFull.Length)
                    .TrimPrefixSlash()
                    .ForwardSlash();

                var isLockFile = rel.EndsWith(PackageIndexFile.LockFileName, StringComparison.OrdinalIgnoreCase);
                if (!isLockFile && !allowedFiles.Contains(rel))
                    continue;

                var destinationPath = Path.GetFullPath(Path.Combine(targetRoot, rel.Backslash()));
                if (!destinationPath.StartsWith(targetRootFull, StringComparison.OrdinalIgnoreCase))
                    return l.ReturnAsError(new(false, $"illegal destination path:'{rel}'"));

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                readOnlyHelper.RemoveReadOnlyIfNeeded(destinationPath, rel);
                File.Copy(file, destinationPath, overwrite: true);
                readOnlyHelper.EnsureReadOnly(destinationPath, rel);
                l.A($"copied:'{rel}'");
            }
        }

        return l.ReturnAsOk(new(true, null));
    }
}
