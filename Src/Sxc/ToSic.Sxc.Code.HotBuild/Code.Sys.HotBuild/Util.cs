using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Code.Sys.HotBuild;

/// <summary>
/// On start need to clean the "2sxc.bin" (AppCode) and "2sxc.bin.cshtml" (Razor) folders,
/// because they are used to temporarily save compiled assemblies.
/// </summary>
public class Util(IGlobalConfiguration globalConfiguration)
{
    private const string Dll = ".dll";
    private const int RetentionDays = 28;
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(RetentionDays);
    private static bool Cleaned { get; set; }
    private static readonly object CleaningLock = new();
    private static readonly HashSet<string> EphemeralExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cmdline", ".err", ".out", ".tmp", ".cs"
    };

    public void CleanTempAssemblyFolder()
    {
        // Ensure that cleaning is executed only once
        if (Cleaned)
            return;

        lock (CleaningLock)
        {
            if (Cleaned)
                return;

            // Clean AppCode folder (2sxc.bin)
            CleanAssemblyFolder(globalConfiguration.TempAssemblyFolder(), "AppCode (2sxc.bin)", AssemblyFolderType.AppCode);

            // Clean Razor compiled templates folder (2sxc.bin.cshtml)
            CleanAssemblyFolder(globalConfiguration.CshtmlAssemblyFolder(), "Razor (2sxc.bin.cshtml)", AssemblyFolderType.Cshtml);

            // Clean ephemeral compiler artifacts from "2sxc.bin.cshtml\\temp" folder
            CleanAssemblyFolder(Path.Combine(globalConfiguration.CshtmlAssemblyFolder(), "temp"), "roslyn compiler temp Folder", AssemblyFolderType.None);

            Cleaned = true;
        }
    }

    /// <summary>
    /// Cleans a specific assembly folder by removing old/orphaned DLL files (including nested app/edition folders) and ephemeral compiler artifacts.
    /// Keeps corresponding .pdb files for debugging.
    /// </summary>
    private void CleanAssemblyFolder(string folderPath, string folderDescription, AssemblyFolderType folderType)
    {
        _ = folderDescription;
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
                    try
                    {
                        File.Delete(legacy);
                    } catch { /* ignore */ }
                }
                // no retention logic needed on root after legacy cleanup
                continue;
            }

            // Step 2b: keep only the newest assembly per prefix and purge younger duplicates
            EnsureSingleNewestPerPrefix(dir, folderType, now);

            // Step 3: delete old DLLs (and matching PDBs) past retention
            foreach (var dll in Directory.GetFiles(dir, "*" + Dll, SearchOption.TopDirectoryOnly))
            {
                var fi = new FileInfo(dll);
                if ((now - fi.LastWriteTime) < RetentionPeriod)
                    continue;

                DeleteAssemblyWithSymbols(dll);
            }

            // Step 4: remove orphaned pdbs without matching dll
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

        // Step 5: remove empty subfolders (but leave the root)
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

    private void EnsureSingleNewestPerPrefix(string dir, AssemblyFolderType folderType, DateTime now)
    {
        if (folderType == AssemblyFolderType.None)
            return;

        var groupedFiles = Directory.GetFiles(dir, "*" + Dll, SearchOption.TopDirectoryOnly)
            .Select(path => new FileInfo(path))
            .Select(fileInfo => new
            {
                File = fileInfo,
                Prefix = GetFilePrefix(fileInfo, folderType)
            })
            .Where(candidate => !string.IsNullOrEmpty(candidate.Prefix))
            .GroupBy(candidate => candidate.Prefix!, StringComparer.OrdinalIgnoreCase);

        foreach (var group in groupedFiles)
        {
            var ordered = group
                .OrderByDescending(candidate => candidate.File.LastWriteTime)
                .ToList();

            var keep = ordered
                .FirstOrDefault(candidate => (now - candidate.File.LastWriteTime) < RetentionPeriod);

            foreach (var candidate in ordered)
            {
                if (keep != null && candidate.File.FullName.Equals(keep.File.FullName, StringComparison.OrdinalIgnoreCase))
                    continue;

                DeleteAssemblyWithSymbols(candidate.File.FullName);
            }
        }
    }

    private static string? GetFilePrefix(FileInfo fileInfo, AssemblyFolderType folderType)
        => folderType switch
        {
            AssemblyFolderType.AppCode => GetAppCodePrefix(fileInfo) ?? GetDepPrefix(fileInfo),
            AssemblyFolderType.Cshtml => GetCshtmlPrefix(fileInfo),
            _ => null
        };

    private static string? GetAppCodePrefix(FileInfo fileInfo)
    {
        if (!string.Equals(fileInfo.Extension, Dll, StringComparison.OrdinalIgnoreCase))
            return null;

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
        var lastDashIndex = fileNameWithoutExtension.LastIndexOf('-');
        if (lastDashIndex <= 0 || lastDashIndex == fileNameWithoutExtension.Length - 1)
            return null;

        if (!fileNameWithoutExtension.StartsWith("AppCode-", StringComparison.OrdinalIgnoreCase))
            return null;

        var hash = fileNameWithoutExtension.Substring(lastDashIndex + 1);
        if (!IsHex(hash, minLength: 6, maxLength: 64))
            return null;

        return fileNameWithoutExtension.Substring(0, lastDashIndex);
    }

    private static string? GetDepPrefix(FileInfo fileInfo)
    {
        if (!string.Equals(fileInfo.Extension, Dll, StringComparison.OrdinalIgnoreCase))
            return null;

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
        var lastDashIndex = fileNameWithoutExtension.LastIndexOf('-');
        if (lastDashIndex <= 0 || lastDashIndex == fileNameWithoutExtension.Length - 1)
            return null;

        if (!fileNameWithoutExtension.StartsWith("dep-", StringComparison.OrdinalIgnoreCase))
            return null;

        var hash = fileNameWithoutExtension.Substring(lastDashIndex + 1);
        if (!IsHex(hash, minLength: 8, maxLength: 64))
            return null;

        return fileNameWithoutExtension.Substring(0, lastDashIndex);
    }
    private static string? GetCshtmlPrefix(FileInfo fileInfo)
    {
        if (!string.Equals(fileInfo.Extension, Dll, StringComparison.OrdinalIgnoreCase))
            return null;

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
        var segments = fileNameWithoutExtension.Split('-');
        if (segments.Length < 3)
            return null;

        var contentHash = segments[segments.Length - 2];
        var appCodeHash = segments[segments.Length - 1];

        if (!IsHex(contentHash, minLength: 6, maxLength: 64)
            || !IsHex(appCodeHash, minLength: 6, maxLength: 64))
            return null;

        return string.Join("-", segments.Take(segments.Length - 2));
    }

    private static bool IsHex(string value, int minLength, int maxLength)
    {
        if (string.IsNullOrEmpty(value)
            || value.Length < minLength
            || value.Length > maxLength)
            return false;

        foreach (var c in value)
        {
            var isHex = c is >= '0' and <= '9'
                or (>= 'a' and <= 'f')
                or (>= 'A' and <= 'F');

            if (!isHex)
                return false;
        }

        return true;
    }

    private static void DeleteAssemblyWithSymbols(string dllPath)
    {
        try
        {
            File.Delete(dllPath);
        }
        catch { /* ignore */ }

        var pdb = Path.ChangeExtension(dllPath, ".pdb");
        if (string.IsNullOrEmpty(pdb) || !File.Exists(pdb))
            return;

        try
        {
            File.Delete(pdb);
        }
        catch { /* ignore */ }
    }

    private enum AssemblyFolderType
    {
        None,
        AppCode,
        Cshtml
    }
}
