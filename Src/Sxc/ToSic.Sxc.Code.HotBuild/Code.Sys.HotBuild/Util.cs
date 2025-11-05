using System.Text.RegularExpressions;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Code.Sys.HotBuild;

/// <summary>
/// On start need to clean the "2sxc.bin" (AppCode) and "2sxc.bin.cshtml" (Razor) folders,
/// because they are used to temporarily save compiled assemblies.
/// </summary>
public class Util(IGlobalConfiguration globalConfiguration)
{
    private const string Dll = ".dll";
    private static bool Cleaned { get; set; }
    private static readonly object CleaningLock = new();
    private static readonly HashSet<string> EphemeralExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cmdline", ".err", ".out", ".tmp", ".cs"
    };

    public void CleanTempAssemblyFolder()
    {
        // Ensure that cleaning is executed only once
        if (Cleaned) return;

        lock (CleaningLock)
        {
            if (Cleaned) return;

            // Clean AppCode folder (2sxc.bin)
            CleanAssemblyFolder(globalConfiguration.TempAssemblyFolder(), "AppCode (2sxc.bin)");

            // Clean Razor compiled templates folder (2sxc.bin.cshtml)
            CleanAssemblyFolder(globalConfiguration.CshtmlAssemblyFolder(), "Razor (2sxc.bin.cshtml)");

            // Clean ephemeral compiler artifacts from "2sxc.bin.cshtml\temp" folder
            CleanAssemblyFolder(Path.Combine(globalConfiguration.CshtmlAssemblyFolder(), "temp"), "roslyn compiler temp Folder");

            Cleaned = true;
        }
    }

    /// <summary>
    /// Cleans a specific assembly folder by removing old/orphaned DLL files and ephemeral compiler artifacts.
    /// Keeps corresponding .pdb files for debugging.
    /// </summary>
    private void CleanAssemblyFolder(string folderPath, string folderDescription)
    {
        if (string.IsNullOrEmpty(folderPath))
            return;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return; // nothing else to do first run
        }

        // Step 1: Remove ephemeral Roslyn temp files (*.cmdline, *.err, *.out, *.tmp, *.cs) but KEEP .pdb
        foreach (var file in Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly))
        {
            var ext = Path.GetExtension(file);

            if (!EphemeralExtensions.Contains(ext))
                continue;

            try { File.Delete(file); } catch { /* ignore */ }
        }

        // Step 2: Retention of DLL/PDB groups by prefix/hash; keep newest recent dll+pdb, remove older ones
        const int retentionDays = 28;
        var now = DateTime.Now;

        // Group dlls by prefix (without trailing hash) and choose which to keep
        var dllFiles = Directory.GetFiles(folderPath, "*" + Dll, SearchOption.TopDirectoryOnly)
            .Select(f => new FileInfo(f))
            .GroupBy(fi => GetFilePrefix(fi.Name));

        foreach (var group in dllFiles)
        {
            // Skip files which didn't match pattern
            if (group.Key == null)
                continue;
            
            // Sort files in the group by LastWriteTime descending (latest first)
            var ordered = group.OrderByDescending(f => f.LastWriteTime).ToList();
            
            // Find the first which is still within retention window
            var keep = ordered.FirstOrDefault(f => (now - f.LastWriteTime).TotalDays < retentionDays) ?? ordered.First();

            // Keep the latest file, delete the rest.
            foreach (var old in ordered.Where(f => f.FullName != keep.FullName))
            {
                try { File.Delete(old.FullName); } catch { /* ignore */ }
                // Also delete matching pdb for removed dll
                var pdb = Path.ChangeExtension(old.FullName, ".pdb");
                if (File.Exists(pdb)) try { File.Delete(pdb); } catch { /* ignore */ }
            }
        }
        // Remove orphaned pdbs without matching dll
        foreach (var pdb in Directory.GetFiles(folderPath, "*.pdb", SearchOption.TopDirectoryOnly))
        {
            var dll = Path.ChangeExtension(pdb, ".dll");
            if (!File.Exists(dll)) try { File.Delete(pdb); } catch { /* ignore */ }
        }
    }

    // Helper method to extract the prefix from the filename, validating the hash part
    private static string? GetFilePrefix(string fileName)
    {
        if (Path.GetExtension(fileName) != Dll) return null;
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var lastDashIndex = nameWithoutExtension.LastIndexOf('-');
        if (lastDashIndex < 0) return null;
        var hashCandidate = nameWithoutExtension.Substring(lastDashIndex + 1);
        return IsValidHash(hashCandidate) ? nameWithoutExtension.Substring(0, lastDashIndex) : null;
    }

    private static bool IsValidHash(string input)
    {
        const string hashPattern = @"^[a-fA-F0-9]{64}$|^[a-fA-F0-9]{6}$";
        return Regex.IsMatch(input, hashPattern);
    }
}