using System.IO;
using System.Text.RegularExpressions;
using ToSic.Eav.Internal.Configuration;

namespace ToSic.Sxc.Code.Internal.HotBuild;

/// <summary>
/// On start need to clean the "2sxc.bin", because it is used to temporarily save shared temp AppCode assemblies.
/// </summary>
public class Util(IGlobalConfiguration globalConfiguration)
{
    private const string Dll = ".dll";
    private static bool Cleaned { get; set; }
    private static readonly object CleaningLock = new();

    public void CleanTempAssemblyFolder()
    {
        // Ensure that cleaning is executed only once
        if (Cleaned) return;

        lock (CleaningLock)
        {
            if (Cleaned) return;

            if (Directory.Exists(globalConfiguration.TempAssemblyFolder))
            {
                // *** Step 1. delete all files without Hash
                var filesWithoutHashToDelete = Directory.GetFiles(globalConfiguration.TempAssemblyFolder, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => GetFilePrefix(file) == null);

                foreach(var file in filesWithoutHashToDelete)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // sink
                    }
                }

                // *** Step 2. Keep only the latest file for each group of files with the same prefix ***
                const int retentionDays = 28;
                var currentDateTime = DateTime.Now;

                // Group files by their prefix (excluding the hash part if valid)
                var groupedFiles = Directory.GetFiles(globalConfiguration.TempAssemblyFolder, "*" + Dll, SearchOption.TopDirectoryOnly)
                    .Select(f => new FileInfo(f))
                    .GroupBy(fileInfo => GetFilePrefix(fileInfo.Name));

                foreach (var group in groupedFiles)
                {
                    // Sort files in the group by LastWriteTime descending (latest first)
                    var filesInGroup = group.OrderByDescending(f => f.LastWriteTime).ToList();

                    // Find the latest file if it's less than 28 days old.
                    var keepFile = filesInGroup.FirstOrDefault(f => (currentDateTime - f.LastWriteTime).TotalDays < retentionDays);

                    // Keep the latest file, delete the rest.
                    var filesToDelete = filesInGroup.Where(f => f != keepFile);
                    foreach (var fileInfo in filesToDelete)
                    {
                        try
                        {
                            File.Delete(fileInfo.FullName);
                        }
                        catch
                        {
                            // sink
                        }
                    }
                }
            }
            else
            {
                // Ensure "2sxc.bin" folder exists to preserve dlls
                Directory.CreateDirectory(globalConfiguration.TempAssemblyFolder);
            }

            Cleaned = true;
        }
    }

    // Helper method to extract the prefix from the filename, validating the hash part
    private static string GetFilePrefix(string fileName)
    {
        if (Path.GetExtension(fileName) != Dll)
            return null;

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var lastDashIndex = nameWithoutExtension.LastIndexOf('-');

        // Validate the potential hash using a regex (e.g., 64-character hexadecimal)
        if (lastDashIndex >= 0 && IsValidHash(nameWithoutExtension.Substring(lastDashIndex + 1)))
            // Valid hash found, return the prefix without the hash
            return nameWithoutExtension.Substring(0, lastDashIndex);

        // No hyphen found or valid hash
        return null;
    }

    private static bool IsValidHash(string input)
    {
        // Pattern to match SHA256 hash format
        const string hashPattern = @"^[a-fA-F0-9]{64}$";
        return Regex.IsMatch(input, hashPattern);
    }
}