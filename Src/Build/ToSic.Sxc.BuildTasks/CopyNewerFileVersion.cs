using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace ToSic.Sxc.BuildTasks
{
    /// <summary>
    /// Represents a custom MSBuild task that copies files from source to destination
    /// with additional logic for handling assembly files (DLLs), 
    /// where overwrite will happen only if source FileVersion is newer than on destination
    /// </summary>
    public class CopyNewerFileVersion : Task
    {
        private const string LogPrefix = $"{nameof(CopyNewerFileVersion)}:";

        /// <summary>
        /// Gets or sets the source files to be copied.
        /// </summary>
        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        /// <summary>
        /// Gets or sets the destination folder where the files will be copied.
        /// </summary>
        [Required]
        public string DestinationFolder { get; set; }

        /// <summary>
        /// Executes the file copy operation.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the operation succeeds; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method iterates through the source files, checks if they need to be copied
        /// (based on version comparison for DLLs), and performs the copy operation.
        /// </remarks>
        public override bool Execute()
        {
            foreach (var sourceFile in SourceFiles)
            {
                var sourcePath = sourceFile.ItemSpec;
                var relativePath = sourceFile.GetMetadata("RecursiveDir");
                var destPath = Path.Combine(DestinationFolder, relativePath ?? "", Path.GetFileName(sourcePath));

                try
                {
                    if (!File.Exists(sourcePath))
                    {
                        Log.LogMessage(MessageImportance.High, $"{LogPrefix} Source file '{sourcePath}' not found. Skipping.");
                        continue;
                    }

                    var copy = true;

                    if (File.Exists(destPath) && Path.GetExtension(sourcePath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var srcVer = GetFileVersion(sourcePath);
                            var dstVer = GetFileVersion(destPath);

                            if (srcVer <= dstVer)
                            {
                                Log.LogMessage(MessageImportance.High, $"{LogPrefix} Skipping '{sourcePath}' — version {srcVer} <= {dstVer}");
                                copy = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogMessage(MessageImportance.High, $"{LogPrefix} Error comparing versions: {ex.Message}. Proceeding to copy.");
                        }
                    }

                    if (copy)
                    {
                        var directoryName = Path.GetDirectoryName(destPath);
                        if (!string.IsNullOrEmpty(directoryName)) Directory.CreateDirectory(directoryName);

                        File.Copy(sourcePath, destPath, true);
                        Log.LogMessage(MessageImportance.High, $"{LogPrefix} Copied '{sourcePath}' to '{destPath}'");
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError($"{LogPrefix} Error failed to copy '{sourcePath}' to '{destPath}': {ex.Message}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Retrieves the version information of a file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The <see cref="Version"/> of the file.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        private static Version GetFileVersion(string filePath)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            return new Version(versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
        }
    }
}
