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

            // Clean ephemeral compiler artifacts from "2sxc.bin.cshtml\\temp" folder
            CleanAssemblyFolder(Path.Combine(globalConfiguration.CshtmlAssemblyFolder(), "temp"), "roslyn compiler temp Folder");

            Cleaned = true;
        }
    }

    /// <summary>
    /// Cleans a specific assembly folder by removing old/orphaned DLL files (including nested app/edition folders) and ephemeral compiler artifacts.
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

        // Clean recursively because cache is now nested per app/edition/shared
        var directories = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories)
            .Prepend(folderPath)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        const int retentionDays = 28;
        var now = DateTime.Now;

        foreach (var dir in directories)
        {
            var isRoot = string.Equals(dir, folderPath, StringComparison.OrdinalIgnoreCase);

            // Step 1: Remove ephemeral Roslyn temp files (*.cmdline, *.err, *.out, *.tmp, *.cs) but KEEP .pdb
            foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly))
            {
                var ext = Path.GetExtension(file);

                if (!EphemeralExtensions.Contains(ext))
                    continue;

                try
                {
                    File.Delete(file);
                }
                catch { /* ignore */ }
            }

            // Step 2a: legacy cleanup - delete any dll/pdb in the root folder (old flat layout, no longer used)
            if (isRoot)
            {
                foreach (var legacy in Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly)
                             .Concat(Directory.GetFiles(dir, "*.pdb", SearchOption.TopDirectoryOnly)))
                {
                    try { File.Delete(legacy); } catch { /* ignore */ }
                }
                // no retention logic needed on root after legacy cleanup
                continue;
            }

            // Step 2: delete old DLLs (and matching PDBs) past retention
            foreach (var dll in Directory.GetFiles(dir, "*" + Dll, SearchOption.TopDirectoryOnly))
            {
                var fi = new FileInfo(dll);
                if ((now - fi.LastWriteTime).TotalDays < retentionDays)
                    continue;

                try
                {
                    File.Delete(dll);
                }
                catch { /* ignore */ }

                var pdb = Path.ChangeExtension(dll, ".pdb");
                if (File.Exists(pdb))
                    try
                    {
                        File.Delete(pdb);
                    }
                    catch { /* ignore */ }
            }

            // Step 3: remove orphaned pdbs without matching dll
            foreach (var pdb in Directory.GetFiles(dir, "*.pdb", SearchOption.TopDirectoryOnly))
            {
                var dll = Path.ChangeExtension(pdb, ".dll");
                if (!File.Exists(dll))
                    try
                    {
                        File.Delete(pdb);
                    }
                    catch { /* ignore */ }
            }
        }

        // Step 4: remove empty subfolders (but leave the root)
        foreach (var dir in directories
                     .Where(d => !string.Equals(d, folderPath, StringComparison.OrdinalIgnoreCase))
                     .OrderByDescending(d => d.Length))
        {
            try
            {
                if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                    Directory.Delete(dir);
            }
            catch { /* ignore */ }
        }
    }
}
